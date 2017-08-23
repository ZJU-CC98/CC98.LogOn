using System;
using System.Threading.Tasks;
using CC98.LogOn.Data;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using JetBrains.Annotations;

namespace CC98.LogOn.Services
{
	/// <inheritdoc />
	/// <summary>
	/// 提供基于数据库的客户端信息检索。
	/// </summary>
	public class AppClientStore : IClientStore
	{
		[UsedImplicitly]
		public AppClientStore(CC98IdentityDbContext dbContext)
		{
			DbContext = dbContext;
		}

		/// <summary>
		/// 获取数据库上下文对象。
		/// </summary>
		public CC98IdentityDbContext DbContext { get; }

		public async Task<Client> FindClientByIdAsync(string clientId)
		{
			if (Guid.TryParse(clientId, out var id))
			{
				var item = await DbContext.Apps.FindAsync(id);
			}

			return null;
		}

		/// <summary>
		/// 将应用信息转换为客户端信息。
		/// </summary>
		/// <param name="app">应用信息。</param>
		/// <returns>客户端信息。</returns>
		public static Client ConvertAppToClient(App app)
		{
			return new Client
			{
				ClientId = app.Id.ToString("D"),
				ClientName = app.DisplayName,
				Enabled = app.IsEnabled,
				RedirectUris = app.RedirectUris
			};
		}
	}
}
