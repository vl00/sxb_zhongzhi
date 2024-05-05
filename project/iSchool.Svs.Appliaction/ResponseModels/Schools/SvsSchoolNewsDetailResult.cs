using iSchool.Infrastructure.Extensions;
using iSchool.Organization.Appliaction.CommonHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace iSchool.Svs.Appliaction.ResponseModels.Schools
{
    /// <summary>
    /// 学校新闻详情
    /// </summary>
    public class SvsSchoolNewsDetailResult
    {

        /// <summary>
        /// 新闻id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 新闻短id code
        /// </summary>
        private string no;

        /// <summary>
        /// 新闻短id
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
        /// 学校短id code
        /// </summary>
        private string SchoolId;

        /// <summary>
        /// 学校短id
        /// </summary>
        public string SchoolId_s
        {
            get
            {
                return SchoolId;
            }
            set
            {
                try
                {
                    SchoolId = UrlShortIdUtil.Long2Base32(Convert.ToInt64(value));
                }
                catch
                {
                    SchoolId = value;
                }
            }
        }

        /// <summary>
        /// 学校名称
        /// </summary>
        public string SchoolName { get; set; }

        /// <summary>
        /// 新闻标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 内容，去HTML标签
        /// </summary>
        public string IntroText => string.IsNullOrWhiteSpace(Content) ? ""
            : HtmlHelper.NoHTML(Content).Substring(0, HtmlHelper.NoHTML(Content).Length > 160 ? 160 : HtmlHelper.NoHTML(Content).Length);

        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get; set; }
    }
}
