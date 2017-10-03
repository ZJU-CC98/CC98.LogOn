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

		public virtual DbSet<CC98Role> Roles { get; set; }

		public virtual DbSet<CC98UserRole> UserRoles { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<CC98Role>().HasAlternateKey(i => i.Name);
			modelBuilder.Entity<CC98User>().HasAlternateKey(i => i.Name);

			modelBuilder.Entity<CC98UserRole>().HasKey(i => new { i.UserId, i.RoleId });
		}
	}
}
