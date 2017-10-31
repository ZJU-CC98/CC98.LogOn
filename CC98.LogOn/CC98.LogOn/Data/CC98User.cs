using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CC98.LogOn.Data
{
	/// <summary>
	/// 表示一个 CC98 用户。
	/// </summary>
	[Table("User")]
	public class CC98User
	{
		/// <summary>
		/// 获取或设置用户的标识。
		/// </summary>
		[Key]
		[Column("UserId")]
		public int Id { get; set; }

		/// <summary>
		/// 获取或设置用户的名称。
		/// </summary>
		[Required]
		[Column("UserName")]
		public string Name { get; set; }

		/// <summary>
		/// 获取或设置用户的密码的散列值。
		/// </summary>
		[Required]
		[Column("UserPassword")]
		public string PasswordHash { get; set; }

		/// <summary>
		/// 获取或设置用户注册时提供的浙大通行证账号。
		/// </summary>
		[StringLength(30)]
		[Column("RegMail")]
		public string RegisterZjuInfoId { get; set; }

		/// <summary>
		/// 获取或设置一个值，指示该账户是否已经被验证。
		/// </summary>
		[Column("Verified")]
		public bool IsVerified { get; set; }

		/// <summary>
		/// 获取或设置用户的头像的 URL 地址。
		/// </summary>
		[Column("face")]
		public string PortraitUri { get; set; }

		/// <summary>
		/// 获取或设置该用户关联的角色信息的集合。
		/// </summary>
		[InverseProperty(nameof(CC98UserRole.User))]
		public virtual ICollection<CC98UserRole> Roles { get; set; } = new Collection<CC98UserRole>();
	}
}