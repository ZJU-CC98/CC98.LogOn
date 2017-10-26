using System;
using System.ComponentModel.DataAnnotations;

namespace CC98.LogOn.Data
{
	/// <summary>
	/// 定义一个 API 资源。
	/// </summary>
	public class AppApiResource
	{
		/// <summary>
		/// 获取或设置 API 资源的标识。
		/// </summary>
		[Key]
		[MaxLength(50)]
		public string Id { get; set; }

		/// <summary>
		/// 获取或设置资源的密钥。
		/// </summary>
		public Guid Secret { get; set; }

		/// <summary>
		/// 获取或设置资源的显示名称。
		/// </summary>
		[Required]
		public string DisplayName { get; set; }

		/// <summary>
		/// 获取或设置该资源的描述。
		/// </summary>
		public string Description { get; set; }
	}
}