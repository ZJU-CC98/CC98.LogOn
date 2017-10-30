using System.ComponentModel.DataAnnotations.Schema;
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

        /// <summary>
        /// 获取或设置数据库中记录的所有角色的集合。
        /// </summary>
        public virtual DbSet<CC98Role> Roles { get; set; }

        /// <summary>
        /// 获取或设置数据库中记录的用户和角色关系的集合。
        /// </summary>
        public virtual DbSet<CC98UserRole> UserRoles { get; set; }

        /// <summary>
        /// 获取或设置数据库中记录的 API 资源的集合。
        /// </summary>
        public virtual DbSet<AppApiResource> ApiResources { get; set; }

        /// <summary>
        /// 获取或设置数据库中记录的 API 资源和领域的关联。
        /// </summary>
        public virtual DbSet<ApiResourceScope> ApiResourceScopes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CC98Role>().HasAlternateKey(i => i.Name);
            modelBuilder.Entity<CC98User>().HasAlternateKey(i => i.Name);

            modelBuilder.Entity<CC98UserRole>().HasKey(i => new { i.UserId, i.RoleId });
            modelBuilder.Entity<ApiResourceScope>().HasKey(i => new { i.ApiId, i.ScopeId });
        }
    }

    /// <summary>
    /// 定义 API 资源需要的领域。
    /// </summary>
    public class ApiResourceScope
    {
        /// <summary>
        /// 获取或设置该对象关联到的 API 资源的标识。
        /// </summary>
        public string ApiId { get; set; }

        /// <summary>
        /// 获取或设置该对象关联到的 API 资源。
        /// </summary>
        [ForeignKey(nameof(ApiId))]
        public AppApiResource Api { get; set; }

        /// <summary>
        /// 获取或设置该对象关联到的领域的标识。
        /// </summary>
        public string ScopeId { get; set; }

        /// <summary>
        /// 获取或设置该对象关联到的领域。
        /// </summary>
        [ForeignKey(nameof(ScopeId))]
        public AppScope Scope { get; set; }

        /// <summary>
        /// 获取或设置一个值，指示该领域是否是必须的。
        /// </summary>
        public bool IsRequired { get; set; }
    }
}
