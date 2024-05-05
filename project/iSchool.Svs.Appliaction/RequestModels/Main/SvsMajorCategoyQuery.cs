using iSchool.Svs.Appliaction.ResponseModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace iSchool.Svs.Appliaction.RequestModels
{
#nullable enable

    public class SvsMajorCategoyQuery : IRequest<SvsMajorCategoyResult>
    { 
        /// <summary>
        /// 为null时不查询省
        /// </summary>
        public int? Province { get; set; }
    }

#nullable disable
}
