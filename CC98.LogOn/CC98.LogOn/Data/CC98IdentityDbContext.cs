using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace CC98.LogOn.Data
{
	/// <inheritdoc />
	/// <summary>
	///     定义 CC98 用户标识数据库。
	/// </summary>
	public class CC98IdentityDbContext : DbContext
	{
		/// <inheritdoc />
		/// <summary>
		///     初始化一个 <see cref="T:CC98.LogOn.Data.CC98IdentityDbContext" /> 对象的新实例。
		/// </summary>
		/// <param name="options">初始化数据库对象时提供的选项。</param>
		public CC98IdentityDbContext(DbContextOptions<CC98IdentityDbContext> options)
			: base(options)
		{
		}

		/// <summary>
		///     获取或设置数据库中包含的所有用户的集合。
		/// </summary>
		public virtual DbSet<CC98User> Users { get; set; }

		/// <summary>
		/// 获取或设置数据库中包含的所有用户头衔的集合。
		/// </summary>
		public virtual DbSet<UserTitle> UserTitles { get; set; }

		/// <summary>
		/// 获取或设置数据库中包含的浙大通行证锁定信息的集合。
		/// </summary>
		public virtual DbSet<ZjuAccountLockDownRecord> ZjuAccountLockDownRecords { get; set; }


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<CC98User>().HasAlternateKey(i => i.Name);
		}


		#region 存储过程

		/// <summary>
		///     创建一个新用户账户。
		/// </summary>
		/// <param name="userName">要创建的新用户名。</param>
		/// <param name="password">要创建的新用户的密码。</param>
		/// <param name="gender">账户的默认性别。</param>
		/// <param name="ipAddress">创建时的 IP 地址。</param>
		/// <param name="zjuInfoId">账户关联的浙大通行证。</param>
		/// <returns>表示异步操作的任务。</returns>
		public async Task<int> CreateAccountAsync(string userName, string password, Gender gender, string ipAddress,
			string zjuInfoId)
		{
			var connection = (SqlConnection)Database.GetDbConnection();

			var command = connection.CreateCommand();
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = "CreateUser";
			command.Parameters.AddWithValue("@userName", userName);
			command.Parameters.AddWithValue("@password", password);
			command.Parameters.AddWithValue("@gender", gender);
			command.Parameters.AddWithValue("@ipAddress", ipAddress);
			command.Parameters.AddWithValue("@zjuInfoId", (object)zjuInfoId ?? DBNull.Value);
			command.Parameters.Add("@userId", SqlDbType.Int).Direction = ParameterDirection.Output;

			await connection.OpenAsync();
			await command.ExecuteNonQueryAsync();

			return (int)command.Parameters["@userId"].Value;
		}

		/// <summary>
		///     更新最近用户。
		/// </summary>
		/// <returns>表示异步操作的任务。</returns>
		public async Task BindUserAsync(int userId, string bindId, string userName, string password, string ip)
		{
			var connection = (SqlConnection)Database.GetDbConnection();

			var command = connection.CreateCommand();
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = "BindUser";
			command.Parameters.AddWithValue("@userId", userId);
			command.Parameters.AddWithValue("@bindId", bindId);
			command.Parameters.AddWithValue("@userName", userName);
			command.Parameters.AddWithValue("@bindPassword", password);
			command.Parameters.AddWithValue("@ip", ip);
			await connection.OpenAsync();
			await command.ExecuteNonQueryAsync();
		}

		/// <summary>
		/// 获取给定通行证账户关联的用户头衔集合。
		/// </summary>
		/// <param name="zjuInfoId"></param>
		/// <returns></returns>
		public async Task<IEnumerable<UserTitle>> GetZjuInfoRelatedUserTitlesAsync(string zjuInfoId)
		{
			if (string.IsNullOrEmpty(zjuInfoId))
			{
				throw new ArgumentNullException(nameof(zjuInfoId));
			}

			return await UserTitles.FromSqlInterpolated($"EXEC LoadZjuInfoIdUserTitles {zjuInfoId}").ToArrayAsync();
		}

		#endregion
	}
}