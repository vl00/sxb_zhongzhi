using System.Collections.Generic;
using System.Threading.Tasks;
using iSchool.Api.Conventions;
using iSchool.Api.Swagger;
using iSchool.Domain.Modles;
using iSchool.Infrastructure.Common;
using iSchool.Svs.Api.ModelBinders;
using iSchool.Svs.Appliaction.RequestModels.Category;
using iSchool.Svs.Appliaction.ResponseModels;
using iSchool.Svs.Appliaction.ResponseModels.HotCategory;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace iSchool.Svs.Api.Controllers
{
    /// <summary>
    /// 热门分类
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class HotCategoryController : ControllerBase
    {
        readonly IMediator _mediator;

        public HotCategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// 热门分类\热门专业\热门学校
        /// </summary>
        /// <param name="id">分类id</param>
        /// <param name="province">区域</param>
        /// <returns></returns>
        [HttpGet("list/{id?}")]
        [ProducesResponseType(typeof(HotCategoryResult), 200)]
        public async Task<ResResult> category([ApiDocParameter(AllowEmptyValue = true)] string id = "", [SvsBindProvince] int province = 0)
        {
            await Task.CompletedTask;
            var res = await _mediator.Send(new SvsHotCategoryQuery { Id = id,Province = province });
            return ResResult.Success(res);
        }
    }
}
