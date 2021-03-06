﻿using System.Text.Json.Serialization;

namespace CC98.LogOn.ZjuInfoAuth
{
	/// <summary>
	///     表示浙大通行证进行身份验证时产生的结果。
	/// </summary>
	internal class ZjuInfoRestFulAuthenticationResult
	{
		/// <summary>
		///     错误代码。如果为 0 表示未发生错误。
		/// </summary>
		[JsonPropertyName("errorcode")]
		public int ErrorCode { get; set; }

		/// <summary>
		///     错误描述信息。
		/// </summary>
		[JsonPropertyName("errormsg")]
		public string ErrorMessage { get; set; }

		/// <summary>
		///     成功时返回的令牌对象。
		/// </summary>
		[JsonPropertyName("token")]
		public string Token { get; set; }
	}
}