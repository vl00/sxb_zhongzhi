using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iSchool.Api.Conventions;
using iSchool.Api.Swagger;
using iSchool.Domain.Modles;
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
    /// 就业
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EmploymentController : ControllerBase
    {
        readonly IMediator _mediator;

        public EmploymentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// 就业前景列表
        /// </summary>
        /// <param name="query">body</param>
        /// <returns></returns>
        [HttpPost("prospects")]
        [ProducesResponseType(typeof(SvsProspectsResult), 200)]
        public async Task<ResResult> Prospects(SvsPageQuery query)
        {
            await Task.CompletedTask;
            var res = await _mediator.Send(query);
            return ResResult.Success(res);
        }

        ///// <summary>
        ///// 就业前景详情
        ///// </summary>
        ///// <param name="id">专业id</param>
        ///// <param name="province">区域</param>
        ///// <returns></returns>
        //[HttpGet("detail/{id}")]
        //[ProducesResponseType(typeof(SvsProspectsDetailResult), 200)]
        //public async Task<ResResult> Detail([CommaConv, ApiDocParameter(typeof(string), In = ParameterLocation.Path)] long id = 0, [SvsBindProvince] int province = 0)
        //{
        //    await Task.CompletedTask;
        //    var res = await _mediator.Send(new SvsProspectsDetailQuery { Id = id,Province = province});
        //    return ResResult.Success(res);
        //}

    }
}
