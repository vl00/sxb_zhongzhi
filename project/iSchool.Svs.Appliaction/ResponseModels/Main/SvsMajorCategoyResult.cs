using iSchool.Svs.Appliaction.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace iSchool.Svs.Appliaction.ResponseModels
{
#nullable enable

    /// <summary>
    /// 首页中职专业分类目录result
    /// </summary>
    public class SvsMajorCategoyResult
    {
        /// <summary>页面顶部切换地区--选中的省份</summary>
        public AreaNameCodeDto? SelectedProvince { get; set; }
        /// <summary>页面顶部切换地区--所有省</summary>
        public IEnumerable<AreaNameCodeDto>? Provinces { get; set; }

        /// <summary>中职专业分类目录</summary>
        public IEnumerable<CgyItemListDto<IdNameDto<string>>> Categoies { get; set; } = default!;
    }

#nullable disable
}
