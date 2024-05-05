using iSchool.Infrastructure.Extensions;
using iSchool.Organization.Appliaction.CommonHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace iSchool.Svs.Appliaction.ResponseModels.HotCategory
{
    /// <summary>
    /// 专业信息
    /// </summary>
    public class MajorDetailResult
    {

        /// <summary>
        /// 专业no mas+no
        /// </summary>
        public string MajorNo { get; set; }

        /// <summary>
        /// 专业id
        /// </summary>
        public Guid Id { get; set; }

        private string no;

        /// <summary>
        /// 专业短id
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
        /// 专业名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 专业简介
        /// </summary>
        public string Intro { get; set; }

        /// <summary>
        /// 专业简介，去HTML标签
        /// </summary>
        public string IntroText => string.IsNullOrWhiteSpace(Intro) ? ""
            : HtmlHelper.NoHTML(Intro).Substring(0, HtmlHelper.NoHTML(Intro).Length > 160 ? 160 : HtmlHelper.NoHTML(Intro).Length);


        /// <summary>
        /// 专业图片
        /// </summary>
        public string Logo { get; set; }

        /// <summary>
        /// 就业前景
        /// </summary>
        public string Prospects { get; set; }

        /// <summary>
        /// 招生人数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 学校层级
        /// </summary>
        public string EduLevel { get; set; }

        /// <summary>
        /// 学制
        /// </summary>
        public string SchoolSystem { get; set; }

        /// <summary>
        /// 招生对象
        /// </summary>
        public string TargetName { get; set; }

        /// <summary>
        /// 学费
        /// </summary>
        public string Tuition { get; set; }

        /// <summary>
        /// 学历层次
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// 专业代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 薪资待遇
        /// </summary>
        public string Salary { get; set; }

        /// <summary>
        /// 专业热度
        /// </summary>
        public string MajorCelsius { get; set; }

        /// <summary>
        /// 专业开办学校数
        /// </summary>
        public string SchoolNumber { get; set; }
    }
}
