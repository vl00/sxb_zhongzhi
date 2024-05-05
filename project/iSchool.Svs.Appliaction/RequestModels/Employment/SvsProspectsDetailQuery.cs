using iSchool.Svs.Appliaction.ResponseModels.Employment;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace iSchool.Svs.Appliaction.RequestModels.Employment
{
    /// <summary>
    /// 就业前景详情
    /// </summary>
    public class SvsProspectsDetailQuery : IRequest<SvsMajorDetailResult>
    {
        /// <summary>
        /// 专业id
        /// </summary>
        public long MajorId { get; set; }

        /// <summary>
        /// 学校id
        /// </summary>
        public long SchoolId { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
        public int Province { get; set; }
    }
}
