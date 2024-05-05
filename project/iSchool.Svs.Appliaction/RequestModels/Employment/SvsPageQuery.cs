using iSchool.Svs.Appliaction.ResponseModels.Employment;
using iSchool.Svs.Appliaction.ResponseModels.HotCategory;
using MediatR;
using System.Collections.Generic;

namespace iSchool.Svs.Appliaction.RequestModels.Employment
{
    /// <summary>
    /// 就业前景
    /// </summary>
    public class SvsPageQuery : IRequest<SvsProspectsResult>
    {
        /// <summary>
        /// 第几页.不传默认为1
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 页大小.不传默认为10
        /// </summary>
        public int PageSize { get; set; } = 10;
    }
}
