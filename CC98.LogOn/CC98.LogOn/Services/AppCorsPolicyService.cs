using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Services;

namespace CC98.LogOn.Services
{
	/// <inheritdoc />
	/// <summary>
	/// 提供应用程序的 CORS 服务。
	/// </summary>
	public class AppCorsPolicyService : ICorsPolicyService
	{
		/// <inheritdoc />
		public Task<bool> IsOriginAllowedAsync(string origin)
		{
			// 目前允许所有 CORS 地址
			return Task.FromResult(true);
		}
	}
}
