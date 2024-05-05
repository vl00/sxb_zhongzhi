using System;
using System.Collections.Generic;
using System.Text;

namespace iSchool.Svs.Domain
{
    public static class CacheKeys
    {
        //cache rule：1.系统   2.模块   3.参数名或者功能模块   4.附带参数
        //eg. org:course:courseid:{0}  
        //eg. org:course:index:sore:{0}:page:{1}
        //eg. org:eval:total
        //eg. org:eval:index:page:{1}
        

        /// <summary>地区省市</summary>
        public const string Area = "svs:areas:item_{0}";
        /// <summary>专业</summary>
        public const string Ma = "svs:ma:item_{0}";

        /// <summary>首页中职专业目录 </summary>
        public const string MajorCategoy = "svs:main:major_categoy";
        /// <summary>首页推荐学校 </summary>
        public const string MainRecommendSchools = "svs:main:recommend_schools:pr{0}";   
        /// <summary>首页院校库 </summary>
        public const string MainSchoolResps = "svs:main:schools:pr_{0}";
        /// <summary>首页热门分类 </summary>
        public const string MainHots = "svs:main:hots";

        /// <summary>学校最热门的2个专业</summary>
        public const string SchoolTop2HotMajors = "svs:schooltop2hotmajors:id_{0}";

        /// <summary>
        /// 热门分类
        /// </summary>
        public const string HotCategoy = "svs:hot_categoy";

        /// <summary>
        /// 热门分类热门专业
        /// </summary>
        public const string HotCategoyMajor = "svs:hot_categoy_major:cgyid_{0}";

        /// <summary>
        /// 热门分类推荐学校
        /// </summary>
        public const string HotCategoySchool = "svs:hot_categoy_school:cgyid_{0}_pr_{1}";

        /// <summary>
        /// 专业详情
        /// </summary>
        public const string MajorDetail = "svs:major_detail:mjid_{0}_sid_{1}_pr_{2}";

        /// <summary>
        /// 学校短id置换长id
        /// </summary>
        public const string SchoolNoToId = "svs:scl_no_to_id:no_{0}";

        /// <summary>
        /// 专业短id置换长id
        /// </summary>
        public const string MajorNoToId = "svs:mj_no_to_id:no_{0}";

        /// <summary>
        /// 新闻短id置换长id
        /// </summary>
        public const string NewsNoToId = "svs:ns_no_to_id:no_{0}";

        /// <summary>
        /// 专业就业前景
        /// </summary>
        public const string MajorProspects = "svs:mj_prospects";

        /// <summary>
        /// 学校详情
        /// </summary>
        public const string SchoolDetail = "svs:scl_detail:sid_{0}";

        /// <summary>
        /// 学校简介
        /// </summary>
        public const string SchoolIntro = "svs:scl_intro:sid_{0}";

        /// <summary>
        /// 新闻详情
        /// </summary>
        public const string NewsDetail = "svs:news_detail:nsid_{0}";

        /// <summary>
        /// 新闻列表
        /// </summary>
        public const string NewsList = "svs:news_list:sid_{0}";

    }
}
