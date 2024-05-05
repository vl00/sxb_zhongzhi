using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace iSchool.Svs.Domain.Enum
{
    /// <summary>
    /// 学校等级
    /// </summary>
    public enum SvsSchoolLevel
    {
        /// <summary>国家级示范</summary>
        [Description("国家级示范")]
        Enum1 = 1,
        /// <summary>国家级重点</summary>
        [Description("国家级重点")]
        Enum2 = 2,
        /// <summary>省级示范</summary>
        [Description("省级示范")]
        Enum3 = 3,
        /// <summary>省级重点</summary>
        [Description("省级重点")]
        Enum4 = 4,
        /// <summary>示范性高职</summary>
        [Description("示范性高职")]
        Enum5 = 5,
        /// <summary>骨干高职</summary>
        [Description("骨干高职")]
        Enum6 = 6,
        /// <summary>普通学校</summary>
        [Description("普通学校")]
        Enum7 = 7,
    }
}
