using System.ComponentModel.DataAnnotations;

namespace CC98.LogOn.Data
{
	/// <summary>
	/// 表示一个 CC98 用户。
	/// </summary>
	public class CC98User
	{
		/// <summary>
		/// 获取或设置用户的标识。
		/// </summary>
		[Key]
		public int Id { get; set; }

		/// <summary>
		/// 获取或设置用户的名称。
		/// </summary>
		[Required]
		public string Name { get; set; }

		/// <summary>
		/// 获取或设置用户的密码的散列值。
		/// </summary>
		[Required]
		public string PasswordHash { get; set; }
	}
}