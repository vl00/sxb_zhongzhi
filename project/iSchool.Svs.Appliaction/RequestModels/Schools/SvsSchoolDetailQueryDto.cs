using iSchool.Svs.Appliaction.ResponseModels.School;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace iSchool.Svs.Appliaction.RequestModels.School
{
    public class SvsSchoolDetailQueryDto : IRequest<SvsSchoolDetailResult>
    {
        /// <summary>
        /// 学校No
        /// </summary>
        public long No { get; set; } 
    }
}
