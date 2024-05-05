using iSchool.Infrastructure.Dapper;
using iSchool.Svs.Appliaction.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace iSchool.Svs.Appliaction.ResponseModels
{
#nullable enable

    public class SvsSchoolPageListQueryResult
    {
        /// <summary>搜索内容</summary>
        public string? SearchTxt { get; set; }
        /// <summary>学校列表分页数据</summary>
        public PagedList<SvsSchoolItem1Dto> SchoolsPageInfo { get; set; } = default!;
    }

#nullable disable
}
