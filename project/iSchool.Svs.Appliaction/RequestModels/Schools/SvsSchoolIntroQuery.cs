using iSchool.Svs.Appliaction.ResponseModels.Schools;
using MediatR;
using System.Collections.Generic;

namespace iSchool.Svs.Appliaction.RequestModels.School
{
    public class SvsSchoolIntroQuery : IRequest<SketchedResult>
    {
        /// <summary>
        /// 学校No
        /// </summary>
        public long No { get; set; } 
    }
}
