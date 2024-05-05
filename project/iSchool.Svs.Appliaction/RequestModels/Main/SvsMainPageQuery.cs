using iSchool.Svs.Appliaction.ResponseModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace iSchool.Svs.Appliaction.RequestModels
{
#nullable enable

    public class SvsMainPageQuery : IRequest<SvsMainPageResult>
    {
        public int Province { get; set; } = 0;
    }

#nullable disable
}
