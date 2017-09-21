using System.ComponentModel.DataAnnotations.Schema;

namespace CC98.LogOn.Data
{
	/// <summary>
	/// 表示用户和角色的集合。
	/// </summary>
	public class CC98UserRole
	{
		/// <summary>
		/// 用户的标识。
		/// </summary>
		public int UserId { get; set; }
		/// <summary>
		/// 角色的标识。
		/// </summary>
		public int RoleId { get; set; }

		/// <summary>
		/// 获取或设置该对象关联的用户。
		/// </summary>
		[ForeignKey(nameof(UserId))]
		public CC98User User { get; set; }

		/// <summary>
		/// 获取或设置该对象关联的角色。
		/// </summary>
		[ForeignKey(nameof(RoleId))]
		public CC98Role Role { get; set; }
	}
}