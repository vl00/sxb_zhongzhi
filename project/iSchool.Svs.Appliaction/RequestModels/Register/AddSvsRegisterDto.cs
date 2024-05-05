using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace iSchool.Svs.Appliaction.RequestModels.Register
{
    public class AddSvsRegisterDto : IRequest<bool>
    {
        /// <summary>
        /// 姓名
        /// </summary>
        [Required]
        [MaxLength(10, ErrorMessage = "最大不能超过10字符！")]
        public string Name { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [Required]
        [RegularExpression(@"^[^\u4e00-\u9fa5]{0,}$", ErrorMessage = "不能输入中文字符！")]
        [MaxLength(20, ErrorMessage = "最大不能超过20字符！")]
        public string Mobile { get; set; }

        /// <summary>
        /// 学校id
        /// </summary>
        public Guid SchoolId { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public int Sex { get; set; }

        /// <summary>
        /// 通信方式
        /// </summary>
        [MaxLength(20, ErrorMessage = "最大不能超过20字符！")]
        public string Chat { get; set; }

        /// <summary>
        /// 是否是应届生
        /// </summary>
        public int IsGraduates { get; set; }

        /// <summary>
        /// 专业意向
        /// </summary>
        [MaxLength(10, ErrorMessage = "最大不能超过10字符！")]
        public string Major { get; set; }
    }
}
