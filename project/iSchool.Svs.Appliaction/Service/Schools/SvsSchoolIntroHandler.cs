using CSRedis;
using Dapper;
using iSchool.Infrastructure.UoW;
using iSchool.Svs.Appliaction.Common;
using iSchool.Svs.Appliaction.RequestModels.School;
using iSchool.Svs.Appliaction.ResponseModels.School;
using iSchool.Svs.Appliaction.ResponseModels.Schools;
using iSchool.Svs.Domain;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace iSchool.Svs.Appliaction.Service.School
{
    public class SvsSchoolIntroHandler : IRequestHandler<SvsSchoolIntroQuery, SketchedResult>
    {
        SvsUnitOfWork svsUnitOfWork;
        CSRedisClient redis;
        GetLongIdService getLongId;

        public SvsSchoolIntroHandler(ISvsUnitOfWork svsUnitOfWork, CSRedisClient redis)
        {
            this.svsUnitOfWork = (SvsUnitOfWork)svsUnitOfWork;
            this.redis = redis;
            this.getLongId = new GetLongIdService(this.svsUnitOfWork, this.redis);
        }

        /// <summary>
        /// 学校详情简介
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public async Task<SketchedResult> Handle(SvsSchoolIntroQuery query, CancellationToken cancellation)
        {
            await Task.CompletedTask;
            //置换长Id
            var schoolId = await getLongId.GetSchoolId(query.No);
            var res = await redis.GetAsync<SketchedResult>(CacheKeys.SchoolIntro.FormatWith(schoolId));
            if (null != res)
            {
                return res;
            }
            //学校详情简介
            var sql = @"SELECT sl.id AS Id,sl.no AS Id_s,sl.name AS Name,sl.intro AS Intro FROM dbo.School AS sl WHERE sl.no = @no AND sl.IsValid = 1";
            var sketched = svsUnitOfWork.DbConnection.QueryFirstOrDefault<SketchedResult>(sql, new { query.No });
            await redis.SetAsync(CacheKeys.SchoolIntro.FormatWith(schoolId), sketched, 60 * 60 * 2 * 1);
            return sketched;
        }
    }
}
