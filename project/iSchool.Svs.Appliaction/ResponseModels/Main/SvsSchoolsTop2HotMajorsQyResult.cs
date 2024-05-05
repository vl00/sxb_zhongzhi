using System;
using System.Collections.Generic;
using System.Text;

namespace iSchool.Svs.Appliaction.ResponseModels
{
#nullable enable

    public class SvsSchoolsTop2HotMajorsQyResult
    {
        public IDictionary<Guid, (Guid MajorId, string MajorName)[]?> Items { get; set; } = default!;
    }

#nullable disable
}
