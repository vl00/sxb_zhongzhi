using System;
namespace iSchool.Domain
{
    /// <summary>
    /// Represents a OnlineSchoolExtContent.
    /// NOTE: This class is generated from a T4 template - you should not modify it manually.
    /// </summary>
	[Table("OnlineSchoolExtContent")]
    public class OnlineSchoolExtContent
    {
        public OnlineSchoolExtContent()
        {
            this.ModifyDateTime = DateTime.Now;
            this.CreateTime = DateTime.Now;
        }
        public Guid Id { get; set; }
        public Guid Eid { get; set; }
        public int? Province { get; set; }
        public int? City { get; set; }
        public int? Area { get; set; }
        public string Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public iSchool.LngLatLocation LatLong { get; set; }
        public string Tel { get; set; }
        public bool? Lodging { get; set; }
        public bool? SdExtern { get; set; }
        public int? Studentcount { get; set; }
        public int? Teachercount { get; set; }
        public float? TsPercent { get; set; }
        public string Authentication { get; set; }
        public string Abroad { get; set; }
        public bool? Canteen { get; set; }
        public string Meal { get; set; }
        public string Characteristic { get; set; }
        public string Project { get; set; }
        public int? SeniorTea { get; set; }
        public int? CharacteristicTea { get; set; }
        public int? ForeignTeaCount { get; set; }
        public float? ForeignTea { get; set; }
        public DateTimeOffset? Creationdate { get; set; }
        public double? Acreage { get; set; }
        public string Openhours { get; set; }
        public string Calendar { get; set; }
        public string Range { get; set; }
        public string Counterpart { get; set; }
        public string Afterclass { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid Creator { get; set; }
        public DateTime ModifyDateTime { get; set; }
        public Guid Modifier { get; set; }
        public bool IsValid { get; set; }
        public float Completion { get; set; }
    }
}
