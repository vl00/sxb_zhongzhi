using CSRedis;
using Dapper;
using iSchool.Infrastructure.Dapper;
using iSchool.Infrastructure.UoW;
using iSchool.Svs.Appliaction.RequestModels.Category;
using iSchool.Svs.Appliaction.RequestModels.Employment;
using iSchool.Svs.Appliaction.ResponseModels;
using iSchool.Svs.Appliaction.ResponseModels.Employment;
using iSchool.Svs.Appliaction.ResponseModels.HotCategory;
using iSchool.Svs.Domain;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace iSchool.Svs.Appliaction.Service.Category
{
    public class SvsProspectsHandler : IRequestHandler<SvsPageQuery, SvsProspectsResult>
    {
        SvsUnitOfWork svsUnitOfWork;
        CSRedisClient redis;

        public SvsProspectsHandler(ISvsUnitOfWork svsUnitOfWork, CSRedisClient redis)
        {
            this.svsUnitOfWork = (SvsUnitOfWork)svsUnitOfWork;
            this.redis = redis;
        }

        /// <summary>
        /// 热门分类
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public async Task<SvsProspectsResult> Handle(SvsPageQuery query, CancellationToken cancellation)
        {
            await Task.CompletedTask;
            var majoes = query.PageIndex != 1 ? null : await redis.GetAsync<PagedList<MajorDetailResult>>(CacheKeys.MajorProspects);
            //热门专业
            if (null == majoes)
            {
                majoes = HotMajorCategory(query);
                if (query.PageIndex == 1) { await redis.SetAsync(CacheKeys.MajorProspects, majoes, 60 * 60 * 2 * 1); }
            }
            var res = new SvsProspectsResult
            {
                HotMajorsPageInfo = majoes
            };
            return res;
        }

        /// <summary>
        /// 专业就业前景
        /// </summary>
        /// <param name="query"></param>
        public PagedList<MajorDetailResult> HotMajorCategory(SvsPageQuery query)
        {
            var sql = @"SELECT mj.id AS Id,mj.no AS Id_s,mj.name AS Name,mj.intro AS Intro,mj.logo AS Logo, mj.prospects AS Prospects FROM dbo.Major AS mj WHERE level = 2 AND mj.IsValid = 1 ORDER BY mj.viewcount DESC OFFSET @PageIndex ROWS FETCH NEXT @PageSize ROWS ONLY";
            var PageIndex = query.PageSize * (query.PageIndex - 1);
            var list = svsUnitOfWork.DbConnection.Query<MajorDetailResult>(sql, new { PageIndex, query.PageSize });
            var totalSql = "SELECT COUNT(1) FROM dbo.Major AS mj WHERE level = 2 AND mj.IsValid = 1";
            var total = svsUnitOfWork.DbConnection.QueryFirstOrDefault<int>(totalSql, new { });
            var ls = new PagedList<MajorDetailResult>
            {
                CurrentPageItems = list,
                CurrentPageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TotalItemCount = total,
            };
            return ls;
        }
    }
}
