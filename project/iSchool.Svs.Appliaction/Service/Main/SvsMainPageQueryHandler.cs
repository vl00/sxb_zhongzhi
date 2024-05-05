using CSRedis;
using Dapper;
using iSchool.Infrastructure;
using iSchool.Infrastructure.Extensions;
using iSchool.Infrastructure.UoW;
using iSchool.Svs.Appliaction.RequestModels;
using iSchool.Svs.Appliaction.ResponseModels;
using iSchool.Svs.Appliaction.ViewModels;
using iSchool.Svs.Domain;
using iSchool.Svs.Domain.Enum;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace iSchool.Organization.Appliaction.Service
{
    public class SvsMainPageQueryHandler : IRequestHandler<SvsMainPageQuery, SvsMainPageResult>
    {
        SvsUnitOfWork unitOfWork;
        IMediator mediator;
        CSRedisClient redis;
        IConfiguration config;

        public SvsMainPageQueryHandler(ISvsUnitOfWork unitOfWork, IMediator mediator, CSRedisClient redis, IConfiguration config)
        {
            this.unitOfWork = (SvsUnitOfWork)unitOfWork;
            this.mediator = mediator;
            this.redis = redis;
            this.config = config;
        }

        public async Task<SvsMainPageResult> Handle(SvsMainPageQuery query, CancellationToken cancellation)
        {
            var result = new SvsMainPageResult();
            await default(ValueTask);

            // for 'SelectedProvince' 'Provinces'
            if (query.Province >= 0)
            {
                var rr = await mediator.Send(new GetSearchItemsQuery
                {
                    ["prci"] = new GetOneSearchTypeItemsQuery { Selected = new[] { query.Province.ToString() } }
                });
                if (rr.Prci?.Any() == true)
                {
                    result.Provinces = rr.Prci.Select(x =>
                    {
                        return new AreaNameCodeDto
                        {
                            Name = x.Name,
                            Code = (int)(get_intId(x.Id) ?? -1L),
                        };
                    });
                    result.SelectedProvince = result.Provinces.FirstOrDefault(x => x.Code == query.Province)
                        ?? new AreaNameCodeDto { Name = "全国", Code = 0 };
                }
                else
                {
                    result.SelectedProvince = new AreaNameCodeDto { Name = "全国", Code = 0 };
                    result.Provinces = new AreaNameCodeDto[] { new AreaNameCodeDto { Name = "全国", Code = 0 } };
                }
            }

            // 院校库
            do
            {
                result.Schools = redis.Get<CgyItemListDto<SvsSchoolListItem1Dto>[]>(CacheKeys.MainSchoolResps.FormatWith(query.Province));
                if (result.Schools != null) break;

                var schools = EnumUtil.GetDescs<SvsSchoolType>().Select(_ => new CgyItemListDto<SvsSchoolListItem1Dto>
                {
                    Id = $"{_.Value.ToInt()}",
                    Name = _.Desc,
                }).AsArray();
                result.Schools = schools;

                var types = EnumUtil.GetDescs<SvsSchoolType>().Select(_ => _.Value);
                var sql = $@"
select * from (select 
id,id_s,name,logo,type,[level],nature,address,count(m2no) as MajorCount,row_number()over(partition by type order by id_s,count(m2no) desc)as _i
from(select DISTINCT s.id,s.no as id_s,s.name,s.logo,s.type,s.[level],s.nature,s.address,
m2.no as m2no,m2.name as m2name
from School s left join SchoolMajor sm on sm.IsValid=1 and sm.sid=s.id
left join Major m2 on m2.id=sm.majorid and m2.IsValid=1 and m2.[level]>=2 
where s.IsValid=1 and s.status={SvsSchoolStatus.Success.ToInt()} ---and s.show=1
{"and isnull(s.province,0)=@Province".If(query.Province > 0)}
and s.type in @types 
) T group by id,id_s,name,logo,type,[level],nature,address
) T where _i<=@size
";
                var ls = unitOfWork.DbConnection.Query<SvsSchoolListItem1Dto>(sql, new { query.Province, types, size = 6 }).AsArray();

                foreach (var school in schools)
                {
                    school.List = ls.Where(_ => _.Type == school.Id).OrderByDescending(_ => _.MajorCount).ThenBy(_ => _.Id_s).ToArray();
                }
                // ready to respone - some number to string
                foreach (var school in schools)
                {
                    school.Id = "ty" + school.Id;
                    if (school.List == null) continue;
                    foreach (var a in school.List)
                    {
                        a.Id_s = UrlShortIdUtil.Long2Base32(get_intId(a.Id_s).Value);
                        if (a.Type != null) a.Type = EnumUtil.GetDesc(a.Type.ToEnum<SvsSchoolType>());
                        if (a.Level != null) a.Level = EnumUtil.GetDesc(a.Level.ToEnum<SvsSchoolLevel>());
                        if (a.Nature != null) a.Nature = EnumUtil.GetDesc(a.Nature.ToEnum<SvsSchoolNature>());
                    }
                }

                await redis.SetAsync(CacheKeys.MainSchoolResps.FormatWith(query.Province), schools, 60 * 60);
            }
            while (false);

            // 热门分类
            do
            {
                result.Hots = await redis.GetAsync<CgyItemListDto<SvsMajorItem1Dto>[]>(CacheKeys.MainHots);
                if (result.Hots?.Any() == true) break;

                var hots = config.GetSection("AppSettings:svs_mainpage:hot_cgy").Get<CgyItemListDto<SvsMajorItem1Dto>[]>();
                result.Hots = hots;
                var cIds = hots.Select(_ => _.Id).Where(_ => !_.IsNullOrEmpty()).ToArray();

                Array.ForEach(hots, a =>
                {
                    if (!string.IsNullOrEmpty(a.Id)) a.Id = UrlShortIdUtil.Long2Base32(Convert.ToInt64(a.Id));
                    else a.Id = null;
                });

                var sql = @"
select --c.name,m.[level],
convert(int,mc.sort)as sort,c.no as cNO,m.Name,m.no as Id_s,m.logo as Img,
((case m.[level] when 1 then 'ma' when 2 then 'mas' else '' end)+convert(varchar(10),m.no)) as Code
from Category c 
left join MajorCategory mc on mc.categoryid=c.id and mc.IsValid=1
left join Major m on m.IsValid=1 and m.id=mc.majorid
where c.IsValid=1 and c.type=1 and c.no in @cIds
and mc.type=0
order by c.no,mc.sort
";
                var ls = await unitOfWork.DbConnection.QueryAsync<int, int, SvsMajorItem1Dto, (int Sort, int Cno, SvsMajorItem1Dto Dto)>(
                    sql, (sort, cNO, dto) => (sort, cNO, dto), new { cIds }, splitOn: "cNO,Name");

                var ls1 = ls.GroupBy(_ => _.Cno, _ => (_.Sort, _.Dto))
                    .ToDictionary(_ => UrlShortIdUtil.Long2Base32(_.Key), x => x.OrderBy(_ => _.Sort)
                        .Select(o => o.Dto.Todo(_o => _o.Id_s = UrlShortIdUtil.Long2Base32(Convert.ToInt64(_o.Id_s))))
                        .ToArray());

                foreach (var hot in hots)
                {
                    if (string.IsNullOrEmpty(hot.Id) || !ls1.TryGetValue(hot.Id, out var itm)) continue;
                    hot.List = itm.Take(9).AsArray();
                }

                await redis.SetAsync(CacheKeys.MainHots, hots, 60 * 60 * 6);
            }
            while (false);

            // 专业库
            do
            {
                var majorResps = config.GetSection("AppSettings:svs_mainpage:ma_cgy").Get<CgyItemListDto<SvsMajorItem1Dto>[]>();
                result.MajorResps = majorResps;

                var maIds = result.MajorResps.Select(_ => _.Id).Where(_ => !_.IsNullOrEmpty());
                var rr = await mediator.Send(new GetSearchItemsQuery
                {
                    ["ma"] = new GetOneSearchTypeItemsQuery { SelectedDirection = 1, Selected = maIds.AsArray() },
                });
                foreach (var m in majorResps)
                {
                    var ls = rr.Ma?.FirstOrDefault(_ => _.Id == m.Id)?.List;
                    if (ls == null) continue;
                    m.List = ls.Take(9).Select(_ => new SvsMajorItem1Dto
                    {
                        Code = _.Id,
                        Name = _.Name,
                        Img = (string)_._Ext1["logo"],
                        Id_s = UrlShortIdUtil.Long2Base32(get_intId(_.Id).Value),
                    });
                }
            }
            while (false);

            return result;
        }


        static long? get_intId(string id)
        {
            if (id == null) return null;
            var gs = Regex.Match(id, @"(?<code>\d*)$", RegexOptions.IgnoreCase).Groups;
            var s = gs["code"].Value;
            return long.TryParse(s, out var _i) ? _i : (long?)null;
        }
    }
}
