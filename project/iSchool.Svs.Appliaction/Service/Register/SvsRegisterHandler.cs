using Dapper;
using iSchool.Infrastructure.Dapper;
using iSchool.Infrastructure.UoW;
using iSchool.Svs.Appliaction.RequestModels.Category;
using iSchool.Svs.Appliaction.RequestModels.Employment;
using iSchool.Svs.Appliaction.RequestModels.Register;
using iSchool.Svs.Appliaction.ResponseModels;
using iSchool.Svs.Appliaction.ResponseModels.Employment;
using iSchool.Svs.Appliaction.ResponseModels.HotCategory;
using iSchool.Svs.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace iSchool.Svs.Appliaction.Service.Category
{
    public class SvsRegisterHandler : IRequestHandler<AddSvsRegisterDto, bool>
    {
        SvsUnitOfWork svsUnitOfWork;

        public SvsRegisterHandler(ISvsUnitOfWork svsUnitOfWork)
        {
            this.svsUnitOfWork = (SvsUnitOfWork)svsUnitOfWork;
        }

        /// <summary>
        /// 新增报名
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public async Task<bool> Handle(AddSvsRegisterDto query, CancellationToken cancellation)
        {
            await Task.CompletedTask;
            //新增报名
            var res = Add(query);
            return res;
        }

        /// <summary>
        /// 新增报名
        /// </summary>
        /// <param name="query"></param>
        public bool Add(AddSvsRegisterDto query)
        {
            var sql = @"INSERT INTO [dbo].[Information]
                        ([id],[schoolid],[major],[name],[tel],[chat],[isgraduates],[sex],[status],[createTime])
                        VALUES
                        (NEWID(),@schoolid,@major,@name,@tel,@chat,@isgraduates,@sex,@status,@createTime)";
            var res = svsUnitOfWork.DbConnection.Execute(sql, new DynamicParameters()
                .Set("schoolid", query.SchoolId)
                .Set("major", query.Major)
                .Set("name", query.Name)
                .Set("tel", query.Mobile)
                .Set("chat", query.Chat)
                .Set("isgraduates", query.IsGraduates == -1 ? null : query?.IsGraduates)
                .Set("sex", query.Sex == -1 ? null : query?.Sex)
                .Set("status", 0)
                .Set("createTime", DateTime.Now)
                , svsUnitOfWork.DbTransaction);
            return res > 0;
        }
    }
}
