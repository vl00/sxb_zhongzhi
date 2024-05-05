﻿using iSchool.Infrastructure;
using iSchool.Infrastructure.Extensions;
using iSchool.Svs.Appliaction.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace iSchool.Svs.Api.Filters
{
    public class GlobalExceptionsFilter : IExceptionFilter
    {

        public void OnException(ExceptionContext actionExecutedContext)
        {

            Logger _logger = LogManager.GetCurrentClassLogger();

            //获取错误信息
            var exption = actionExecutedContext.Exception;
            //获取请求参数
            var queryDic = actionExecutedContext.HttpContext.Request.Query.ToDictionary(kv => kv.Key, kv =>
            {
                var v = kv.Value.ToString();
                if (Guid.TryParse(v, out var gid)) return gid.ToString("n");
                return v;
            });

            var queryString = string.Join("&", queryDic.Select(q => q.Key + "=" + q.Value));
            string Params = "";

            if (actionExecutedContext.HttpContext.Request.Method.ToLower() == "get")
            {
                Params = queryString;
            }
            else
            {
                var request = actionExecutedContext.HttpContext.Request;
                Params = BodyToString(request);

                //actionExecutedContext.HttpContext.Request.EnableBuffering();
                //using (var reader = new StreamReader(actionExecutedContext.HttpContext.Request.Body, Encoding.UTF8, true, 1024, true))
                //{
                //    // Params = reader.ReadToEnd(); //Synchronous operations are disallowed. Call ReadAsync or set AllowSynchronousIO to true instead
                //    Params = reader.ReadToEndAsync().Result;
                //}
                //actionExecutedContext.HttpContext.Request.Body.Position = 0;
            }

            LogEventInfo ei = new LogEventInfo();
            ei.Properties["BusinessId"] = "";
            ei.Properties["ErrorCode"] = "";
            ei.Properties["Time"] = DateTime.Now.ToMillisecondString();
            ei.Properties["Url"] = actionExecutedContext.HttpContext.Request.GetDisplayUrl();
            ei.Properties["Application"] = exption.Source;
            ei.Properties["Class"] = "[" + exption.GetType().FullName + "]";
            ei.Properties["Method"] = exption.TargetSite.Name.ToString();
            ei.Properties["Params"] = Params;
            ei.Properties["Ip"] = GetClientIP(actionExecutedContext.HttpContext);
            ei.Properties["Host"] = Dns.GetHostName();
            ei.Properties["ThreadId"] = Thread.CurrentThread.ManagedThreadId.ToString();

            //获取用户信息
            //var user = actionExecutedContext.HttpContext.RequestServices.GetService<IUserInfo>();
            //ei.Properties["UserId"] = user.UserId;
            //ei.Properties["Role"] = user.UserRole;


            ei.Properties["Caption"] = "ExceptionHandling";
            ei.Properties["Content"] = exption.Message;
            ei.Properties["Error"] = exption.Message;
            ei.Properties["StackTrace"] = exption.StackTrace;
            if (exption is CustomResponseException)
            {
                var creExption = (CustomResponseException)exption;

                //返回自定义错误信息到前端
                //var response = ResponseResult.Failed(ei.Message);
                var response = ResponseResult.Failed(creExption.Message);
                actionExecutedContext.Result = new ObjectResult(response);

                //自定义的错误进行处理
                ei.Properties["Error"] = creExption.Message;
                ei.Properties["ErrorCode"] = creExption.ErrorCode;
                ei.Properties["Level"] = "自定义错误";
                _logger.Info(ei);
            }
            else if (exption is AuthResponseException)
            {
                //返回错误信息
                var response = new ResponseResult
                {
                    Succeed = false,
                    status = Domain.Enum.ResponseCode.NoAuth,
                    Msg = "没有权限"
                };
                actionExecutedContext.Result = new ObjectResult(response);

                ei.Properties["ErrorCode"] = 403;
                ei.Properties["Level"] = "权限错误";
                _logger.Info(ei);

            }
            else
            {
                //返回错误信息
                var response = new ResponseResult
                {
                    Succeed = false,
                    status = Domain.Enum.ResponseCode.Error,
#if DEBUG
                    Msg = $"系统错误: {exption.Message}\n\t{exption.StackTrace}",
#else
                    Msg = "系统错误",
#endif
                    //Data = actionExecutedContext.HttpContext.Request.ContentType.ToLower().Contains("form-data") ?
                    //(object)actionExecutedContext.HttpContext.Request.Form : (object)Params
                };
                actionExecutedContext.Result = new ObjectResult(response);


                ei.Properties["Level"] = "错误";
                _logger.Error(ei);
            }
        }

        /// <summary>
        /// 获取用户ip
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string GetClientIP(HttpContext context)
        {
            var ip = context.Request.Headers["X-Forwarded-For"].ObjToString();
            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Connection.RemoteIpAddress.ObjToString();
            }
            return ip;
        }


        public string BodyToString(HttpRequest request)
        {
            var returnValue = string.Empty;
            //request.EnableRewind();
            //ensure we read from the begining of the stream - in case a reader failed to read to end before us.
            request.Body.Position = 0;
            //use the leaveOpen parameter as true so further reading and processing of the request body can be done down the pipeline
            using (var stream = new StreamReader(request.Body, Encoding.UTF8, true, 1024, leaveOpen: true))
            {
                returnValue = stream.ReadToEndAsync().Result;
            }
            //reset position to ensure other readers have a clear view of the stream 
            request.Body.Position = 0;
            return returnValue;
        }
    }
}
