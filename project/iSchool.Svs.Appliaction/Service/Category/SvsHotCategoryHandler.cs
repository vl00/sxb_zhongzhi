using CSRedis;
using Dapper;
using iSchool.Infrastructure.UoW;
using iSchool.Svs.Appliaction.RequestModels.Category;
using iSchool.Svs.Appliaction.ResponseModels;
using iSchool.Svs.Appliaction.ResponseModels.HotCategory;
using iSchool.Svs.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace iSchool.Svs.Appliaction.Service.Category
{
    public class SvsHotCategoryHandler : IRequestHandler<SvsHotCategoryQuery, HotCategoryResult>
    {
        SvsUnitOfWork svsUnitOfWork;
        CSRedisClient redis;

        public SvsHotCategoryHandler(ISvsUnitOfWork svsUnitOfWork,CSRedisClient redis)
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
        public async Task<HotCategoryResult> Handle(SvsHotCategoryQuery query, CancellationToken cancellation)
        {
            await Task.CompletedTask;
            //热门分类  MajorCategoryResult
            var categories = await redis.GetAsync<MajorCategoryResult[]>(CacheKeys.HotCategoy);
            if (null == categories) 
            {
                categories = MajorCategory().AsArray();
                await redis.SetAsync(CacheKeys.HotCategoy, categories, 60 * 60 * 2 * 1);
            }
            var categoryid ="";
            if (string.IsNullOrEmpty(query.Id) && categories != null && categories?.AsList()?.Count > 0){
                categoryid = categories.AsList()[0].Id.ToString();
            }else {
                categoryid = ((IEnumerable<MajorCategoryResult>)categories).Where(w => w.Id_s == query.Id).FirstOrDefault().Id.ToString();
            }
            //热门专业
            var majoes = await redis.GetAsync<MajorDetailResult[]>(CacheKeys.HotCategoyMajor.FormatWith(categoryid));
            if (null == majoes)
            {
                majoes = HotMajorCategory(categoryid).AsArray();
                await redis.SetAsync(CacheKeys.HotCategoyMajor.FormatWith(categoryid), majoes, 60 * 60 * 2 * 1);
            }
            //推荐学校
            var schools = await redis.GetAsync<HotSchoolResult[]>(CacheKeys.HotCategoySchool.FormatWith(categoryid, query.Province));
            if (null == schools)
            {
                schools = SvsSchool(categoryid, query.Province).AsArray();
                await redis.SetAsync(CacheKeys.HotCategoySchool.FormatWith(categoryid, query.Province), schools, 60 * 60 * 2 * 1);
            }
            var res = new HotCategoryResult
            {
                Category = categories,
                HotMajor = majoes,
                HotSchool = schools
            };
            return res;
        }

        /// <summary>
        /// 热门分类
        /// </summary>
        public IEnumerable<MajorCategoryResult> MajorCategory()
        {
            var sql = "";
            sql = @"SELECT id AS Id,no AS Id_s,name AS Name FROM dbo.Category WHERE IsValid = 1 AND type = 1 ORDER BY sort ASC;";
            var categories = svsUnitOfWork.DbConnection.Query<MajorCategoryResult>(sql, new { });
            return categories;
        }

        /// <summary>
        /// 热门专业
        /// </summary>
        /// <param name="id">专业id</param>
        public IEnumerable<MajorDetailResult> HotMajorCategory(string id)
        {
            var sql = @"SELECT TOP 10 mj.id AS Id,mj.no AS Id_s,mj.name AS Name,mj.intro AS Intro,mj.logo AS Logo,mj.prospects AS Prospects
                        FROM dbo.MajorCategory AS mjcy
                        LEFT JOIN dbo.Major AS mj ON mj.id = mjcy.majorid
                        WHERE mjcy.categoryid = @categoryid AND mj.IsValid = 1 AND mjcy.type = 0 AND mjcy.IsValid = 1
                        ORDER BY mjcy.sort ASC";
            var categoryid = id;
            var list = svsUnitOfWork.DbConnection.Query<MajorDetailResult>(sql, new { categoryid });
            return list;
        }

        /// <summary>
        /// 推荐学校
        /// </summary>
        /// <param name="id">专业id</param>
        /// <param name="province">区域</param>
        public IEnumerable<HotSchoolResult> SvsSchool(string id, int province)
        {
            var sql = @"SELECT TOP 10 sl.no AS Id_s, sl.id AS Id,sl.name AS Name,
                        (SELECT TOP 2 mjh.name+' ' FROM dbo.Major AS mjh 
                        LEFT JOIN dbo.SchoolMajor AS slmjh ON mjh.id = slmjh.majorid 
                        WHERE slmjh.sid = slmj.sid GROUP BY mjh.name,mjh.viewcount ORDER BY mjh.viewcount DESC FOR XML PATH('')) AS HotMajors
                        ,sl.address AS Address,sl.nature AS NatureId,sl.level AS LevelId,sl.logo AS Logo
                        FROM dbo.School AS sl
                        LEFT JOIN dbo.SchoolMajor AS slmj ON slmj.sid = sl.id
                        LEFT JOIN dbo.Major AS mj ON mj.id = slmj.majorid
                        LEFT JOIN dbo.MajorCategory AS mjcy ON mjcy.majorid = slmj.majorid AND mjcy.IsValid = 1
                        WHERE mjcy.categoryid=@categoryid AND sl.IsValid = 1";
            var sqlWhere = "";
            if (province != 0)
            {
                sqlWhere += " AND sl.province = @province";
            }
            var sqlOrder = " GROUP BY sl.no,sl.id,sl.name,sl.address,slmj.sid,sl.nature,sl.level,sl.logo;";
            var categoryid = id;
            var list = svsUnitOfWork.DbConnection.Query<HotSchoolResult>(sql+ sqlWhere+ sqlOrder, new { categoryid, province });
            return list;
        }
    }
}
