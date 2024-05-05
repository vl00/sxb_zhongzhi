using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace iSchool.Svs.Domain.Enum
{
    /// <summary>
    /// 学校性质 
    /// </summary>
    public enum SvsSchoolNature
    {
        /// <summary>公办</summary>
        [Description("公办")]
        Public = 1,
        /// <summary>民办</summary>
        [Description("民办")]
        Private = 2,
        /// <summary>独立院校</summary>
        [Description("独立院校")]
        Independent = 3,
        /// <summary>中外合办</summary>
        [Description("中外合办")]
        SinoforeignJoint = 4,
        /// <summary>校企合办</summary>
        [Description("校企合办")]
        SchEnterpriseCooperation = 5,
        /// <summary>其他</summary>
        [Description("其他")]
        Other = 6,
    }
}
