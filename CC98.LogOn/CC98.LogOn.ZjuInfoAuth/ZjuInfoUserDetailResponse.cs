using System.Text.Json.Serialization;

namespace CC98.LogOn.ZjuInfoAuth
{
	/// <summary>
	/// 定义浙大通行证详细信息接口返回的数据结构。
	/// </summary>
	internal class ZjuInfoUserDetailResponse
	{
		/// <summary>
		/// 获取或设置响应的用户信息。
		/// </summary>
		[JsonPropertyName("userinfo")]
		public ZjuInfoUserInfo UserInfo { get; set; }
	}
}