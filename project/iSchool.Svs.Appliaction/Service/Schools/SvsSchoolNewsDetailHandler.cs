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
    public class SvsSchoolNewsDetailHandler : IRequestHandler<SvsSchoolNewsDetailQuery, SvsSchoolNewsDetailResult>
    {
        SvsUnitOfWork svsUnitOfWork;
        CSRedisClient redis;
        GetLongIdService getLongId;

        public SvsSchoolNewsDetailHandler(ISvsUnitOfWork svsUnitOfWork, CSRedisClient redis)
        {
            this.svsUnitOfWork = (SvsUnitOfWork)svsUnitOfWork;
            this.redis = redis;
            this.getLongId = new GetLongIdService(this.svsUnitOfWork, this.redis);
        }

        /// <summary>
        /// 新闻详情
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public async Task<SvsSchoolNewsDetailResult> Handle(SvsSchoolNewsDetailQuery query, CancellationToken cancellation)
        {
            await Task.CompletedTask;
            //置换长Id
            var newsId = await getLongId.GetNewsId(query.No);
            var res = await redis.GetAsync<SvsSchoolNewsDetailResult>(CacheKeys.NewsDetail.FormatWith(newsId));
            if (null != res)
            {
                return res;
            }
            //学校新闻
            var sql = @"SELECT slns.id AS Id,slns.no AS Id_s,slns.title AS Title,slns.source AS Source,slns.content AS Content ,slns.count AS Count,sl.no AS SchoolId_s,sl.name AS SchoolName
                        FROM dbo.SchoolNews AS slns
                        LEFT JOIN dbo.School AS sl ON sl.id = slns.sid
                        WHERE slns.no = @No AND sl.IsValid = 1 AND slns.IsValid = 1";
            var news = svsUnitOfWork.DbConnection.QueryFirstOrDefault<SvsSchoolNewsDetailResult>(sql, new { query.No});
            await redis.SetAsync(CacheKeys.NewsDetail.FormatWith(newsId), news, 60 * 60 * 2 * 1);
            return news;
        }
    }
}
