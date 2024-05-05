using iSchool.Svs.Appliaction.ResponseModels.Schools;
using MediatR;
using System.Collections.Generic;

namespace iSchool.Svs.Appliaction.RequestModels.School
{
    public class SvsSchoolNewsQuery : IRequest<IEnumerable<SvsSchoolNewsResult>>
    {
        /// <summary>
        /// 学校No
        /// </summary>
        public long No { get; set; }

        /// <summary>
        /// 第几页.不传默认为1
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 页大小.不传默认为100
        /// </summary>
        public int PageSize { get; set; } = 100;
    }
}
