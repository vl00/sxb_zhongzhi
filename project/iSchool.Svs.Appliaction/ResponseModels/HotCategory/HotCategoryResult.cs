using System;
using System.Collections.Generic;
using System.Text;

namespace iSchool.Svs.Appliaction.ResponseModels.HotCategory
{
    /// <summary>
    /// 热门分类
    /// </summary>
    public class HotCategoryResult
    {
        /// <summary>
        /// 热门分类目录
        /// </summary>
        public IEnumerable<MajorCategoryResult> Category { get; set; }

        /// <summary>
        /// 热门专业
        /// </summary>
        public IEnumerable<MajorDetailResult> HotMajor { get; set; }

        /// <summary>
        /// 热门学校
        /// </summary>
        public IEnumerable<HotSchoolResult> HotSchool { get; set; }
    }
}
