using CSRedis;
using Dapper;
using iSchool.Infrastructure;
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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace iSchool.Organization.Appliaction.Service
{
    public class SvsSchoolsTop2HotMajorsQueryHandler : IRequestHandler<SvsSchoolsTop2HotMajorsQuery, SvsSchoolsTop2HotMajorsQyResult>
    {
        SvsUnitOfWork unitOfWork;
        IMediator mediator;
        CSRedisClient redis;

        public SvsSchoolsTop2HotMajorsQueryHandler(ISvsUnitOfWork unitOfWork, IMediator mediator, CSRedisClient redis)
        {
            this.unitOfWork = (SvsUnitOfWork)unitOfWork;
            this.mediator = mediator;
            this.redis = redis;
        }

        public async Task<SvsSchoolsTop2HotMajorsQyResult> Handle(SvsSchoolsTop2HotMajorsQuery query, CancellationToken cancellation)
        {
            var result = new SvsSchoolsTop2HotMajorsQyResult();
            query.SchoolsIds ??= Enumerable.Empty<Guid>();
            await default(ValueTask);

            result.Items = query.SchoolsIds.Distinct().ToDictionary(_ => _, _ => ((Guid, string)[])null);
            if (!result.Items.Any()) return result;

            // try find from redis
            //
            foreach (var arr in SplitArr(query.SchoolsIds, 5))
            {
                var pipe = redis.StartPipe();
                Array.ForEach(arr, a => pipe.Get(CacheKeys.SchoolTop2HotMajors.FormatWith(a)));
                var rr = (await pipe.EndPipeAsync()).OfType<string>();
                for (var i = 0; i < arr.Length; i++)
                {
                    var r = rr.ElementAtOrDefault(i) is string _r ? _r.ToObject<(Guid, string)[]>() : default;
                    if (r == default) continue;
                    result.Items[arr[i]] = r;
                }                
            }

            // try find and set to redis
            //
            var ls_notIn = result.Items.Where(_ => _.Value == null).Select(_ => _.Key).ToArray();
            if (ls_notIn.Any())
            {
                var sql = $@"-- 已过滤SchoolMajor表major,system字段带来的重复专业问题
select sid as item1,string_agg(convert(nvarchar(max),majorid)+' '+majorname,char(10))within group(order by viewcount desc) as item2
from (
select s.id as sid,m.id as majorid,m.name as majorname,isnull(m.viewcount,0)as viewcount
from School s
left join SchoolMajor sm on sm.sid=s.id and sm.IsValid=1
left join Major m on m.id=sm.majorid 
where s.IsValid=1 and s.status={SvsSchoolStatus.Success.ToInt()} and m.IsValid=1 and m.[level]>=2
and s.id in @SchoolIds
group by s.id,m.id,m.name,isnull(m.viewcount,0)
--order by s.id,isnull(m.viewcount,0) desc
)T group by sid
";
                var ls = unitOfWork.DbConnection.Query<(Guid Sid, string Majors)>(sql, new { SchoolIds = ls_notIn });
                var pipe = redis.StartPipe();
                foreach (var sid in ls_notIn)
                {
                    var (sid1, majors) = ls.FirstOrDefault(_ => _.Sid == sid);
                    if (sid1 == default)
                    {
                        result.Items[sid] = Array.Empty<(Guid, string)>();
                    }
                    else
                    {
                        result.Items[sid] = majors.Split('\n').Take(2).Select(str => 
                        {
                            return (Guid.Parse(str[..36]), str[37..]);
                        }).ToArray();
                    }
                    pipe.Set(CacheKeys.SchoolTop2HotMajors.FormatWith(sid), result.Items[sid], 60 * 60 * 3);
                }
                await pipe.EndPipeAsync();
            }

            return result;
        }

        static IEnumerable<T[]> SplitArr<T>(IEnumerable<T> collection, int c /* c > 0 */)
        {
            for (var arr = collection; arr.Any();)
            {
                yield return arr.Take(c).ToArray();
                arr = arr.Skip(c);
            }
        }

    }
}
