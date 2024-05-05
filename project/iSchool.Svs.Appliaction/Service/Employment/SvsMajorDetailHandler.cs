using CSRedis;
using Dapper;
using iSchool.Infrastructure.UoW;
using iSchool.Svs.Appliaction.Common;
using iSchool.Svs.Appliaction.RequestModels.Category;
using iSchool.Svs.Appliaction.RequestModels.Employment;
using iSchool.Svs.Appliaction.ResponseModels;
using iSchool.Svs.Appliaction.ResponseModels.Employment;
using iSchool.Svs.Appliaction.ResponseModels.HotCategory;
using iSchool.Svs.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace iSchool.Svs.Appliaction.Service.Category
{
    public class SvsMajorDetailHandler : IRequestHandler<SvsProspectsDetailQuery, SvsMajorDetailResult>
    {
        SvsUnitOfWork svsUnitOfWork;
        CSRedisClient redis;
        GetLongIdService getLongId;

        public SvsMajorDetailHandler(ISvsUnitOfWork svsUnitOfWork, CSRedisClient redis)
        {
            this.svsUnitOfWork = (SvsUnitOfWork)svsUnitOfWork;
            this.redis = redis;
            this.getLongId = new GetLongIdService(this.svsUnitOfWork, this.redis);
        }

        /// <summary>
        /// 专业详情
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public async Task<SvsMajorDetailResult> Handle(SvsProspectsDetailQuery query, CancellationToken cancellation)
        {
            await Task.CompletedTask;
            //置换长id
            var majorId = await getLongId.GetMajorId(query.MajorId); 
            var schoolId = await getLongId.GetSchoolId(query.SchoolId);
            //读取缓存
            var redisData  = await redis.GetAsync<SvsMajorDetailResult>(CacheKeys.MajorDetail.FormatWith(majorId, schoolId, query.Province));
            if (redisData != null)
            {
                return redisData;
            }
            //专业详情
            SvsMajorDetailResult majorDetail;
            if (query.SchoolId == 0)
            {
                majorDetail = MajorDetail(query.MajorId);
            }
            else
            {
                majorDetail = MajorDetail(query.MajorId, query.SchoolId);
            }
            //推荐学校
            var schools = SvsSchool(query.MajorId, query.Province);
            majorDetail = majorDetail == null ? new SvsMajorDetailResult() : majorDetail;
            majorDetail.Schools = schools;
            //三级专业则置换成二级专业
            var mid = MajorCheck(query.MajorId);
            if (0 != mid)
            {
                //获取专业信息
                var secondLevel = MajorInfo(mid);
                majorDetail.Code = secondLevel.Code;
                majorDetail.Salary = secondLevel.Salary;
                majorDetail.MajorCelsius = secondLevel.MajorCelsius;
                majorDetail.Intro = secondLevel.Intro;
                majorDetail.Logo = secondLevel.Logo;
                majorDetail.SchoolNumber = secondLevel.SchoolNumber;
                majorDetail.Count = secondLevel.Count;
                majorDetail.TargetName = secondLevel.TargetName;
                majorDetail.EduLevel = secondLevel.EduLevel;
                majorDetail.SchoolSystem = secondLevel.SchoolSystem;
                majorDetail.Tuition = secondLevel.Tuition;
                majorDetail.Prospects = secondLevel.Prospects;
            }
            await redis.SetAsync(CacheKeys.MajorDetail.FormatWith(majorId, schoolId, query.Province), majorDetail, 60 * 60 * 2 * 1);
            return majorDetail;
        }

        /// <summary>
        /// 三级专业则置换成二级专业
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        public long MajorCheck(long no)
        {
            var sql = "SELECT no FROM dbo.Major WHERE id = (SELECT pid FROM dbo.Major WHERE level = 3 AND no = @no)";
            var id = svsUnitOfWork.DbConnection.QueryFirstOrDefault<long>(sql, new { no });
            return id;
        }

        /// <summary>
        /// 专业详情-无学校id
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        public SvsMajorDetailResult MajorDetail(long no)
        {
            var sql = @"SELECT slmj.id AS Id,slmj.no AS Id_s, slmj.name AS Name,CASE WHEN slmj.code IS NULL THEN '' ELSE slmj.code END  AS Code
                        ,(CASE WHEN slmj.minsalary IS NULL THEN '' ELSE CAST(slmj.minsalary+'-' AS varchar(20)) END +CASE WHEN slmj.maxsalary IS NULL THEN '' ELSE CAST(slmj.maxsalary AS varchar(20)) END) AS  Salary
                        ,(CASE WHEN slmj.viewcount IS NULL THEN '' ELSE CAST(slmj.viewcount AS varchar(20)) END) AS MajorCelsius 
                        ,(SELECT COUNT(1) FROM dbo.SchoolMajor WHERE majorid = slmj.id)AS SchoolNumber,slmj.intro AS Intro,slmj.logo AS Logo,REPLACE('mas'+CAST(slmj.no AS CHAR),' ','') AS MajorNo
                        FROM dbo.Major AS slmj WHERE no = @no AND slmj.IsValid = 1";
            var data = svsUnitOfWork.DbConnection.QueryFirstOrDefault<SvsMajorDetailResult>(sql, new { no });
            return data;
        }

        /// <summary>
        /// 专业详情-有学校id
        /// </summary>
        /// <param name="majorNo"></param>
        /// <param name="schoolNo"></param>
        /// <returns></returns>
        public SvsMajorDetailResult MajorDetail(long majorNo,long schoolNo)
        {
            var sql = @"SELECT SL.no,mj.id AS  Id,mj.no AS Id_s,(sl.name+'-'+mj.name) AS Name,slmj.count AS Count,tt.name AS TargetName,slmj.edulevel AS EduLevel,slmj.systemdesc AS SchoolSystem,slmj.tuition AS Tuition
            ,mj.intro,mj.prospects AS Prospects
            ,(CASE WHEN mj.minsalary IS NULL THEN '' ELSE CAST(mj.minsalary +'-' AS varchar(10)) END+CASE WHEN mj.maxsalary IS NULL THEN '' ELSE CAST(mj.maxsalary AS varchar(10)) END) AS  Salary,mj.logo AS Logo,REPLACE('mas'+CAST(mj.no AS CHAR),' ','') AS MajorNo
            FROM dbo.SchoolMajor AS slmj
            LEFT JOIN dbo.School AS sl ON sl.id = slmj.sid
            LEFT JOIN dbo.Major AS mj ON mj.id = slmj.majorid
            LEFT JOIN dbo.Target AS tt ON tt.id = slmj.targetid
            WHERE sl.no = @schoolNo AND mj.no = @majorNo AND sl.IsValid =1 AND mj.IsValid = 1";
            var data = svsUnitOfWork.DbConnection.QueryFirstOrDefault<SvsMajorDetailResult>(sql, new { majorNo, schoolNo });
            return data;
        }

        /// <summary>
        /// 获取专业信息
        /// </summary>
        /// <param name="no"></param>
        public MajorDetailResult MajorInfo(long no)
        {
            var sql = @"SELECT mj.code AS Code,(CASE WHEN mj.minsalary IS NULL THEN '' ELSE CAST(mj.minsalary+'-' AS varchar(20)) END +CASE WHEN mj.maxsalary IS NULL THEN '' ELSE CAST(mj.maxsalary AS varchar(20)) END) AS  Salary
            ,(CASE WHEN mj.viewcount IS NULL THEN '' ELSE CAST(mj.viewcount AS varchar(20)) END) AS MajorCelsius,slmj.count AS Count 
            ,mj.intro AS Intro,mj.logo AS Logo,(SELECT COUNT(1) FROM dbo.SchoolMajor WHERE majorid = mj.id)AS SchoolNumber,tt.name AS TargetName,slmj.edulevel AS EduLevel,slmj.systemdesc AS SchoolSystem,slmj.tuition AS Tuition
            ,mj.prospects AS Prospects
            FROM dbo.Major AS mj 
            LEFT JOIN dbo.SchoolMajor AS slmj ON slmj.majorid = mj.id
            LEFT JOIN dbo.Target AS tt ON tt.id = slmj.targetid
            WHERE mj.no = @no AND mj.IsValid =1 ";
            var data = svsUnitOfWork.DbConnection.QueryFirstOrDefault<MajorDetailResult>(sql, new { no });
            return data;
        }

        /// <summary>
        /// 开设学校
        /// </summary>
        /// <param name="no"></param>
        /// <param name="province"></param>
        public IEnumerable<HotSchoolResult> SvsSchool(long no,int province)
        {
            var sql = @"SELECT TOP 10 sl.no AS Id_s, sl.id AS Id,sl.name AS Name,sl.tel AS Tel,
                    (SELECT TOP 2 name+' ' FROM dbo.Major AS mj 
                    LEFT JOIN dbo.SchoolMajor AS slmjx ON mj.id = slmjx.majorid 
                    WHERE slmjx.sid = slmj.sid ORDER BY NEWID() FOR XML PATH('')) AS HotMajors
                    ,sl.address AS Address,sl.nature AS NatureId,sl.level AS LevelId,sl.logo AS Logo
                    FROM dbo.School AS sl
                    LEFT JOIN dbo.SchoolMajor AS slmj ON slmj.sid = sl.id
                    LEFT JOIN dbo.Major AS mj ON mj.id = slmj.majorid
                    LEFT JOIN dbo.MajorCategory AS mjcy ON mjcy.majorid = slmj.majorid AND mjcy.IsValid = 1
                    WHERE mj.no=@no AND sl.IsValid = 1 AND mj.IsValid = 1";
            var sqlWhere = "";
            if (province != 0)
            {
                sqlWhere += " AND sl.province = @province";
            }
            var sqlOrder = " GROUP BY sl.no,sl.id,sl.name,sl.address,slmj.sid,sl.nature,sl.level,sl.tel,sl.logo; ";
            var list = svsUnitOfWork.DbConnection.Query<HotSchoolResult>(sql+ sqlWhere+ sqlOrder, new { no, province });
            return list;
        }
    }
}
