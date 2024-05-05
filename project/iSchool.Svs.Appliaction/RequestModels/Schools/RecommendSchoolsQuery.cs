using iSchool.Svs.Appliaction.ResponseModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace iSchool.Svs.Appliaction.RequestModels
{
#nullable enable

    /// <summary>
    /// 随机查询某个省的10个学校作推荐
    /// </summary>
    public class RecommendSchoolsQuery : IRequest<RecommendSchoolsQyResult>
    {
        public int Province { get; set; } = 0;
    }

#nullable disable
}
