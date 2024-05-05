using iSchool.Svs.Appliaction.ResponseModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace iSchool.Svs.Appliaction.RequestModels
{
#nullable enable

    /// <summary>
    /// 查询学校s最热门的2个专业
    /// </summary>
    public class SvsSchoolsTop2HotMajorsQuery : IRequest<SvsSchoolsTop2HotMajorsQyResult>
    {
        public IEnumerable<Guid> SchoolsIds { get; set; } = default!;
    }

#nullable disable
}
