using iSchool.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace iSchool.Svs.Appliaction.ResponseModels.HotCategory
{
    public class MajorCategoryResult
    {
        /// <summary>
        /// 专业类目id
        /// </summary>
        public Guid Id { get; set; }

        private string no;
        /// <summary>
        /// 专业类目短id
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
        /// 专业类目名称
        /// </summary>
        public string Name { get; set; }
    }
}
