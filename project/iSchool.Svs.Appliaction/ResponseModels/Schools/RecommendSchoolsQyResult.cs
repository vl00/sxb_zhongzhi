using System;
using System.Collections.Generic;
using System.Text;

namespace iSchool.Svs.Appliaction.ResponseModels
{
#nullable enable

    public class RecommendSchoolsQyResult
    {
        /// <summary>推荐学校s</summary>
        public IEnumerable<SvsSchoolItem1Dto> RecommendSchools { get; set; } = default!;
    }

#nullable disable
}
