using AutoMapper;
using iSchool.Infrastructure;
using iSchool.Svs.Appliaction.RequestModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iSchool.Svs.Appliaction.AutoMapper
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            //CreateMap<,>():
            /* CreateMap<,>()
                .ForMember(t => t., option => option.MapFrom((s, t) => s.)); */


            CreateMap<GetSearchItemsQuery1, GetSearchItemsQuery>()
                .AfterMap((s, t) => from_GetSearchItemsQuery1_to_GetSearchItemsQuery(s, t));


        }

        static void from_GetSearchItemsQuery1_to_GetSearchItemsQuery(GetSearchItemsQuery1 s, GetSearchItemsQuery t)
        {            
            static IEnumerable<(string Type, JToken[][] JTokens, string Str4Err)> getJtk(GetSearchItemsQuery1 s)
            {
                yield return ("prci", new[] { s.Pr, s.Ci }, "传入选中的省市");
                yield return ("ma", new[] { s.Ma, s.Mas }, "传入选中的专业");
                yield return ("ty", new[] { s.Ty }, "传入选中的学校类型");
                yield return ("na", new[] { s.Na }, "传入选中的学校性质");
                yield return ("lv", new[] { s.Lv }, "传入选中的学校等级");
            }
            foreach (var x in getJtk(s))
            {
                if (!x.JTokens.Any(_ => _ != null)) continue;
                var item = new GetOneSearchTypeItemsQuery();
                t[x.Type] = (item);
                var selected = new List<string>();
                foreach (var jtoken in x.JTokens)
                {
                    if (jtoken == null) continue;
                    // ` [ [], 0 ] `
                    if (jtoken.ElementAtOrDefault(0) is JArray ja)
                    {
                        if (ja?.Any() == true) selected.AddRange(ja.Select(_ => (string)_));
                        var i = jtoken.ElementAtOrDefault(1) is JValue _i ? ((int?)_i ?? 0) : 0;
                        if (item.SelectedDirection != null && item.SelectedDirection != i)
                        {
                            throw new CustomResponseException($"{x.Str4Err}查询方向不一致.");
                        }
                        item.SelectedDirection = i;
                        continue;
                    }
                    // ` [ ] `
                    foreach (var _jv in jtoken)
                    {
                        if (!(_jv is JValue jv))
                        {
                            throw new CustomResponseException($"{x.Str4Err}格式不正确.");
                        }
                        item.SelectedDirection = 0;
                        selected.Add((string)jv);
                    }
                }
                item.Selected = selected.ToArray();
            }            
        }
    }
}
