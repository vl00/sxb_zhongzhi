using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace iSchool.Svs.Domain.Enum
{
    /// <summary>
    /// 推荐类型 
    /// </summary>
    public enum RecommendType
    {
        /// <summary>专业?</summary>
        [Description("专业")]
        Type0 = 0,
        /// <summary>学校</summary>
        [Description("学校")]
        School = 1,
        
    }
}
