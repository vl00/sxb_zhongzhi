using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using iSchool.Api.Conventions;
using iSchool.Api.ModelBinders;
using iSchool.Api.Swagger;
using iSchool.Domain.Modles;
using iSchool.Svs.Api.ModelBinders;
using iSchool.Svs.Appliaction.RequestModels;
using iSchool.Svs.Appliaction.RequestModels.School;
using iSchool.Svs.Appliaction.ResponseModels;
using iSchool.Svs.Appliaction.ResponseModels.School;
using iSchool.Svs.Appliaction.ResponseModels.Schools;
using iSchool.Svs.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace iSchool.Svs.Api.Controllers
{
    /// <summary>
    /// 中职学校
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolController : ControllerBase
    {
        readonly IMediator _mediator;

        public SchoolController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// 首页
        /// </summary>
        /// <param name="province">省份,默认0=全国</param>
        /// <returns></returns>
        [HttpGet("")]
        [ProducesResponseType(typeof(SvsMainPageResult), 200)]
        public async Task<ResResult> Index(
            [SvsBindProvince]
            [ApiDocParameter(AllowEmptyValue = true)] 
            int province = 0)
        {
            var r = await _mediator.Send(new SvsMainPageQuery { Province = province });
            return ResResult.Success(r);
        }

        /// <summary>
        /// 首页中职目录
        /// </summary>
        /// <param name="province"></param>
        /// <returns></returns>
        [HttpGet("macgy")]
        [ProducesResponseType(typeof(SvsMajorCategoyResult), 200)]
        public async Task<ResResult> SvsMajorCategoyQuery([SvsBindProvince] int province = 0)
        {
            var r = await _mediator.Send(new SvsMajorCategoyQuery { Province = province });
            return ResResult.Success(r);
        }

        /// <summary>
        /// 推荐学校s
        /// </summary>
        /// <param name="province">省,默认0=全国</param>
        /// <returns></returns>
        [HttpGet("recommends")]
        [ProducesResponseType(typeof(RecommendSchoolsQyResult), 200)]
        public async Task<ResResult> GetRecommendSchools([SvsBindProvince, ApiDocParameter(AllowEmptyValue = true)] int province = 0)
        {
            var r = await _mediator.Send(new RecommendSchoolsQuery { Province = province });
            return ResResult.Success(r);
        }

        /// <summary>
        /// 获取搜索项s.详细的参数使用参考'pr'字段.
        /// </summary>
        /// <returns></returns>
        [HttpPost("searchitems")]
        [ProducesResponseType(typeof(GetSearchItemsResult), 200)]
        public async Task<ResResult> GetSearchItems(GetSearchItemsQuery1 queries, 
            [FromServices] IMapper mapper)
        {
            var r = await _mediator.Send(mapper.Map<GetSearchItemsQuery>(queries));
            return ResResult.Success(r);            
        }

        /// <summary>
        /// 根据搜索项s查询/搜索 学校列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("search")]
        [ProducesResponseType(typeof(SvsSchoolPageListQueryResult), 200)]
        public async Task<ResResult> SearchToPageList(SvsSchoolPageListQuery query)
        {
            var r = await _mediator.Send(query);
            return ResResult.Success(r);
        }

        /// <summary>
        /// 学校详情
        /// </summary>
        /// <param name="id">学校Id</param>
        /// <returns></returns>
        [HttpGet("detail/{id}")]
        [ProducesResponseType(typeof(SvsSchoolDetailResult), 200)]
        public async Task<ResResult> detail([CommaConv, ApiDocParameter(typeof(string), In = ParameterLocation.Path)] long id = 0)
        {
            await Task.CompletedTask;
            var res = await _mediator.Send(new SvsSchoolDetailQueryDto{No = id });
            return ResResult.Success(res);
        }

        /// <summary>
        /// 学校新闻
        /// </summary>
        /// <param name="id">学校id</param>
        /// <param name="PageIndex">第几页</param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        [HttpGet("news/{id}")]
        [ProducesResponseType(typeof(SvsSchoolNewsResult), 200)]
        public async Task<ResResult> news([CommaConv, ApiDocParameter(typeof(string), In = ParameterLocation.Path)] long id = 0, int PageIndex = 1,int PageSize = 100)
        {
            await Task.CompletedTask;
            var res = await _mediator.Send(new SvsSchoolNewsQuery { No = id,PageIndex = PageIndex ,PageSize = PageSize });
            return ResResult.Success(res);
        }

        /// <summary>
        /// 学校简介
        /// </summary>
        /// <param name="id">学校Id</param>
        /// <returns>页大小</returns>
        [HttpGet("intro/{id}")]
        [ProducesResponseType(typeof(SketchedResult), 200)]
        public async Task<ResResult> intro([CommaConv, ApiDocParameter(typeof(string), In = ParameterLocation.Path)] long id = 0)
        {
            await Task.CompletedTask;
            var res = await _mediator.Send(new SvsSchoolIntroQuery { No = id });
            return ResResult.Success(res);
        }

        /// <summary>
        /// 学校新闻详情
        /// </summary>
        /// <param name="id">新闻Id</param>
        /// <returns>页大小</returns>
        [HttpGet("newsdetail/{id}")]
        [ProducesResponseType(typeof(SvsSchoolNewsDetailResult), 200)]
        public async Task<ResResult> newsdetail([CommaConv, ApiDocParameter(typeof(string), In = ParameterLocation.Path)] long id = 0)
        {
            await Task.CompletedTask;
            var res = await _mediator.Send(new SvsSchoolNewsDetailQuery { No = id });
            return ResResult.Success(res);
        }

    }
}
