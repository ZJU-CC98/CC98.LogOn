using System.ComponentModel.DataAnnotations;

namespace CC98.LogOn.Data
{
	/// <summary>
	/// 表示一个用户声明。
	/// </summary>
	public class CC98UserClaim
	{
		/// <summary>
		/// 获取或设置用户声明对应的标识。
		/// </summary>
		[Key]
		[StringLength(50)]
		[Display(Name = "标识")]
		public string Id { get; set; }

		/// <summary>
		/// 获取或设置用户声明对应的显示名称。
		/// </summary>
		[Required]
		[Display(Name = "名称")]
		public string DisplayName { get; set; }

		/// <summary>
		/// 获取或设置用户声明的描述。
		/// </summary>
		[Display(Name = "描述")]
		public string Description { get; set; }
	}
}