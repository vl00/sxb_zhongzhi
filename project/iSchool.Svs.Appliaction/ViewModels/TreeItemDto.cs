using iSchool;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace iSchool.Svs.Appliaction.ViewModels
{
#nullable enable

    public class TreeItemDto
    {
        /// <summary>id/code/type, 通常用于传数据到后端</summary>
        [JsonProperty("id")]
        public string Id { get; set; } = default!;
        /// <summary>用于显示名称</summary>
        [JsonProperty("name")]
        public string Name { get; set; } = default!;
        /// <summary>父id. 可null</summary>
        [JsonProperty("pid")]
        public string? Pid { get; set; }

        /// <summary>
        /// (通常1层)扩展字段<br/>
        /// int? depth; //深度. <br/>
        /// string? logo; //图标/图片. <br/>
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JToken> _Ext1 { get; set; } = new DictionarySafeGet<string, JToken>(new StringToLowerEqualityComparer());
    }

    public class TreeItemDto<T> : TreeItemDto
    {
        /// <summary>列表. 可null</summary>
        [JsonProperty("list", Order = 4)]
        public IEnumerable<T>? List { get; set; }
    }

#nullable disable
}
