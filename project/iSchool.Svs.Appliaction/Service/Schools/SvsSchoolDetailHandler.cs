using CSRedis;
using Dapper;
using iSchool.Infrastructure.UoW;
using iSchool.Svs.Appliaction.Common;
using iSchool.Svs.Appliaction.RequestModels.School;
using iSchool.Svs.Appliaction.ResponseModels.HotCategory;
using iSchool.Svs.Appliaction.ResponseModels.School;
using iSchool.Svs.Appliaction.ResponseModels.Schools;
using iSchool.Svs.Domain;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace iSchool.Svs.Appliaction.Service.School
{
    public class SvsSchoolDetailHandler : IRequestHandler<SvsSchoolDetailQueryDto, SvsSchoolDetailResult>
    {
        SvsUnitOfWork svsUnitOfWork;
        CSRedisClient redis;
        GetLongIdService getLongId;

        public SvsSchoolDetailHandler(ISvsUnitOfWork svsUnitOfWork, CSRedisClient redis)
        {
            this.svsUnitOfWork = (SvsUnitOfWork)svsUnitOfWork;
            this.redis = redis;
            this.getLongId = new GetLongIdService(this.svsUnitOfWork, this.redis);
        }

        /// <summary>
        /// 学校详情
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public async Task<SvsSchoolDetailResult> Handle(SvsSchoolDetailQueryDto query, CancellationToken cancellation)
        {
            await Task.CompletedTask;
            //置换长Id
            var schoolId = await getLongId.GetSchoolId(query.No);
            var res = await redis.GetAsync<SvsSchoolDetailResult>(CacheKeys.SchoolDetail.FormatWith(schoolId));
            if (res != null) {
                return res;
            }
            //学校概况
            var sql = @"SELECT sl.id AS Id,sl.no AS Id_s,sl.name AS Name,sl.type AS TypeId,'' AS Tel,sl.level AS LevelId,sl.nature AS NatureId,sl.address AS Address,sl.logo
                        FROM dbo.School AS sl WHERE sl.no = @no AND sl.IsValid = 1";
            var sketched = svsUnitOfWork.DbConnection.QueryFirstOrDefault<SketchedResult>(sql, new { query.No });
            //热门专业
            sql = @"SELECT  mj.id AS Id,mj.no AS Id_s,mj.name AS Name,tg.name AS TargetName,slmj.systemdesc AS SchoolSystem,slmj.edulevel AS EduLevel,slmj.tuition AS Tuition,mj.prospects AS Prospects
                    FROM dbo.SchoolMajor AS slmj
                    LEFT JOIN dbo.Major AS mj ON mj.id = slmj.majorid
                    LEFT JOIN dbo.Target AS tg ON tg.id = slmj.targetid
                    LEFT JOIN dbo.School AS sl ON sl.id = slmj.sid
                    WHERE sl.no = @no AND mj.IsValid = 1";
            var hotMajorList = svsUnitOfWork.DbConnection.Query<MajorDetailResult>(sql, new { query.No });
            //学校新闻
            sql = @"SELECT TOP 5 slns.id AS Id,slns.no AS Id_s,slns.title AS Title,slns.ModifyDateTime 
                    FROM dbo.SchoolNews AS slns
                    LEFT JOIN dbo.School AS sl ON sl.id = slns.sid
                    WHERE sl.no = @no AND slns.IsValid = 1";
            var newsList = svsUnitOfWork.DbConnection.Query<SvsSchoolNewsResult>(sql, new { query.No });
            res = new SvsSchoolDetailResult
            {
                Sketched = sketched,
                HotMajor = hotMajorList,
                News = newsList
            };
            await redis.SetAsync(CacheKeys.SchoolDetail.FormatWith(sketched.Id), res, 60 * 60 * 2 * 1);
            return res;
        }
    }
}
