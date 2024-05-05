using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace iSchool.Domain
{
    /// <summary>
    /// 
    /// </summary>
    [Table("HighSchoolAchievement")]
    public partial class HighSchoolAchievement
    {

        /// <summary>
        /// 
        /// </summary> 
        [ExplicitKey]
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        public Guid ExtId { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        public int Year { get; set; }

        /// <summary>
        /// �ر���
        /// </summary> 
        public double? Keyundergraduate { get; set; }

        /// <summary>
        /// ������
        /// </summary> 
        public double? Undergraduate { get; set; }

        /// <summary>
        /// ����¼ȡ����
        /// </summary> 
        public int? Count { get; set; }

        /// <summary>
        /// Name Time
        /// </summary> 
        public string Fractionaline { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 
        /// </summary> 
        public Guid Creator { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        public DateTime ModifyDateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 
        /// </summary> 
        public Guid Modifier { get; set; }

        /// <summary>
        /// 
        /// </summary> 
        public bool IsValid { get; set; } = true;
        /// <summary>
        /// �����
        /// </summary>
        public float? Completion { get; set; } = 0;

    }
}
