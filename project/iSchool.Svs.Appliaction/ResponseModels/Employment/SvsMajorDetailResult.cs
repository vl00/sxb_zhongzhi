using iSchool.Svs.Appliaction.ResponseModels.HotCategory;
using System;
using System.Collections.Generic;
using System.Text;

namespace iSchool.Svs.Appliaction.ResponseModels.Employment
{
    /// <summary>
    /// 中职专业详情
    /// </summary>
    public class SvsMajorDetailResult: MajorDetailResult
    {
        /// <summary>
        /// 开设的学校
        /// </summary>
        public IEnumerable<HotSchoolResult> Schools { get; set; }
    }
}
