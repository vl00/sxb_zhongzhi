using CSRedis;
using Dapper;
using iSchool.Infrastructure;
using iSchool.Infrastructure.UoW;
using iSchool.Svs.Appliaction.RequestModels;
using iSchool.Svs.Appliaction.ResponseModels;
using iSchool.Svs.Appliaction.ViewModels;
using iSchool.Svs.Domain;
using iSchool.Svs.Domain.Enum;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace iSchool.Organization.Appliaction.Service
{
    public class SvsMajorCategoyQueryHandler : IRequestHandler<SvsMajorCategoyQuery, SvsMajorCategoyResult>
    {
        SvsUnitOfWork unitOfWork;
        IMediator mediator;
        CSRedisClient redis;

        public SvsMajorCategoyQueryHandler(ISvsUnitOfWork unitOfWork, IMediator mediator, CSRedisClient redis)
        {
            this.unitOfWork = (SvsUnitOfWork)unitOfWork;
            this.mediator = mediator;
            this.redis = redis;
        }

        public async Task<SvsMajorCategoyResult> Handle(SvsMajorCategoyQuery query, CancellationToken cancellation)
        {
            var result = new SvsMajorCategoyResult();
            await default(ValueTask);

            if (query.Province != null)
            {
                var rr = await mediator.Send(new GetSearchItemsQuery
                {
                    ["prci"] = new GetOneSearchTypeItemsQuery { Selected = new[] { query.Province.Value.ToString() } }
                });
                if (rr.Prci?.Any() == true)
                {
                    result.Provinces = rr.Prci.Select(x =>
                    {
                        return new AreaNameCodeDto
                        {
                            Name = x.Name,
                            Code = get_intId(x.Id) ?? -1,
                        };
                    });
                    result.SelectedProvince = result.Provinces.FirstOrDefault(x => x.Code == query.Province)
                        ?? new AreaNameCodeDto { Name = "全国", Code = 0 };
                }
            }

            result.Categoies = await redis.GetAsync<CgyItemListDto<IdNameDto<string>>[]>(CacheKeys.MajorCategoy);
            if (result.Categoies == null)
            {
                var sql = @"
select --c.*,m.*,b.* 
c.no as Item1,c.name as Item2,c.sort as Item3,m.no as Item4,m.name as Item5,b.sort as Item6,c.type,m.level
from Category c left join MajorCategory b on b.IsValid=1 and c.id=b.categoryid
left join Major m on m.IsValid=1 and m.id=b.majorid
where c.IsValid=1 and c.type=0 --and m.level=2
order by c.sort,b.sort
";
                var arr = await unitOfWork.DbConnection.QueryAsync<(string Id1, string Name1, int Sort1, string Id2, string Name2, int Sort2)>(sql);
                arr = arr.AsArray();

                result.Categoies = arr.GroupBy(_ => (_.Id1, _.Name1, _.Sort1)).OrderBy(_ => _.Key.Sort1)
                    .Select(g => new CgyItemListDto<IdNameDto<string>>
                    {
                        Id = g.Key.Id1,
                        Name = g.Key.Name1,
                        List = g.OrderBy(_ => _.Sort2).Select(_ => new IdNameDto<string>
                        {
                            Id = "mas" + _.Id2,
                            Name = _.Name2,
                        })
                    })
                    .AsArray();

                await redis.SetAsync(CacheKeys.MajorCategoy, result.Categoies, 60 * 60 * 24 * 7);
            }

            return result;
        }

        static int? get_intId(string id)
        {
            if (id == null) return null;
            var gs = Regex.Match(id, @"(?<code>\d*)$", RegexOptions.IgnoreCase).Groups;
            var s = gs["code"].Value;
            return int.TryParse(s, out var _i) ? _i : (int?)null;
        }

    }
}
