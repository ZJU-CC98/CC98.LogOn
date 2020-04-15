using System;
using System.Text.Json.Serialization;

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
		[JsonPropertyName("CODE")]
		public string Id { get; set; }

		/// <summary>
		/// 用户的姓名。
		/// </summary>
		[JsonPropertyName("XM")]
		public string Name { get; set; }

		/// <summary>
		/// 用户的性别代码。
		/// </summary>
		[JsonPropertyName("XB")]
		public ZjuInfoGender? GenderCode { get; set; }

		/// <summary>
		/// 用户性别。
		/// </summary>
		[JsonPropertyName("XBMC")]
		public string Gender { get; set; }

		/// <summary>
		/// 用户所在的单位的代码。
		/// </summary>
		[JsonPropertyName("JGDM")]
		public string OrganizationCode { get; set; }

		/// <summary>
		/// 用户所在的单位的名称。
		/// </summary>
		[JsonPropertyName("JGMC")]
		public string Orgnization { get; set; }

		/// <summary>
		/// 表示用户类型的代码。
		/// </summary>
		[JsonPropertyName("YHLX")]
		public string TypeCode { get; set; }

		/// <summary>
		/// 用户类型的描述。
		/// </summary>
		[JsonPropertyName("YHLXMC")]
		public string Type { get; set; }

		/// <summary>
		/// 用户的证件类型。
		/// </summary>
		[JsonPropertyName("ZJLX")]
		public string CertificateTypeCode { get; set; }

		/// <summary>
		/// 用户的证件类型描述。
		/// </summary>
		[JsonPropertyName("ZJLXMC")]
		public string CertificateType { get; set; }

		/// <summary>
		/// 用户的证件号码。
		/// </summary>
		[JsonPropertyName("ZJHM")]
		public string CertificateCode { get; set; }

		/// <summary>
		/// 用户的政治面貌代码。
		/// </summary>
		[JsonPropertyName("ZZMMDM")]
		public string PoliticalStatusCode { get; set; }

		/// <summary>
		/// 用户的政治面貌。
		/// </summary>
		[JsonPropertyName("ZZMMMC")]
		public string PoliticalStatus { get; set; }

		/// <summary>
		/// 用户的国籍代码。
		/// </summary>
		[JsonPropertyName("GJ")]
		public string NationalityCode { get; set; }

		/// <summary>
		/// 用户的国籍。
		/// </summary>
		[JsonPropertyName("GJMC")]
		public string Nationality { get; set; }

		/// <summary>
		/// 用户的民族代码。
		/// </summary>
		[JsonPropertyName("MZDM")]
		public string EthnicityCode { get; set; }

		/// <summary>
		/// 用户的民族。
		/// </summary>
		[JsonPropertyName("MZMC")]
		public string Ethnicity { get; set; }

		/// <summary>
		/// 用户的出生日期。
		/// </summary>
		[JsonPropertyName("CSRQ")]
		public DateTime? Birthday { get; set; }

		/// <summary>
		/// 用户的联系电话。
		/// </summary>
		[JsonPropertyName("LXDH")]
		public string PhoneNumber { get; set; }

		/// <summary>
		/// 用户的电子邮箱地址。
		/// </summary>
		[JsonPropertyName("DZYX")]
		public string EmailAddress { get; set; }

		/// <summary>
		/// 用户的年级。
		/// </summary>
		[JsonPropertyName("NJ")]
		public int? Grade { get; set; }

		/// <summary>
		/// 用户的班级代码 。
		/// </summary>
		[JsonPropertyName("BH")]
		public string ClassCode { get; set; }

		/// <summary>
		/// 用户的班级。
		/// </summary>
		[JsonPropertyName("BJMC")]
		public string Class { get; set; }

		/// <summary>
		/// 用户的专业代码。
		/// </summary>
		[JsonPropertyName("ZYDM")]
		public string MajorCode { get; set; }

		/// <summary>
		/// 用户的专业。
		/// </summary>
		[JsonPropertyName("ZYMC")]
		public string Major { get; set; }

		/// <summary>
		/// 用户的学籍状态代码。
		/// </summary>
		[JsonPropertyName("XJZT")]
		public string SchoolStatusCode { get; set; }

		/// <summary>
		/// 用户的学籍状态。
		/// </summary>
		[JsonPropertyName("XJZTMC")]
		public string SchoolStatus { get; set; }

		/// <summary>
		/// 用户的出生地。
		/// </summary>
		[JsonPropertyName("SYD")]
		public string BirthPlace { get; set; }

		/// <summary>
		/// 用户的学制。
		/// </summary>
		[JsonPropertyName("XZ")]
		public double? LengthOfSchooling { get; set; }

		/// <summary>
		/// 用户的入学时间。
		/// </summary>
		[JsonPropertyName("RXSJ")]
		public DateTime? EntranceTime { get; set; }

		/// <summary>
		/// 用户的职工状态代码。
		/// </summary>
		[JsonPropertyName("ZGZT")]
		public string StaffStatusCode { get; set; }

		/// <summary>
		/// 用户的职工状态。
		/// </summary>
		[JsonPropertyName("ZGZTMC")]
		public string StaffStatus { get; set; }
	}
}