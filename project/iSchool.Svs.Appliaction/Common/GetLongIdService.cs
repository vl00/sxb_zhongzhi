using CSRedis;
using Dapper;
using iSchool.Infrastructure.UoW;
using iSchool.Svs.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace iSchool.Svs.Appliaction.Common
{
    public class GetLongIdService
    {
        SvsUnitOfWork svsUnitOfWork;
        CSRedisClient redis;

        public GetLongIdService(ISvsUnitOfWork svsUnitOfWork, CSRedisClient redis)
        {
            this.svsUnitOfWork = (SvsUnitOfWork)svsUnitOfWork;
            this.redis = redis;
        }

        /// <summary>
        /// 学校短id置换长id
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        public async Task<string> GetSchoolId(long no)
        {
            var id = await redis.GetAsync<string>(CacheKeys.SchoolNoToId.FormatWith(no));
            if (null == id)
            {
                var sql = $"SELECT id FROM dbo.School WHERE no = @no";
                id = svsUnitOfWork.DbConnection.QueryFirstOrDefault<Guid>(sql, new { no }).ToString();
                await redis.SetAsync(CacheKeys.SchoolNoToId.FormatWith(no), id, 60 * 60 * 24 * 1);
            }
            return id;
        }

        /// <summary>
        /// 专业短id置换长id
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        public async Task<string> GetMajorId(long no)
        {
            var id = await redis.GetAsync<string>(CacheKeys.MajorNoToId.FormatWith(no));
            if (null == id) {
                var sql = $"SELECT id FROM dbo.Major WHERE no = @no";
                id = svsUnitOfWork.DbConnection.QueryFirstOrDefault<Guid>(sql, new { no }).ToString();
                await redis.SetAsync(CacheKeys.MajorNoToId.FormatWith(no), id, 60 * 60 * 24 * 1);
            }
            return id;
        }

        /// <summary>
        /// 新闻短id置换长id
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        public async Task<string> GetNewsId(long no)
        {
            var id = await redis.GetAsync<string>(CacheKeys.NewsNoToId.FormatWith(no));
            if (null == id)
            {
                var sql = $"SELECT id FROM dbo.SchoolNews WHERE no = @no";
                id = svsUnitOfWork.DbConnection.QueryFirstOrDefault<Guid>(sql, new { no }).ToString();
                await redis.SetAsync(CacheKeys.NewsNoToId.FormatWith(no), id, 60 * 60 * 24 * 1);
            }
            return id;
        }
    }
}
