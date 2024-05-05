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
    public class RecommendSchoolsQueryHandler : IRequestHandler<RecommendSchoolsQuery, RecommendSchoolsQyResult>
    {
        SvsUnitOfWork unitOfWork;
        IMediator mediator;
        CSRedisClient redis;

        public RecommendSchoolsQueryHandler(ISvsUnitOfWork unitOfWork, IMediator mediator, CSRedisClient redis)
        {
            this.unitOfWork = (SvsUnitOfWork)unitOfWork;
            this.mediator = mediator;
            this.redis = redis;
        }

        public async Task<RecommendSchoolsQyResult> Handle(RecommendSchoolsQuery query, CancellationToken cancellation)
        {
            var result = new RecommendSchoolsQyResult();
            await default(ValueTask);

            result.RecommendSchools = await redis.GetAsync<SvsSchoolItem1Dto[]>(CacheKeys.MainRecommendSchools.FormatWith(query.Province));
            if (result.RecommendSchools == null)
            {
                var sql = $@"
select top 10 r.weight,r.province,s.status,
s.id,s.no as id_s,s.name,s.logo,s.type,s.[level],s.nature,s.address,s.tel
from Recommend r left join School s on r.contentid=s.id
where r.IsValid=1 and r.type={RecommendType.School.ToInt()} and isnull(r.province,0)=@Province ---
and s.IsValid=1 and s.status={SvsSchoolStatus.Success.ToInt()} ---and s.show=1
and getdate() between isnull(r.time_start,'2000-01-01') and isnull(r.time_end,'9999-12-31')
order by r.weight,s.no
";
                result.RecommendSchools = unitOfWork.DbConnection.Query<SvsSchoolItem1Dto>(sql, new { query.Province }).AsArray();

                var schoolIds = result.RecommendSchools.Select(_ => _.Id);
                var rr = await mediator.Send(new SvsSchoolsTop2HotMajorsQuery { SchoolsIds = schoolIds });
                foreach (var school in result.RecommendSchools)
                {
                    if (!rr.Items.TryGetValue(school.Id, out var arr) || arr?.Any() != true)
                        school.HotMajors = Enumerable.Empty<string>();
                    else
                        school.HotMajors = arr.Select(_ => _.MajorName);

                    school.Type = EnumUtil.GetDesc(school.Type.ToEnum<SvsSchoolType>());
                    school.Nature = EnumUtil.GetDesc(school.Nature.ToEnum<SvsSchoolNature>());
                    school.Level = EnumUtil.GetDesc(school.Level.ToEnum<SvsSchoolLevel>());
                }

                await redis.SetAsync(CacheKeys.MainRecommendSchools.FormatWith(query.Province), result.RecommendSchools,
                    60 * 60 * 3);
            }

            return result;
        }

        

    }
}
