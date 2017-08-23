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

		/// <summary>
		/// 获取或设置数据库中记录的所有领域的集合。
		/// </summary>
		public virtual DbSet<AppScope> AppScopes { get; set; }
	}

	/// <summary>
	/// 表示一个应用的授权领域。
	/// </summary>
	public class AppScope
	{
		/// <summary>
		/// 获取或设置该领域的标识。
		/// </summary>
		[MaxLength(20)]
		[Key]
		public string Id { get; set; }

		/// <summary>
		/// 获取或设置该领域的显示名称。
		/// </summary>
		[Required]
		public string DisplayName { get; set; }

		/// <summary>
		/// 获取或设置该领域的描述。
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// 获取或设置该领域所属的相关使用范围。
		/// </summary>
		public string Region { get; set; }

		/// <summary>
		/// 获取或设置一个值，指示是否要在领域列表中隐藏该领域。
		/// </summary>
		public string IsHidden { get; set; }
	}
}
