using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using iSchool.Api.Conventions;
using iSchool.Api.ModelBinders;
using iSchool.Api.Swagger;
using iSchool.Domain.Modles;
using iSchool.Infrastructure.Extensions;
using iSchool.Svs.Api.ModelBinders;
using iSchool.Svs.Appliaction.RequestModels;
using iSchool.Svs.Appliaction.RequestModels.School;
using iSchool.Svs.Appliaction.ResponseModels;
using iSchool.Svs.Appliaction.ResponseModels.School;
using iSchool.Svs.Appliaction.ResponseModels.Schools;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace iSchool.Svs.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        readonly IMediator _mediator;

        public TestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public string Get()
        {
            Regex.Match("", @"(?<pr>\d\d)\d*$", RegexOptions.IgnoreCase);
            return "this is values";
        }

        //[HttpGet(nameof(Get1))]
        //public JsFrontItemList1Dto Get1()
        //{
        //    return new JsFrontItemList1Dto()
        //    {
        //        Type = new JsFrontItem1[0],
        //        Item = new JsFrontItem_list1[0],
        //    };
        //}

        [HttpGet("id/{id}")]
        public object FnId(
            [BindId_LongOrShort("id")]
            [ApiDocParameter(typeof(string), In = ParameterLocation.Path)]
            object id)
        {
            var src = id;
            var _long = (long?)null;
            var _guid = (Guid?)null;
            var _shortid = (string)null;

            if (id is Guid _g) _guid = _g;
            else if (id is long _l)
            {
                _long = _l;
                _shortid = UrlShortIdUtil.Long2Base32(_long.Value);
            }
            else
            {
                _shortid = src.ToString();
                _long = UrlShortIdUtil.Base322Long(_shortid);
            }
            return new { src, _guid, _long, _shortid };
        }

        [HttpPost("/api/v2/school/search/items")]
        [ProducesResponseType(typeof(GetSearchItemsResult), 200)]
        public async Task<ResResult> School_GetSearchItems(GetSearchItemsQuery query)
        {
            var r = query?.Any() != true ? new GetSearchItemsResult()
                : await _mediator.Send(query);
            return ResResult.Success(r);
        }


    }
}
