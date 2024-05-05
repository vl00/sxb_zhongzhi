using CSRedis;
using Dapper;
using iSchool.Infrastructure;
using iSchool.Infrastructure.Dapper;
using iSchool.Infrastructure.UoW;
using iSchool.Svs.Appliaction.RequestModels;
using iSchool.Svs.Appliaction.ResponseModels;
using iSchool.Svs.Appliaction.ViewModels;
using iSchool.Svs.Domain;
using iSchool.Svs.Domain.Enum;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace iSchool.Organization.Appliaction.Service
{
    public class SvsSchoolPageListQueryHandler : IRequestHandler<SvsSchoolPageListQuery, SvsSchoolPageListQueryResult>
    {
        SvsUnitOfWork unitOfWork;
        IMediator mediator;
        CSRedisClient redis;

        public SvsSchoolPageListQueryHandler(ISvsUnitOfWork unitOfWork, IMediator mediator, CSRedisClient redis)
        {
            this.unitOfWork = (SvsUnitOfWork)unitOfWork;
            this.mediator = mediator;
            this.redis = redis;
        }

        public async Task<SvsSchoolPageListQueryResult> Handle(SvsSchoolPageListQuery query, CancellationToken cancellation)
        {
            var result = new SvsSchoolPageListQueryResult { SearchTxt = query.SearchTxt };
            await default(ValueTask);

            var pr = (query.Pr ?? new string[0]).Select(_ => get_intId(_)).Where(_ => _ != null && _ != 0).Select(_ => _.Value);
            var ci = (query.Ci ?? new string[0]).Select(_ => get_intId(_)).Where(_ => _ != null && _ != 0).Select(_ => _.Value);
            var ma = (query.Ma ?? new string[0]).Union(query.Mas ?? new string[0]).Select(_ => get_intId(_)).Where(_ => _ != null && _ != 0).Select(_ => _.Value);
            var ty = (query.Ty ?? new string[0]).Select(_ => get_intId(_)).Where(_ => _ != null && _ != 0).Select(_ => _.Value);
            var na = (query.Na ?? new string[0]).Select(_ => get_intId(_)).Where(_ => _ != null && _ != 0).Select(_ => _.Value);
            var lv = (query.Lv ?? new string[0]).Select(_ => get_intId(_)).Where(_ => _ != null && _ != 0).Select(_ => _.Value);

            var (ma1, ma2) = (Enumerable.Empty<int>(), Enumerable.Empty<int>());
            if (ma.Any())
            {
                var itmas = await mediator.Send(new GetSearchItemsQuery
                {
                    ["ma"] = new GetOneSearchTypeItemsQuery { SelectedDirection = 1, Selected = ma.Select(_ => _.ToString()).ToArray() },
                });

                ma2 = itmas.Ma.Where(x => (int)x._Ext1["depth"] >= 2)
                    .Select(x => get_intId(x.Id)).Distinct()
                    .Where(_ => _ != null && _ != 0).Select(_ => _.Value);

                ma1 = itmas.Ma.Where(x1 => (int)x1._Ext1["depth"] == 1 &&
                        !x1.Id.In(itmas.Ma.Where(x2 => (int)x2._Ext1["depth"] == 2).Select(x2 => x2.Pid).ToArray())
                    ).Select(x => get_intId(x.Id)).Distinct()
                    .Where(_ => _ != null && _ != 0).Select(_ => _.Value);

                ma1 = ma1.Any() ? ma1.AsArray() : new[] { -1 };
                ma2 = ma2.Any() ? ma2.AsArray() : new[] { -1 };
            }

            var sql = $@"select * from(
select s.status,s.id,s.no as id_s,s.name,s.logo,s.type,s.[level],s.nature,s.address,s.tel
{",(case when contains(s.name,@SearchTxt) then 2 when freetext(s.name,@SearchTxt) then 1 else 0 end)_ti".If(!query.SearchTxt.IsNullOrWhiteSpace())}
from School s 
where s.IsValid=1 and s.status={SvsSchoolStatus.Success.ToInt()} 
{(!query.SearchTxt.IsNullOrWhiteSpace() ? $@"
and (contains(s.name,@SearchTxt) or freetext(s.name,@SearchTxt))
" : string.Empty)}
{"and s.province in @pr".If(pr.Any())} {"and s.city in @ci".If(ci.Any())}
{"and s.type in @ty".If(ty.Any())} {"and s.nature in @na".If(na.Any())} {"and s.[level] in @lv".If(lv.Any())} 
{(ma.Any() ? $@"and exists(select 1 from SchoolMajor sm 
	left join Major m on m.id=sm.majorid and m.IsValid=1 and m.[level]>=2 --暂时只支持3级
    left join Major m2 on m2.IsValid=1 and ((m.[level]=2 and m2.id=m.id) or (m.[level]=3 and m2.id=m.pid))
	left join Major m1 on m1.id=m2.pid and m1.IsValid=1 and m1.[level]=1
	where sm.IsValid=1 and sm.sid=s.id 
	and (m1.no in ({string.Join(',', ma1)}) or m2.no in ({string.Join(',', ma2)}))
)" : string.Empty)}
)T ";
            sql = $@"
select count(1) from ({sql}) _t
-------
{sql}
order by {"_ti desc,".If(!query.SearchTxt.IsNullOrWhiteSpace())}id_s
OFFSET (@pageIndex-1)*@pageSize ROWS FETCH NEXT @pageSize ROWS ONLY
";
            var dy = new DynamicParameters()
                .Set("@pageIndex", query.PageIndex)
                .Set("@pageSize", query.PageSize)
                .Set("@SearchTxt", query.SearchTxt)
                .Set("@pr", pr).Set("@ci", ci)                
                .Set("@ty", ty).Set("@na", na).Set("@lv", lv)
                ;

            var gr = await unitOfWork.DbConnection.QueryMultipleAsync(sql, dy);
            var cc = await gr.ReadFirstAsync<int>();
            var pg = (await gr.ReadAsync<SvsSchoolItem1Dto>()).AsArray();

            var rr = await mediator.Send(new SvsSchoolsTop2HotMajorsQuery { SchoolsIds = pg.Select(_ => _.Id) });
            foreach (var s in pg)
            {
                if (!rr.Items.TryGetValue(s.Id, out var r)) s.HotMajors = Enumerable.Empty<string>();
                else s.HotMajors = r.Select(_ => _.MajorName);
                s.Level = s.Level.IsNullOrEmpty() ? null : EnumUtil.GetDesc(s.Level.ToEnum<SvsSchoolLevel>());
                s.Type = s.Type.IsNullOrEmpty() ? null : EnumUtil.GetDesc(s.Type.ToEnum<SvsSchoolType>());
                s.Nature = s.Nature.IsNullOrEmpty() ? null : EnumUtil.GetDesc(s.Nature.ToEnum<SvsSchoolNature>());
            }

            result.SchoolsPageInfo = pg.ToPagedList(query.PageSize, query.PageIndex, cc);
            return result;
        }


        static int? get_intId(string id)
        {
            if (id == null) return null;
            var gs = Regex.Match(id, @"(?<code>\d*)$", RegexOptions.IgnoreCase).Groups;
            var s = gs["code"].Value;
            return int.TryParse(s, out var _i) ? _i : (int?)null;
        }
    }
}
