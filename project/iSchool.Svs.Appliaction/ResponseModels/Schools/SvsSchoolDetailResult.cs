using iSchool.Svs.Appliaction.ResponseModels.HotCategory;
using iSchool.Svs.Appliaction.ResponseModels.Schools;
using System.Collections.Generic;

namespace iSchool.Svs.Appliaction.ResponseModels.School
{
    /// <summary>
    /// 学校详情
    /// </summary>
    public class SvsSchoolDetailResult
    {
        /// <summary>
        /// 学校概况
        /// </summary>
        public SketchedResult Sketched { get; set; } 

        /// <summary>
        /// 热门专业列表
        /// </summary>
        public IEnumerable<MajorDetailResult> HotMajor { get; set; }

        /// <summary>
        /// 学校新闻列表
        /// </summary>
        public IEnumerable<SvsSchoolNewsResult> News { get; set; } 

    }
}
