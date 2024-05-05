using iSchool.Svs.Appliaction.ResponseModels.HotCategory;
using MediatR;
using System.Collections.Generic;

namespace iSchool.Svs.Appliaction.RequestModels.Category
{
    public class SvsHotCategoryQuery : IRequest<HotCategoryResult>
    {
        /// <summary>
        /// 分类id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        public int Province { get; set; }
    }
}
