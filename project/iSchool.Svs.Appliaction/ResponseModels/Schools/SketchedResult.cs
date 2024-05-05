using iSchool.Infrastructure;
using iSchool.Infrastructure.Extensions;
using iSchool.Organization.Appliaction.CommonHelper;
using iSchool.Svs.Domain.Enum;
using Newtonsoft.Json;
using System;

namespace iSchool.Svs.Appliaction.ResponseModels.Schools
{
    /// <summary>
    /// 学校概况
    /// </summary>
    public class SketchedResult
    {
        /// <summary>
        /// id
        /// </summary>
        public Guid Id { get; set; }

        private string no;
        /// <summary>
        /// 短id
        /// </summary>
        public string Id_s
        {
            get
            {
                return no;
            }
            set
            {
                try
                {
                    no = UrlShortIdUtil.Long2Base32(Convert.ToInt64(value));
                }
                catch
                {
                    no = value;
                }
            }
        }

        /// <summary>
        /// 学校名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 学校类型code
        /// 兼容读取缓存模式-不能忽略
        /// </summary>
        public int TypeId { get; set; }

        /// <summary>
        /// 学校类型
        /// </summary>
        public string Type => EnumUtil.GetDesc((SvsSchoolType)Enum.ToObject(typeof(SvsSchoolType), TypeId));

        /// <summary>
        /// 电话
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        /// 学校等级code
        /// 兼容读取缓存模式-不能忽略
        /// </summary>
        public int LevelId { get; set; }

        /// <summary>
        /// 学校等级
        /// </summary>
        public string Level => EnumUtil.GetDesc((SvsSchoolLevel)Enum.ToObject(typeof(SvsSchoolLevel), LevelId));

        /// <summary>
        /// 学校性质code
        /// 兼容读取缓存模式-不能忽略
        /// </summary>
        public int NatureId { get; set; }

        /// <summary>
        /// 学校性质
        /// </summary>
        public string Nature => EnumUtil.GetDesc((SvsSchoolNature)Enum.ToObject(typeof(SvsSchoolNature), NatureId));

        /// <summary>
        /// 学校地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 学校简介
        /// </summary>
        public string Intro { get; set; }

        /// <summary>
        /// 专业简介，去HTML标签
        /// </summary>
        public string IntroText => string.IsNullOrWhiteSpace(Intro) ? "" 
            : HtmlHelper.NoHTML(Intro).Substring(0, HtmlHelper.NoHTML(Intro).Length > 160 ? 160: HtmlHelper.NoHTML(Intro).Length);

        /// <summary>
        /// 封面logo
        /// </summary>
        public string Logo { get; set; }
    }
}
