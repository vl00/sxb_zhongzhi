using Autofac;
using iSchool.Svs.Api.Configurations;
using iSchool.Svs.Api.Filters;
using iSchool.Svs.Api.Middlewares;
using iSchool.Svs.Api.Modules.AutofacModule;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NLog.Web;
using System;
using System.IO;
using System.Net.Http;
using System.Text;

namespace iSchool.Svs.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            //������������
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);  //������־�е������������

            services.AddControllers(o =>
            {
                //���ȫ�ִ��������
                o.Filters.Add(typeof(GlobalExceptionsFilter));
                o.Conventions.Add(new iSchool.Api.Conventions.CommaConvQueryStringConvention());
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            });

            //automapper
            services.AddAutoMapperSetup();

            //MediatR 
            services.AddMediatR(typeof(Startup));

            // httpcontext
            services.AddHttpContextAccessor(); //same as 'services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();'

            // csredis
            services.AddSingleton(sp => new CSRedis.CSRedisClient(Configuration["redis:0"]));

            // httpclient
            services.AddHttpClient(string.Empty, (http) =>
            {
                http.Timeout = new TimeSpan(0, 5, 0);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler() { UseProxy = false });

#if DEBUG
            //Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "iSchool.Svs.Api",
                    Version = "v1",
                    Description = "hushushu"
                });
                //�������ע��                
                var basePath = Path.GetDirectoryName(typeof(Startup).Assembly.ManifestModule.FullyQualifiedName);
                var files = Directory.EnumerateFiles(basePath, "iSchool.*.xml");
                foreach (var file in files)
                {
                    c.IncludeXmlComments(file);
                }

                c.DocInclusionPredicate((docName, description) => true);
                c.ParameterFilter<iSchool.Api.Swagger.ApiDocParameterOperationFilter>();
            });
            services.AddSwaggerGenNewtonsoftSupport();
#endif

            //services.Configure<KestrelServerOptions>(x => x.AllowSynchronousIO = true)
            //    .Configure<IISServerOptions>(x => x.AllowSynchronousIO = true);

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyMethod();
                    builder.AllowAnyHeader();
                    builder.SetIsOriginAllowed(_ => true);
                    builder.AllowCredentials();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Use(next => context =>
            {
                context.Request.EnableBuffering();
                return next(context);
            });

            //ʹ��NLog��Ϊ��־��¼����
            if (env.IsDevelopment())
            {
                NLogBuilder.ConfigureNLog("nlog.Development.config");
                app.UseDeveloperExceptionPage();
            }
            else if (env.IsProduction())
            {
                NLogBuilder.ConfigureNLog("nlog.config");
            }
            //��nlog �����ļ�������������־�����ַ���
            NLog.LogManager.Configuration.Variables["connectionString"] = Configuration["ConnectionStrings:LogSqlServerConnection"];

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseCors();


            app.UseAuthentication();
            app.UseRouting();


            //�����־�м��
            app.UseLoggerMiddleware();

#if DEBUG
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                // /swagger
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "iSchool.Svs.Api v1");
            });
#endif

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
        /// <summary>
        /// ע��autofac module
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new MediatorModule());
            builder.RegisterModule(new InfrastructureModule(Configuration.GetConnectionString("SqlServerConnection")));
            builder.RegisterModule(new DomainModule());
        }

    }
}
