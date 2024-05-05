using iSchool.Svs.Appliaction.ResponseModels;
using MediatR;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace iSchool.Svs.Appliaction.RequestModels
{
#nullable enable

    /**
            {
              "prci": { selected:['pr110000'], selectedDirection: 0 },
              "ma": { selected:['801'], selectedDirection: 1 },
            }
     */

    /// <summary>
    /// 字典 ` { "key": { } } ` <br/>
    /// key = search项s类型.字段值为返回结果的字段名.参考文档返回结果.           
    /// </summary>
    public class GetSearchItemsQuery : Dictionary<string, GetOneSearchTypeItemsQuery>, IRequest<GetSearchItemsResult>
    {
    }

    public class GetOneSearchTypeItemsQuery
    {
        /// <summary>
        /// 选中的search项s.可null
        /// </summary>
        public string[]? Selected { get; set; }
        /// <summary>
        /// 可null. Selected字段包含一个能多级item时起效.<br/>
        /// 0和null = 选中项所有每个上级路径中同层所有项+选中项的下一级.<br/>
        /// 1 = 同类型每个选中的search项的下一级所有项(仅仅下一级)
        /// </summary>
        public int? SelectedDirection { get; set; }
    }

#nullable disable
}

namespace iSchool.Svs.Appliaction.RequestModels
{
    public class GetSearchItemsQuery1 //: IRequest<GetSearchItemsResult>
    {
        /// <summary>
        /// 省.<br/>
        /// 格式1 ` [ [], 0 ] ` .<br/>
        /// 数组arr不能为null,可空数组.(为null不会查询该类型)<br/>
        /// <list type="bullet">
        ///     <item>arr[0] 表示选中的项,没选中项 可null可空.<br/></item>
        ///     <item>
        ///         arr[1] 表示查询方向.<br/>
        ///         <list type="bullet">
        ///             <item>- 0和null = 选中项所有每个上级路径中同层所有项+选中项的下一级.<br/></item>
        ///             <item>- 1 = 同类型每个选中的search项的下一级所有项(仅仅下一级).<br/></item>
        ///         </list>
        ///     </item>
        /// </list>
        /// -------------------------------------------------------------------------------<br/>
        /// 格式2 ` [ ] ` .<br/>
        /// 数组arr不能为null,可空数组.(为null不会查询该类型)<br/>
        /// arr里的元素为选中的项, 没选中项为空数组.元素可以是数字|字符串<br/>
        /// 查询方向为0.<br/>
        /// </summary>
        public JToken[] Pr { get; set; }
        /// <summary>城市. 使用方式参考'省'</summary>
        public JToken[] Ci { get; set; }
        /// <summary>(一级)专业分类/专业. 使用方式参考'省'</summary>
        public JToken[] Ma { get; set; }
        /// <summary>(二级)专业分类/专业. 使用方式参考'省'</summary>
        public JToken[] Mas { get; set; }
        /// <summary>学校类型. 使用方式参考'省'</summary>
        public JToken[] Ty { get; set; }
        /// <summary>学校性质. 使用方式参考'省'</summary>
        public JToken[] Na { get; set; }
        /// <summary>学校等级. 使用方式参考'省'</summary>
        public JToken[] Lv { get; set; } 
    }
}
