using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CC98.LogOn.Data
{
	/// <inheritdoc />
	/// <summary>
	/// 定义 CC98 用户标识数据库。
	/// </summary>
	public class CC98IdentityDbContext : DbContext
	{
		/// <inheritdoc />
		/// <summary>
		/// 初始化一个 <see cref="T:CC98.LogOn.Data.CC98IdentityDbContext" /> 对象的新实例。
		/// </summary>
		/// <param name="options">初始化数据库对象时提供的选项。</param>
		public CC98IdentityDbContext(DbContextOptions<CC98IdentityDbContext> options)
			: base(options)
		{

		}

		/// <summary>
		/// 获取或设置数据库中包含的所有用户的集合。
		/// </summary>
		public virtual DbSet<CC98User> Users { get; set; }

		/// <summary>
		/// 获取或设置数据库中包含的应用的集合。
		/// </summary>
		public virtual DbSet<App> Apps { get; set; }
	}

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
