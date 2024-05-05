using Autofac;
using iSchool.Domain.Repository.Interfaces;
using iSchool.Infrastructure.Repositories.Svs;
using iSchool.Infrastructure.UoW;
using iSchool.Svs.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iSchool.Svs.Api.Modules.AutofacModule
{
    public class InfrastructureModule : Autofac.Module
    {
        private readonly string _databaseConnectionString;
        public InfrastructureModule(string databaseConnectionString)
        {
            this._databaseConnectionString = databaseConnectionString;
        }
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(SvsBaseRepository<>))
            .As(typeof(IBaseRepository<>))
            .Named("SvsBaseRepository", typeof(IBaseRepository<>))
            .InstancePerLifetimeScope();

            //main repository
            builder.RegisterGeneric(typeof(SvsBaseRepository<>))
             .As(typeof(IRepository<>))
             .InstancePerLifetimeScope();

            builder.RegisterType<SvsUnitOfWork>()
           .As<ISvsUnitOfWork>()
           .WithParameter("connectionString", _databaseConnectionString)
           .InstancePerLifetimeScope();
        }
    }
}
