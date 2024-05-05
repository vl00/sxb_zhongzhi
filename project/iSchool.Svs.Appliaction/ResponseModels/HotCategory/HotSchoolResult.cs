using iSchool.Infrastructure;
using iSchool.Svs.Domain.Enum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace iSchool.Svs.Appliaction.ResponseModels.HotCategory
{
    public class HotSchoolResult: SvsSchoolItem1Dto
    {
        /// <summary>
        /// 热门专业
        /// </summary>
        public new string HotMajors { get; set; } = default!;

        /// <summary>
        /// 学校性质code
        /// 兼容读取缓存模式-不能忽略
        /// </summary>
        public int NatureId { get; set; } = default!;

        /// <summary>
        /// 学校性质
        /// </summary>
        public new string Nature => EnumUtil.GetDesc((SvsSchoolNature)Enum.ToObject(typeof(SvsSchoolNature), NatureId));

        /// <summary>
        /// 学校等级code
        /// 兼容读取缓存模式-不能忽略
        /// </summary>
        public int LevelId { get; set; } = default!;

        /// <summary>
        /// 学校等级
        /// </summary>
        public new string Level => EnumUtil.GetDesc((SvsSchoolLevel)Enum.ToObject(typeof(SvsSchoolLevel), LevelId));
    }
}
