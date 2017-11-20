using System;
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
		/// 获取或设置用户的性别。
		/// </summary>
		[Column("Sex")]
		public Gender Gender { get; set; }

		/// <summary>
		/// 获取或设置账号的注册时间。
		/// </summary>
		[Column("adddate")]
		public DateTime RegisterTime { get; set; }

		/// <summary>
		/// 获取或设置最后登录时间。
		/// </summary>
		[Column("lastlogin")]
		public DateTime LastLogOnTime { get; set; }

		/// <summary>
		/// 获取或设置文章数。
		/// </summary>
		[Column("article")]
		public int PostCount { get; set; }

		/// <summary>
		/// 获取或设置登录次数。
		/// </summary>
		[Column("logins")]
		public int LogOnCount { get; set; }
	}
}