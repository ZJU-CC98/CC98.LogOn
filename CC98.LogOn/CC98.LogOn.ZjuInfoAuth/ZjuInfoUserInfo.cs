using System;
using Newtonsoft.Json;

namespace CC98.LogOn.ZjuInfoAuth
{
	/// <summary>
	/// 定义浙大通行证提供的用户信息。
	/// </summary>
	public class ZjuInfoUserInfo
	{
		/// <summary>
		/// 用户的唯一编号（学工号）。
		/// </summary>
		[JsonProperty("CODE")]
		public string Id { get; set; }

		/// <summary>
		/// 用户的姓名。
		/// </summary>
		[JsonProperty("XM")]
		public string Name { get; set; }

		/// <summary>
		/// 用户的性别代码。
		/// </summary>
		[JsonProperty("XB")]
		public ZjuInfoGender GenderCode { get; set; }

		/// <summary>
		/// 用户性别。
		/// </summary>
		[JsonProperty("XBMC")]
		public string Gender { get; set; }

		/// <summary>
		/// 用户所在的单位的代码。
		/// </summary>
		[JsonProperty("JGDM")]
		public int OrgnizationCode { get; set; }

		/// <summary>
		/// 用户所在的单位的名称。
		/// </summary>
		[JsonProperty("JGMC")]
		public string Orgnization { get; set; }

		/// <summary>
		/// 表示用户类型的代码。
		/// </summary>
		[JsonProperty("YHLX")]
		public int TypeCode { get; set; }

		/// <summary>
		/// 用户类型的描述。
		/// </summary>
		[JsonProperty("YHLXMC")]
		public string Type { get; set; }

		/// <summary>
		/// 用户的证件类型。
		/// </summary>
		[JsonProperty("ZJLX")]
		public int CertificateTypeCode { get; set; }

		/// <summary>
		/// 用户的证件类型描述。
		/// </summary>
		[JsonProperty("ZJLXMC")]
		public string CertificateType { get; set; }

		/// <summary>
		/// 用户的证件号码。
		/// </summary>
		[JsonProperty("ZJHM")]
		public string CertificateCode { get; set; }

		/// <summary>
		/// 用户的政治面貌代码。
		/// </summary>
		[JsonProperty("ZZMMDM")]
		public int PoliticalStatusCode { get; set; }

		/// <summary>
		/// 用户的政治面貌。
		/// </summary>
		[JsonProperty("ZZMMMC")]
		public string PoliticalStatus { get; set; }

		/// <summary>
		/// 用户的国籍代码。
		/// </summary>
		[JsonProperty("GJ")]
		public int NationalityCode { get; set; }

		/// <summary>
		/// 用户的国籍。
		/// </summary>
		[JsonProperty("GJMC")]
		public string Nationality { get; set; }

		/// <summary>
		/// 用户的民族代码。
		/// </summary>
		[JsonProperty("MZDM")]
		public int EthnicityCode { get; set; }

		/// <summary>
		/// 用户的民族。
		/// </summary>
		[JsonProperty("MZMC")]
		public string Ethnicity { get; set; }

		/// <summary>
		/// 用户的出生日期。
		/// </summary>
		[JsonProperty("CSRQ")]
		[JsonConverter(typeof(UnixTimeStampToDateTimeConverter))]
		public DateTime Birthday { get; set; }

		/// <summary>
		/// 用户的联系电话。
		/// </summary>
		[JsonProperty("LXDH")]
		public string PhoneNumber { get; set; }

		/// <summary>
		/// 用户的电子邮箱地址。
		/// </summary>
		[JsonProperty("DZYX")]
		public string EmailAddress { get; set; }

		/// <summary>
		/// 用户的年级。
		/// </summary>
		[JsonProperty("NJ")]
		public int? Grade { get; set; }

		/// <summary>
		/// 用户的班级代码 。
		/// </summary>
		[JsonProperty("BH")]
		public int ClassCode { get; set; }

		/// <summary>
		/// 用户的班级。
		/// </summary>
		[JsonProperty("BJMC")]
		public string Class { get; set; }

		/// <summary>
		/// 用户的专业代码。
		/// </summary>
		[JsonProperty("ZYDM")]
		public int MajorCode { get; set; }

		/// <summary>
		/// 用户的专业。
		/// </summary>
		[JsonProperty("ZYMC")]
		public string Major { get; set; }

		/// <summary>
		/// 用户的学籍状态。
		/// </summary>
		[JsonProperty("XJZT")]
		public int? SchoolStatus { get; set; }

		/// <summary>
		/// 用户的出生地。
		/// </summary>
		[JsonProperty("SYD")]
		public string BirthPlace { get; set; }

		/// <summary>
		/// 用户的学制。
		/// </summary>
		[JsonProperty("XZ")]
		public double? LengthOfSchooling { get; set; }

		/// <summary>
		/// 用户的入学时间。
		/// </summary>
		[JsonProperty("RXSJ")]
		[JsonConverter(typeof(UnixTimeStampToDateTimeConverter))]
		public DateTime? EntranceTime { get; set; }

		/// <summary>
		/// 用户的职工状态。
		/// </summary>
		[JsonProperty("ZGZT")]
		public int? StaffStatus { get; set; }
	}
}