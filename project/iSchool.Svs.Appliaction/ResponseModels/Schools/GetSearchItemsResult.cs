using iSchool.Svs.Appliaction.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace iSchool.Svs.Appliaction.ResponseModels
{
#nullable enable

    /**
           {
              "id": "string",
              "name": "string",
              "pid": "string",
              "list": [
                {
                  "id": "string",
                  "name": "string",
                  "pid": "string"
                }
              ]
            }
     */

    /// <summary>
    /// 搜索项s. 根据查询的类型,里面的字段都可为null
    /// </summary>
    public class GetSearchItemsResult
    {
        /// <summary>省市帅选</summary>
        public IEnumerable<TreeItemDto<TreeItemDto>>? Prci { get; set; }

        /// <summary>专业分类/专业</summary>
        public IEnumerable<TreeItemDto<TreeItemDto>>? Ma { get; set; }

        /// <summary>学校类型</summary>
        public IEnumerable<IdNameDto<string>>? Ty { get; set; }
        /// <summary>学校性质</summary>
        public IEnumerable<IdNameDto<string>>? Na { get; set; }
        /// <summary>学校等级</summary>
        public IEnumerable<IdNameDto<string>>? Lv { get; set; }
    }

#nullable disable
}
