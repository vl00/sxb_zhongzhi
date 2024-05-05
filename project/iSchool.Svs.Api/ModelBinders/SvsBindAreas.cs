using iSchool.Infrastructure;
using iSchool.Infrastructure.Extensions;
using iSchool.Svs.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iSchool.Svs.Api.ModelBinders
{
    /// <summary>
    /// for model-bind area and can set to cookie when response.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class SvsBindArea : ModelBinderAttribute
    {
        public SvsBindArea(string path4Url, string path4Header, string path4Cookie, bool rewriteCookieAfterResponse) : this()
        {
            this.path4Url = path4Url;
            this.path4Header = path4Header;
            this.path4Cookie = path4Cookie;
            this.rewriteCookieAfterResponse = rewriteCookieAfterResponse;
        }

        private SvsBindArea() : base(typeof(ModelBinder))
        {
            BindingSource = BindingSource.Query;
        }

        readonly string path4Url, path4Header, path4Cookie;
        readonly bool rewriteCookieAfterResponse;

        class ModelBinder : IModelBinder
        {
            public Task BindModelAsync(ModelBindingContext bindingContext)
            {
                var httpContext = bindingContext.HttpContext;
                var request = httpContext.Request;
                var routeData = bindingContext.ActionContext.RouteData;
                var metadata = bindingContext.ModelMetadata as DefaultModelMetadata;

                var pattr = metadata.Attributes.ParameterAttributes.OfType<SvsBindArea>().Single();                

                var str_id = "";
                if (string.IsNullOrEmpty(str_id) && !string.IsNullOrEmpty(pattr.path4Url))
                {
                    str_id = routeData.Values[pattr.path4Url]?.ToString();
                }
                if (string.IsNullOrEmpty(str_id) && !string.IsNullOrEmpty(pattr.path4Url))
                {
                    str_id = request.Query[pattr.path4Url];
                }
                if (string.IsNullOrEmpty(str_id) && !string.IsNullOrEmpty(pattr.path4Header))
                {
                    str_id = fmt_areacode_fromHeaderOrCookie(request.Headers[pattr.path4Header]);
                }
                if (string.IsNullOrEmpty(str_id) && !string.IsNullOrEmpty(pattr.path4Cookie))
                {
                    str_id = fmt_areacode_fromHeaderOrCookie(request.Cookies[pattr.path4Cookie]);                    
                }
                if (string.IsNullOrEmpty(str_id))
                {
                    // 默认全国
                    str_id = "0";
                }

                if (pattr.rewriteCookieAfterResponse)
                {
                    httpContext.Response.Cookies.Append(pattr.path4Cookie, str_id, new CookieOptions
                    {
                        HttpOnly = false,
                        //Path = "/",
                        Expires = DateTime.Now.AddDays(7),
                        Domain = ".sxkid.com",
                    });
                }

                try
                {
                    bindingContext.Result = ModelBindingResult.Success(Convert.ChangeType(str_id, bindingContext.ModelType));
                    return Task.CompletedTask;
                }
                catch (Exception ex)
                {
                    throw new CustomResponseException($"str_id='{str_id}'转换为'{bindingContext.ModelType.FullName}'失败.err: {ex.Message}");
                }
            }
        }

        static string fmt_areacode_fromHeaderOrCookie(string orginCode)
        {
            if (string.IsNullOrEmpty(orginCode)) return orginCode;

            JToken j = null;
            try { j = JToken.Parse(orginCode); } catch { }
            if (j == null) return orginCode;

            if (j is JValue jv)
            {
                return jv.ToString();
            }
            if (j is JObject jo)
            {
                var v = FindByNameIgnoreCase(jo, "code");
                return (string)v;
            }

            return orginCode;
        }

        static JToken FindByNameIgnoreCase(JToken jToken, string name)
        {
            if (!(jToken is JObject jo)) return null;
            return jo.Children().OfType<JProperty>().FirstOrDefault(_ => string.Equals(_.Name, name, StringComparison.OrdinalIgnoreCase))?.Value;
        }
    }

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class SvsBindProvince : SvsBindArea
    {
        public SvsBindProvince(bool rewriteCookieAfterResponse = false)
            : base(Consts.LocalProvinceInUrl, Consts.LocalProvince, Consts.LocalProvince, rewriteCookieAfterResponse)
        { }
    }
}
