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
    public class SvsSchoolNewsHandler : IRequestHandler<SvsSchoolNewsQuery, IEnumerable<SvsSchoolNewsResult>>
    {
        SvsUnitOfWork svsUnitOfWork;
        CSRedisClient redis;
        GetLongIdService getLongId;

        public SvsSchoolNewsHandler(ISvsUnitOfWork svsUnitOfWork, CSRedisClient redis)
        {
            this.svsUnitOfWork = (SvsUnitOfWork)svsUnitOfWork;
            this.redis = redis;
            this.getLongId = new GetLongIdService(this.svsUnitOfWork, this.redis);
        }
        /// <summary>
        /// 学校详情新闻列表
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SvsSchoolNewsResult>> Handle(SvsSchoolNewsQuery query, CancellationToken cancellation)
        {
            await Task.CompletedTask;
            //置换长Id
            var schoolId = await getLongId.GetSchoolId(query.No);
            var newsList = query.PageIndex != 1 ? null : await redis.GetAsync<SvsSchoolNewsResult[]>(CacheKeys.NewsList.FormatWith(schoolId));
            if (null != newsList) {
                return newsList;
            }
            //学校新闻
            var sql = @"SELECT slns.sid AS Id,slns.no AS Id_s,slns.title AS Title,slns.ModifyDateTime 
                    FROM dbo.SchoolNews AS slns
                    LEFT JOIN dbo.School AS sl ON sl.id = slns.sid
                    WHERE sl.no = @no AND slns.IsValid = 1 ORDER BY slns.CreateTime DESC,slns.id DESC OFFSET @PageIndex ROWS FETCH NEXT @PageSize ROWS ONLY ";
            var PageIndex = query.PageSize * (query.PageIndex - 1);
            var PageSize = query.PageSize;
            newsList = svsUnitOfWork.DbConnection.Query<SvsSchoolNewsResult>(sql, new { query.No, PageIndex, PageSize }).AsArray();
            if (query.PageIndex == 1) { await redis.SetAsync(CacheKeys.NewsList.FormatWith(schoolId), newsList, 60 * 60 * 2 * 1); }
            return newsList;
        }
    }
}
