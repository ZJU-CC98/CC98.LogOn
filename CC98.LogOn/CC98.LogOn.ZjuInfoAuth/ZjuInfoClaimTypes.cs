namespace CC98.LogOn.ZjuInfoAuth
{
	/// <summary>
	/// 提供浙大通行证相关的声明类型字段。该类型为静态类型。
	/// </summary>
	public static class ZjuInfoClaimTypes
	{
		/// <summary>
		/// 所有字段的前缀。字段为常量。
		/// </summary>
		private const string Prefix = "zjuinfo.";

		/// <summary>
		/// 表示所在部门。
		/// </summary>
		public const string Organization = Prefix + "organization";

		/// <summary>
		/// 表示用户类型。
		/// </summary>
		public const string UserType = Prefix + "user-type";

		/// <summary>
		/// 表示证件类型。
		/// </summary>
		public const string CertificateType = Prefix + "certificate-type";

		/// <summary>
		/// 表示证件号码。
		/// </summary>
		public const string CertificateId = Prefix + "certificate-id";

		/// <summary>
		/// 表示国籍。
		/// </summary>
		public const string Nationality = Prefix + "nationality";

		/// <summary>
		/// 表示政治面貌。
		/// </summary>
		public const string PoliticalStatus = Prefix + "political-status";

		/// <summary>
		/// 表示民族。
		/// </summary>
		public const string Ethnicity = Prefix + "ethnicity";

		/// <summary>
		/// 表示年级。
		/// </summary>
		public const string Grade = Prefix + "grade";

		/// <summary>
		/// 表示班级。
		/// </summary>
		public const string Class = Prefix + "class";

		/// <summary>
		/// 表示专业。
		/// </summary>
		public const string Major = Prefix + "major";

		/// <summary>
		/// 表示学籍状态。
		/// </summary>
		public const string StudentStatus = Prefix + "student-status";

		/// <summary>
		/// 表示职工状态。
		/// </summary>
		public const string StaffStatus = Prefix + "staff-status";

		/// <summary>
		/// 表示出生地。
		/// </summary>
		public const string PlaceOfBirth = Prefix + "place-of-birth";

		/// <summary>
		/// 表示学制。
		/// </summary>
		public const string LengthOfSchooling = Prefix + "length-of-schooling";

		/// <summary>
		/// 表示入学时间。
		/// </summary>
		public const string EntranceTime = Prefix + "entrance-time";
	}
}