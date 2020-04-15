using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CC98.LogOn.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CC98.LogOn.Services
{
	/// <summary>
	///     提供 CC98 数据的相关服务。
	/// </summary>
	public class CC98DataService
	{
		public CC98DataService(IOptions<AppSetting> appSetting, CC98IdentityDbContext dbContext)
		{
			DbContext = dbContext;
			AppSetting = appSetting.Value;
		}

		/// <summary>
		///     应用程序设置服务。
		/// </summary>
		private AppSetting AppSetting { get; }

		/// <summary>
		///     数据库上下文对象。
		/// </summary>
		private CC98IdentityDbContext DbContext { get; }

		/// <summary>
		///     获取单个浙大通行证允许激活的最大数量个数。
		/// </summary>
		public int MaxActivationCount => AppSetting.MaxCC98AccountPerZjuInfoId;

		/// <summary>
		///     获取给定浙大通行证账号激活的账号数量。
		/// </summary>
		/// <param name="zjuInfoId">浙大通行证账号。</param>
		/// <param name="cancellationToken">用于取消操作的令牌。</param>
		/// <returns>表示异步操作的方法。操作结果返回激活数量。</returns>
		public Task<int> GetActivatedUserCountAsync(string zjuInfoId, CancellationToken cancellationToken = default)
		{
			return (from i in DbContext.Users
					where string.Equals(i.RegisterZjuInfoId, zjuInfoId, StringComparison.OrdinalIgnoreCase)
					select i).CountAsync(cancellationToken);
		}

		/// <summary>
		///     获取给定浙大通行证账号激活的账号。
		/// </summary>
		/// <param name="zjuInfoId">浙大通行证账号。</param>
		/// <param name="cancellationToken">用于取消操作的令牌。</param>
		/// <returns>表示异步操作的方法。操作结果返回激活数量。</returns>
		public async Task<IEnumerable<CC98User>> GetActivatedUsersAsync(string zjuInfoId, CancellationToken cancellationToken = default)
		{
			return await (from i in DbContext.Users
						  where string.Equals(i.RegisterZjuInfoId, zjuInfoId, StringComparison.OrdinalIgnoreCase)
						  select i).ToArrayAsync(cancellationToken);
		}

		/// <summary>
		///     获取一个值，指示给定的账号是否还可以进行激活。
		/// </summary>
		/// <param name="zjuInfoId">浙大通行证账号。</param>
		/// <param name="cancellationToken">用于取消操作的令牌。</param>
		/// <returns>表示异步操作的方法。操作结果返回一个值，表示是否还能激活账号。</returns>
		public async Task<bool> CanActivateUsersAsync(string zjuInfoId, CancellationToken cancellationToken = default)
		{
			// 检测账户是否被锁定。
			var isLocked = await IsLockedAsync(zjuInfoId, cancellationToken);

			if (isLocked)
			{
				return false;
			}

			var count = await GetActivatedUserCountAsync(zjuInfoId, cancellationToken);
			return count < AppSetting.MaxCC98AccountPerZjuInfoId;
		}

		/// <summary>
		/// 获取一个值，指示给定的账户是否被锁定。
		/// </summary>
		/// <param name="zjuInfoId">浙大通行证信息。</param>
		/// <param name="cancellationToken">用于取消操作的令牌。</param>
		/// <returns>表示异步操作的方法。操作结果返回一个值，表示账户是否被锁定。</returns>
		public Task<bool> IsLockedAsync(string zjuInfoId, CancellationToken cancellationToken = default)
		{
			var item = 
				from i in DbContext.ZjuAccountLockDownRecords
					   where i.ZjuAccountId == zjuInfoId
					   select i;

			return item.AnyAsync(cancellationToken);
		}

		/// <summary>
		///     获取可用于网站使用的头像地址。
		/// </summary>
		/// <param name="portraitUri">数据库中记载的头像地址。</param>
		/// <returns>可用于网站的实际头像地址。</returns>
		public string GetPortraitUri(string portraitUri)
		{
			if (string.IsNullOrEmpty(portraitUri))
				return null;

			var baseUri = new Uri(AppSetting.BaseUriForPortrait);
			var finalUrl = new Uri(baseUri, portraitUri);
			return finalUrl.AbsoluteUri;
		}
	}
}