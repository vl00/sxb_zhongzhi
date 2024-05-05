using System;
using System.Collections.Generic;
using System.Text;

namespace iSchool.Svs.Domain
{
    public static class Consts
    {

        //cache rule：1.系统   2.模块   3.参数名或者功能模块   4.附带参数
        //eg. org:course:courseid:{0}  
        //eg. org:course:coursesore:{0}:page:{1}
        //eg. org:eval:total:page:{1}
        //eg. org:eval:index:page:{1}

        public const string SwaggerSampleDataDir = "App_Data/swagger_sample_data/";

        /// <summary>省份url参数</summary>
        public const string LocalProvinceInUrl = "province";

        /// <summary>切换省份保存到cookie的path</summary>
        public const string LocalProvince = "localprovince";

    }
}
