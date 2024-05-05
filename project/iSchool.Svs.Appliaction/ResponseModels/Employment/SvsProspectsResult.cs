using iSchool.Infrastructure.Dapper;
using iSchool.Svs.Appliaction.ResponseModels.HotCategory;
using System;
using System.Collections.Generic;
using System.Text;

namespace iSchool.Svs.Appliaction.ResponseModels.Employment
{
    /// <summary>
    /// 就业前景
    /// </summary>
    public class SvsProspectsResult
    {
        /// <summary>
        /// 就业前景专业分页数据
        /// </summary>
        public PagedList<MajorDetailResult> HotMajorsPageInfo { get; set; } = default!;

    }
}
