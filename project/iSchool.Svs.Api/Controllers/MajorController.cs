using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iSchool.Api.Conventions;
using iSchool.Api.Swagger;
using iSchool.Domain.Modles;
using iSchool.Infrastructure.Extensions;
using iSchool.Svs.Api.ModelBinders;
using iSchool.Svs.Appliaction.RequestModels;
using iSchool.Svs.Appliaction.RequestModels.Employment;
using iSchool.Svs.Appliaction.RequestModels.Register;
using iSchool.Svs.Appliaction.RequestModels.School;
using iSchool.Svs.Appliaction.ResponseModels;
using iSchool.Svs.Appliaction.ResponseModels.Employment;
using iSchool.Svs.Appliaction.ResponseModels.School;
using iSchool.Svs.Appliaction.ResponseModels.Schools;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace iSchool.Svs.Api.Controllers
{
    /// <summary>
    /// 专业
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MajorController : ControllerBase
    {
        readonly IMediator _mediator;

        public MajorController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// 专业详情
        /// </summary>
        /// <param name="majorIid">专业id</param>
        /// <param name="schoolIid">学校id</param>
        /// <param name="province">区域</param>
        /// <returns></returns>
        [HttpGet("detail/{majorIid?}/{schoolIid?}")]
        [ProducesResponseType(typeof(SvsMajorDetailResult), 200)]
        public async Task<ResResult> Detail([CommaConv, ApiDocParameter(typeof(string), In = ParameterLocation.Path)] long majorIid = 0
            , [ApiDocParameter(AllowEmptyValue = true)] string schoolIid = "", [SvsBindProvince] int province = 0)
        {
            await Task.CompletedTask;
            long id = 0;
            long.TryParse(schoolIid,out id);
            if (id == 0)
            {
                id = UrlShortIdUtil.Base322Long(schoolIid);
            }
            var res = await _mediator.Send(new SvsProspectsDetailQuery { MajorId = majorIid,SchoolId = id, Province = province});
            return ResResult.Success(res);
        }
    }
}
