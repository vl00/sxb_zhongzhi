using iSchool.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace iSchool.Svs.Appliaction.ResponseModels
{
#nullable enable

    /// <summary>
    /// for院校库list item<br/>
    /// 目前能用于大多数页面
    /// </summary>
    public class SvsSchoolItem1Dto
    {
        /// <summary>id</summary>
        public Guid Id { get; set; }

        private string no = default!;
        /// <summary>短id</summary>
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

        public string Name { get; set; } = default!;
        public string? Logo { get; set; }
        /// <summary>地址</summary>
        public string? Address { get; set; }
        /// <summary>电话</summary>
        public string? Tel { get; set; }
        /// <summary>学校性质</summary>
        public string? Nature { get; set; }
        /// <summary>学校等级</summary>
        public string? Level { get; set; }
        /// <summary>学校类型</summary>
        public string? Type { get; set; }

        /// <summary>热门专业（pm话拿最热门的2个）</summary>
        public IEnumerable<string> HotMajors { get; set; } = default!;
    }

#nullable disable
}
