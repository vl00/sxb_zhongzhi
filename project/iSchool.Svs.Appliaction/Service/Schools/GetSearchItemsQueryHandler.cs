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
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace iSchool.Organization.Appliaction.Service
{
    public class GetSearchItemsQueryHandler : IRequestHandler<GetSearchItemsQuery, GetSearchItemsResult>
    {
        SvsUnitOfWork unitOfWork;
        IMediator mediator;
        CSRedisClient redis;

        public GetSearchItemsQueryHandler(ISvsUnitOfWork unitOfWork, IMediator mediator, CSRedisClient redis)
        {
            this.unitOfWork = (SvsUnitOfWork)unitOfWork;
            this.mediator = mediator;
            this.redis = redis;
        }

        public async Task<GetSearchItemsResult> Handle(GetSearchItemsQuery query, CancellationToken cancellation)
        {
            var result = new GetSearchItemsResult();            
            await default(ValueTask);
            //
            // 目前只有2层 
            //
            await GetPrCiAreas(query.GetValueEx("prci"), result);
            await Gets_ma(query.GetValueEx("ma"), result);
            result.Ty = await Gets_ty(query.GetValueEx("ty"));
            result.Na = await Gets_na(query.GetValueEx("na"));
            result.Lv = await Gets_lv(query.GetValueEx("lv"));
            //;
            return result;
        }

        #region 地区省市
        private async Task GetPrCiAreas(GetOneSearchTypeItemsQuery query, GetSearchItemsResult result)
        {
            if (query == null) return;
            result.Prci = Enumerable.Empty<TreeItemDto<TreeItemDto>>();
            await default(ValueTask);

            var codes = fmt_selected(query.Selected).Distinct();

            // 向下
            if (query.SelectedDirection is 1)
            {
                await foreach (var (self, list) in prci_find_selected_items(codes))
                {
                    var item = new TreeItemDto<TreeItemDto>();
                    item.Id = res_areacode((int?)self._Ext1["depth"], self.Id);
                    item.Name = self.Name;
                    item.Pid = res_areacode(((int?)self._Ext1["depth"] ?? 0) - 1, self.Pid);
                    item.List = list.Select((c) =>
                    {
                        return new TreeItemDto
                        {
                            Id = res_areacode((int?)c._Ext1["depth"], c.Id),
                            Name = c.Name,
                            Pid = res_areacode((int?)self._Ext1["depth"], c.Pid),
                        };
                    });
                    result.Prci = result.Prci.Union(Enumerable.Repeat(item, 1));
                }
                result.Prci = result.Prci.AsArray();
            }
            // 向上(包括向下一级)
            else if (query.SelectedDirection is 0 || query.SelectedDirection is null)
            {
                var r1 = new GetSearchItemsResult();
                await GetPrCiAreas(new GetOneSearchTypeItemsQuery { Selected = new[] { "pr0" }, SelectedDirection = 1 }, r1);
                
                var areas = r1.Prci?.FirstOrDefault(_ => _.Id == "pr0")?.List?
                    .Select(_ => new TreeItemDto<TreeItemDto> { Id = _.Id, Pid = _.Pid, Name = _.Name, /*List = new List<TreeItemDto>(),*/ })
                    .ToList() ?? new List<TreeItemDto<TreeItemDto>>();
                areas.Insert(0, new TreeItemDto<TreeItemDto> { Id = "pr0", Name = "全国" });

                result.Prci = areas;

                await foreach (var (self, list) in prci_find_selected_items(codes))
                {
                    var depth = ((int?)self._Ext1["depth"]) ?? 0;
                    switch (depth)
                    {
                        default:
                        case 0: continue;
                        case 1:
                            {
                                var item = areas.FirstOrDefault(_ => _.Id == res_areacode(depth, self.Id));
                                if (item == null) continue;
                                item.List = list.Select((c) =>
                                {
                                    return new TreeItemDto
                                    {
                                        Id = res_areacode((int?)c._Ext1["depth"], c.Id),
                                        Name = c.Name,
                                        Pid = res_areacode(depth, c.Pid),
                                    };
                                });
                            }
                            break;
                        case 2:
                            {
                                // 选中第2层, 应该find其父节点的子list
                                //
                                var item = areas.FirstOrDefault(_ => _.Id == res_areacode(depth - 1, self.Pid));
                                if (item == null) continue;
                                if (item.List?.Any() == true) continue;
                                var enumr = prci_find_selected_items(new[] { get_intId(self.Pid).Value }).GetAsyncEnumerator();
                                if (!(await enumr.MoveNextAsync())) continue;
                                var (self1, list1) = enumr.Current;                               
                                item.List = list1.Select((c) =>
                                {
                                    return new TreeItemDto
                                    {
                                        Id = res_areacode((int?)c._Ext1["depth"], c.Id),
                                        Name = c.Name,
                                        Pid = res_areacode(depth - 1, c.Pid),
                                    };
                                });
                            }
                            break;
                    }
                }
            }
        }
        async IAsyncEnumerable<(TreeItemDto Self, IEnumerable<TreeItemDto> List)> prci_find_selected_items(IEnumerable<long> codes)
        { 
            foreach (var code in codes)
            {
                TreeItemDto self = null;
                IEnumerable<TreeItemDto> list = null;
                var rdk = CacheKeys.Area.FormatWith(code);
                var ss = await redis.HMGetAsync(rdk, "self", "list");
                if (ss?.Length >= 2 && ss[0] != null && ss[1] != null)
                {
                    self = ss[0].ToObject<TreeItemDto>();
                    list = ss[1].ToObject<List<TreeItemDto>>();
                }
                else
                {
                    var sql = @"
select [key] as id,name,parentid as pid,depth from KeyValue where IsValid=1 and type=1 and [key]=@id
---
select [key] as id,name,parentid as pid,depth from KeyValue where IsValid=1 and type=1 and parentid=@id and depth<=2 order by [key]
";
                    var gg = await unitOfWork.DbConnection.QueryMultipleAsync(sql, new { id = code });
                    self = gg.Read<TreeItemDto, int?, TreeItemDto>((tree, depth) =>
                    {
                        tree._Ext1["depth"] = depth;
                        return tree;
                    }, "depth").FirstOrDefault() ?? new TreeItemDto();
                    list = gg.Read<TreeItemDto, int?, TreeItemDto>((tree, depth) =>
                    {
                        tree._Ext1["depth"] = depth;
                        return tree;
                    }, "depth").AsList();
                    if (code == 0)
                    {
                        self.Id = "0";
                        self.Name = "全国";
                        self.Pid = null;
                        self._Ext1["depth"] = 0;
                    }
                    await redis.HMSetAsync(rdk, "self", self, "list", list);
                    _ = redis.ExpireAsync(rdk, 60 * 60 * 12);
                }
                if (self.Id == null)
                {
                    // ignore if it's not exists
                    continue;
                }                
                yield return (self, list);
            }
        }
        static IEnumerable<long> fmt_selected(string[] selected)
        {
            if (selected == null) yield break;
            foreach (var code in selected)
            {
                var s = get_intId(code);
                if (s == null) continue;
                yield return s.Value;
            }
        }
        static string res_areacode(int? depth, object code)
        {
            if (code is null || code is "") return code?.ToString();
            return depth switch
            {
                null => $"{code}",
                0 => $"pr{code}",
                1 => $"pr{code}",
                2 => $"ci{code}",
                3 => $"ar{code}",
                _ => $"ar{code}",
            };
        }
        #endregion

        static long? get_intId(string id)
        {
            if (id == null) return null;
            var gs = Regex.Match(id, @"(?<code>\d*)$", RegexOptions.IgnoreCase).Groups;
            var s = gs["code"].Value;
            return long.TryParse(s, out var _i) ? _i : (long?)null;
        }

        #region 专业分类/专业
        private async Task Gets_ma(GetOneSearchTypeItemsQuery query, GetSearchItemsResult result)
        {
            if (query == null) return;
            result.Ma = Enumerable.Empty<TreeItemDto<TreeItemDto>>();
            await default(ValueTask);

            var codes = fmt_selected(query.Selected).Distinct();

            // 向下
            if (query.SelectedDirection is 1)
            {
                await foreach (var (self, list) in ma_find_selected_items(codes))
                {
                    var item = new TreeItemDto<TreeItemDto>();
                    item.Id = res_macode((int?)self._Ext1["depth"], self.Id);
                    item.Name = self.Name;
                    item.Pid = res_macode(((int?)self._Ext1["depth"] ?? 0) - 1, self.Pid);
                    item.List = list.Select((c) =>
                    {
                        return new TreeItemDto
                        {
                            Id = res_macode((int?)c._Ext1["depth"], c.Id),
                            Name = c.Name,
                            Pid = res_macode((int?)self._Ext1["depth"], c.Pid),
                            _Ext1 = c._Ext1,
                        };
                    });
                    item._Ext1 = self._Ext1;
                    result.Ma = result.Ma.Union(Enumerable.Repeat(item, 1));
                }
                result.Ma = result.Ma.AsArray();
            }
            // 向上(包括向下一级)
            else
            {
                var r1 = new GetSearchItemsResult();
                await Gets_ma(new GetOneSearchTypeItemsQuery { Selected = new[] { "ma0" }, SelectedDirection = 1 }, r1);

                var ma = r1.Ma?.FirstOrDefault(_ => _.Id == "ma0")?.List?
                    .Select(_ => new TreeItemDto<TreeItemDto> { Id = _.Id, Pid = _.Pid, Name = _.Name, /*List = new List<TreeItemDto>(),*/ _Ext1 = _._Ext1 })
                    .ToList() ?? new List<TreeItemDto<TreeItemDto>>();
                ma.Insert(0, new TreeItemDto<TreeItemDto> { Id = "ma0", Name = "全部" });

                result.Ma = ma;

                await foreach (var (self, list) in ma_find_selected_items(codes))
                {
                    var depth = ((int?)self._Ext1["depth"]) ?? 0;
                    switch (depth)
                    {
                        default:
                        case 0: continue;
                        case 1:
                            {
                                var item = ma.FirstOrDefault(_ => _.Id == res_macode(depth, self.Id));
                                if (item == null) continue;
                                item.List = list.Select((c) =>
                                {
                                    return new TreeItemDto
                                    {
                                        Id = res_macode((int?)c._Ext1["depth"], c.Id),
                                        Name = c.Name,
                                        Pid = res_macode(depth, c.Pid),
                                        _Ext1 = c._Ext1,
                                    };
                                });
                            }
                            break;
                        case 2:
                            {
                                // 选中第2层, 应该find其父节点的子list
                                //
                                var item = ma.FirstOrDefault(_ => _.Id == res_macode(depth - 1, self.Pid));
                                if (item == null) continue;
                                if (item.List?.Any() == true) continue;
                                var enumr = ma_find_selected_items(new[] { get_intId(self.Pid).Value }).GetAsyncEnumerator();
                                if (!(await enumr.MoveNextAsync())) continue;
                                var (self1, list1) = enumr.Current;
                                item.List = list1.Select((c) =>
                                {
                                    return new TreeItemDto
                                    {
                                        Id = res_macode((int?)c._Ext1["depth"], c.Id),
                                        Name = c.Name,
                                        Pid = res_macode(depth - 1, c.Pid),
                                        _Ext1 = c._Ext1,
                                    };
                                });
                            }
                            break;
                    }
                }
            }
        }
        async IAsyncEnumerable<(TreeItemDto Self, IEnumerable<TreeItemDto> List)> ma_find_selected_items(IEnumerable<long> codes)
        {
            foreach (var code in codes)
            {
                TreeItemDto self = null;
                IEnumerable<TreeItemDto> list = null;
                var rdk = CacheKeys.Ma.FormatWith(code);
                var ss = await redis.HMGetAsync(rdk, "self", "list");
                if (ss?.Length >= 2 && ss[0] != null && ss[1] != null)
                {
                    self = ss[0].ToObject<TreeItemDto>();
                    list = ss[1].ToObject<List<TreeItemDto>>();
                }
                else
                {
                    var sql = $@"
select m1.no as id,m1.name,m0.no as pid,convert(int,m1.[level]) as depth,m1.logo
from Major m1 left join Major m0 on m0.IsValid=1 and m0.id=m1.pid
where m1.IsValid=1 and m1.no=@id
---
{(code <= 0 ? @"
select m1.no as id,m1.name,0 as pid,convert(int,m1.[level]) as depth,m1.logo 
from Major m1 
where m1.IsValid=1 and m1.[level]=1 and m1.pid is null
order by m1.viewcount,m1.no
" :
@"select m2.no as id,m2.name,m1.no as pid,convert(int,m2.[level]) as depth,m2.logo
from Major m1 join Major m2 on m1.id=m2.pid
where m1.IsValid=1 and m2.IsValid=1 and m2.[level]<=2 and m1.no=@id
order by m2.viewcount,m2.no
")} ";
                    var gg = await unitOfWork.DbConnection.QueryMultipleAsync(sql, new { id = code });
                    self = gg.Read<TreeItemDto, int?, string, TreeItemDto>((tree, depth, logo) =>
                    {
                        tree._Ext1["depth"] = depth;
                        tree._Ext1["logo"] = logo;
                        return tree;
                    }, "depth,logo").FirstOrDefault() ?? new TreeItemDto();
                    list = gg.Read<TreeItemDto, int?, string, TreeItemDto>((tree, depth, logo) =>
                    {
                        tree._Ext1["depth"] = depth;
                        tree._Ext1["logo"] = logo;
                        return tree;
                    }, "depth,logo").AsList();
                    if (code == 0)
                    {
                        self.Id = "0";
                        self.Name = "全部";
                        self.Pid = null;
                        self._Ext1["depth"] = 0;
                        self._Ext1["logo"] = null;
                    }
                    await redis.HMSetAsync(rdk, "self", self, "list", list);
                    _ = redis.ExpireAsync(rdk, 60 * 60 * 12);
                }
                if (self.Id == null)
                {
                    // ignore if it's not exists
                    continue;
                }
                yield return (self, list);
            }
        }
        static string res_macode(int? depth, object code)
        {
            if (code is null || code is "") return code?.ToString();
            return depth switch
            {
                null => $"{code}",
                0 => $"ma{code}",
                1 => $"ma{code}",
                2 => $"mas{code}",
                _ => $"mas{code}",
            };
        }
        #endregion

        /// <summary>
        /// 学校类型
        /// </summary>
        private async Task<IEnumerable<IdNameDto<string>>> Gets_ty(GetOneSearchTypeItemsQuery query)
        {
            if (query == null) return default;
            await Task.CompletedTask;

            if (query.SelectedDirection == 1)
            {
                return Enumerable.Empty<IdNameDto<string>>();
            }            
            else
            {
                return Enumerable.Repeat(new IdNameDto<string> { Id = "ty0", Name = "全部" }, 1)
                    .Union(EnumUtil.GetDescs<SvsSchoolType>().Select(x => new IdNameDto<string>
                    {
                        Id = $"ty{x.Value.ToInt()}",
                        Name = x.Desc,
                    }));                
            }
        }

        /// <summary>
        /// 学校性质
        /// </summary>
        private async Task<IEnumerable<IdNameDto<string>>> Gets_na(GetOneSearchTypeItemsQuery query)
        {
            if (query == null) return default;
            await Task.CompletedTask;

            if (query.SelectedDirection == 1)
            {
                return Enumerable.Empty<IdNameDto<string>>();
            }
            else
            {
                return Enumerable.Repeat(new IdNameDto<string> { Id = "na0", Name = "全部" }, 1)
                    .Union(EnumUtil.GetDescs<SvsSchoolNature>().Select(x => new IdNameDto<string>
                    {
                        Id = $"na{x.Value.ToInt()}",
                        Name = x.Desc,
                    }));
            }
        }

        /// <summary>
        /// 学校等级
        /// </summary>
        private async Task<IEnumerable<IdNameDto<string>>> Gets_lv(GetOneSearchTypeItemsQuery query)
        {
            if (query == null) return default;
            await Task.CompletedTask;

            if (query.SelectedDirection == 1)
            {
                return Enumerable.Empty<IdNameDto<string>>();
            }
            else
            {
                return Enumerable.Repeat(new IdNameDto<string> { Id = "lv0", Name = "全部" }, 1)
                    .Union(EnumUtil.GetDescs<SvsSchoolLevel>().Select(x => new IdNameDto<string>
                    {
                        Id = $"lv{x.Value.ToInt()}",
                        Name = x.Desc,
                    }));
            }
        }
    }
}
