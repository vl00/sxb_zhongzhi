using System;
using System.Collections.Generic;
using System.Text;

namespace iSchool.Svs.Appliaction.ViewModels
{
#nullable enable

    public class IdNameDto<Tid>
    {
        /// <summary>唯一标识,如id/code</summary>
        public virtual Tid Id { get; set; } = default!;
        /// <summary>用于显示的名称</summary>
        public virtual string Name { get; set; } = default!;        
    }

#nullable disable
}
