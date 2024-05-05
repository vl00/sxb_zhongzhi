using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iSchool.Api.Conventions;
using iSchool.Api.Swagger;
using iSchool.Domain.Modles;
using iSchool.Svs.Appliaction.RequestModels;
using iSchool.Svs.Appliaction.RequestModels.Register;
using iSchool.Svs.Appliaction.RequestModels.School;
using iSchool.Svs.Appliaction.ResponseModels;
using iSchool.Svs.Appliaction.ResponseModels.School;
using iSchool.Svs.Appliaction.ResponseModels.Schools;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace iSchool.Svs.Api.Controllers
{
    /// <summary>
    /// 快速登记
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        readonly IMediator _mediator;

        public RegisterController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="query">body</param>
        /// <returns></returns>
        [HttpPost("add")]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<ResResult> add(AddSvsRegisterDto query)
        {
            await Task.CompletedTask;
            var res = await _mediator.Send(query);
            return res ? ResResult.Success() : ResResult.Failed();
        }

    }
}
