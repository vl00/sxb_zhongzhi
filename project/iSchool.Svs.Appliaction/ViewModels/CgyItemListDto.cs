using System;
using System.Collections.Generic;
using System.Text;

namespace iSchool.Svs.Appliaction.ViewModels
{
#nullable enable

    /// <summary>
    /// 分类项列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CgyItemListDto<T>
    {        
        /// <summary>id/code/type, 通常用于传数据到后端</summary>
        public string? Id { get; set; }
        /// <summary>用于显示名称</summary>
        public string? Name { get; set; }
        /// <summary>列表</summary>
        public IEnumerable<T> List { get; set; } = default!;
    }

#nullable disable
}
