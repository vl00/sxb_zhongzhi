using iSchool.Svs.Appliaction.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace iSchool.Svs.Appliaction.ResponseModels
{
#nullable enable

    /// <summary>首页返回</summary>
    public class SvsMainPageResult
    {
        /// <summary>选中的省份</summary>
        public AreaNameCodeDto SelectedProvince { get; set; } = default!;
        /// <summary>所有省</summary>
        public IEnumerable<AreaNameCodeDto> Provinces { get; set; } = default!;

        ///// <summary>中职专业目录</summary>
        //public IEnumerable<CgyItemListDto<SlimSvsMajorItemDto>> Categoies { get; set; } = default!;

        ///// <summary>推荐学校s</summary>
        //public IEnumerable<SvsSchoolItem1Dto> RecommendSchools { get; set; } = default!;

        /// <summary>院校库</summary>
        public IEnumerable<CgyItemListDto<SvsSchoolListItem1Dto>> Schools { get; set; } = default!;
        /// <summary>热门分类</summary>
        public IEnumerable<CgyItemListDto<SvsMajorItem1Dto>> Hots { get; set; } = default!;
        /// <summary>专业库</summary>
        public IEnumerable<CgyItemListDto<SvsMajorItem1Dto>> MajorResps { get; set; } = default!;
    }    

    /// <summary>仅仅用于主页-院校库list item</summary>
    public class SvsSchoolListItem1Dto
    {
        /// <summary>id</summary>
        public Guid Id { get; set; }
        /// <summary>短id</summary>
        public string Id_s { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Logo { get; set; }
        /// <summary>地址</summary>
        public string? Address { get; set; }
        /// <summary>专业数</summary>
        public int MajorCount { get; set; }
        /// <summary>学校性质</summary>
        public string? Nature { get; set; }
        /// <summary>学校等级</summary>
        public string? Level { get; set; }
        /// <summary>学校类型</summary>
        public string? Type { get; set; }
    }

    /// <summary>仅仅用于主页-专业库/热门专业</summary>
    public class SvsMajorItem1Dto
    { 
        /// <summary>图片可null</summary>
        public string? Img { get; set; }
        /// <summary>专业号</summary>
        public string Code { get; set; } = default!;
        /// <summary>专业名</summary>
        public string Name { get; set; } = default!;
        /// <summary>短id</summary>
        public string Id_s { get; set; } = default!;
    }

    ///// <summary>专业项 slim</summary>
    //public class SlimSvsMajorItemDto
    //{
    //    /// <summary>专业名</summary>
    //    public string Name { get; set; } = default!;
    //    /// <summary>短id</summary>
    //    public string Id_s { get; set; } = default!;
    //}

#nullable disable
}
