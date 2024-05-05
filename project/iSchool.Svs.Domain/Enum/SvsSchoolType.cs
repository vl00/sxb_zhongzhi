using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace iSchool.Svs.Domain.Enum
{
    /// <summary>
    /// 学校类型 
    /// </summary>
    public enum SvsSchoolType
    {
        /// <summary>技工学校</summary>
        [Description("技工学校")]
        Mechanic = 1,
        /// <summary>普通中专</summary>
        [Description("普通中专")]
        NormalSvs = 2,
        /// <summary>职业高中</summary>
        [Description("职业高中")]
        VocationalHigh = 3,
    }
}
