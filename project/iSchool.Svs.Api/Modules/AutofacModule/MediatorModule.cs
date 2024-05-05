using Autofac;
using FluentValidation;
using iSchool.Svs.Appliaction.RequestModels;
using MediatR;
using MediatR.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace iSchool.Svs.Api.Modules.AutofacModule
{
    /// <summary>
    /// Mediator 注入
    /// </summary>
    public class MediatorModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();

            var mediatrOpenTypes = new[]
            {
                typeof(IRequestHandler<,>),
                typeof(INotificationHandler<>),
                typeof(IValidator<>),
            };

            foreach (var mediatrOpenType in mediatrOpenTypes)
            {
                builder
                    .RegisterAssemblyTypes(typeof(SvsMainPageQuery).Assembly)
                    .AsClosedTypesOf(mediatrOpenType)
                    .AsImplementedInterfaces();

                //builder
                //    .RegisterAssemblyTypes(typeof(CourseQuery).GetTypeInfo().Assembly)
                //    .AsClosedTypesOf(mediatrOpenType)
                //    .AsImplementedInterfaces();

                //builder
                //     .RegisterAssemblyTypes(typeof(DomainNotificationHandler).GetTypeInfo().Assembly)
                //     .AsClosedTypesOf(mediatrOpenType)
                //     .AsImplementedInterfaces();
            }

            builder.RegisterGeneric(typeof(RequestPostProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(RequestPreProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));

            builder.Register<ServiceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });
        }
    }
}
