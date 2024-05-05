using iSchool.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace iSchool.Svs.Appliaction.ResponseModels.Schools
{
    /// <summary>
    /// 学校新闻列表
    /// </summary>
    public class SvsSchoolNewsResult
    {

        /// <summary>
        /// 新闻id
        /// </summary>
        public Guid Id { get; set; }

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
        /// 新闻标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifyDateTime { get; set; }
    }
}
