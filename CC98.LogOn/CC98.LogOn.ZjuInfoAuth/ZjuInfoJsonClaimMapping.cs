namespace CC98.LogOn.ZjuInfoAuth
{
	/// <summary>
	/// 定义浙大通行证 JSON 数据到标准用户账户声明的映射关系。
	/// </summary>
	public class ZjuInfoJsonClaimMapping
	{
		/// <summary>
		/// 声明类型。
		/// </summary>
		public string ClaimType { get; set; }

		/// <summary>
		/// 值类型。
		/// </summary>
		public string ValueType { get; set; }

		/// <summary>
		/// 声明的值使用的属性名。
		/// </summary>
		public string ClaimValueName { get; set; }

		/// <summary>
		/// 声明的描述使用的属性名。
		/// </summary>
		public string ClaimDescriptionName { get; set; }

		/// <summary>
		/// 初始化一个 <see cref="ZjuInfoJsonClaimMapping"/> 对象的新实例。
		/// </summary>
		public ZjuInfoJsonClaimMapping()
		{

		}

		/// <summary>
		/// 用给定的信息初始化一个 <see cref="ZjuInfoJsonClaimMapping"/> 对象的新实例。
		/// </summary>
		/// <param name="claimType">声明的类型。</param>
		/// <param name="valueType">声明的值的类型。</param>
		/// <param name="claimValueName">声明的值所在的属性名。</param>
		/// <param name="claimDescriptionName">声明的描述所在的属性名。</param>
		public ZjuInfoJsonClaimMapping(string claimType, string valueType, string claimValueName,
			string claimDescriptionName = null)
		{
			ClaimType = claimType;
			ValueType = valueType;
			ClaimValueName = claimValueName;
			ClaimDescriptionName = claimDescriptionName;
		}
	}
}