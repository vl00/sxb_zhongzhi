using iSchool.Svs.Appliaction.ResponseModels.Schools;
using MediatR;
using System.Collections.Generic;

namespace iSchool.Svs.Appliaction.RequestModels.School
{
    public class SvsSchoolNewsDetailQuery : IRequest<SvsSchoolNewsDetailResult>
    {
        /// <summary>
        /// 学校新闻No
        /// </summary>
        public long No { get; set; }
    }
}
