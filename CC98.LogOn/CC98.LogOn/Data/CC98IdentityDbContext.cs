﻿using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
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
        /// 获取或设置数据库中记录的用户声明的集合。
        /// </summary>
        public virtual DbSet<CC98UserClaim> UserClaims { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CC98Role>().HasAlternateKey(i => i.Name);
            modelBuilder.Entity<CC98User>().HasAlternateKey(i => i.Name);

            modelBuilder.Entity<CC98UserRole>().HasKey(i => new { i.UserId, i.RoleId });
        }


        #region 存储过程

        /// <summary>
        /// 创建一个新用用户账户。
        /// </summary>
        /// <param name="userName">要创建的新用户名。</param>
        /// <param name="password">要创建的新用户的密码。</param>
        /// <param name="gender">账户的默认性别。</param>
        /// <param name="ipAddress">创建时的 IP 地址。</param>
        /// <param name="zjuInfoId">账户关联的浙大通行证。</param>
        /// <returns>表示异步操作的任务。</returns>
        public async Task<int> CreateAccountAsync(string userName, string password, Gender gender, string ipAddress, string zjuInfoId)
        {
            var connnection = (SqlConnection)Database.GetDbConnection();

            var command = connnection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "CreateUser";
            command.Parameters.AddWithValue("@userName", userName);
            command.Parameters.AddWithValue("@password", password);
            command.Parameters.AddWithValue("@gender", gender);
            command.Parameters.AddWithValue("@ipAddress", ipAddress);
            command.Parameters.AddWithValue("@zjuInfoId", (object)zjuInfoId ?? DBNull.Value);
            command.Parameters.Add("@userId", SqlDbType.Int).Direction = ParameterDirection.Output;

            await connnection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return (int)command.Parameters["@userId"].Value;


        }

        #endregion
    }
}
