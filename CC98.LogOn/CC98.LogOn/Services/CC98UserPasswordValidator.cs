using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC98.LogOn.Data;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace CC98.LogOn.Services
{
	/// <summary>
	/// 提供 CC98 账户的用户名密码验证。
	/// </summary>
	public class CC98UserPasswordValidator : IResourceOwnerPasswordValidator
	{
		[UsedImplicitly]
		public CC98UserPasswordValidator(CC98IdentityDbContext dbContext, CC98PasswordHashService hashService)
		{
			DbContext = dbContext;
			HashService = hashService;
		}

		/// <summary>
		/// 获取数据上下文对象。
		/// </summary>
		private CC98IdentityDbContext DbContext { get; }

		/// <summary>
		/// 密码散列服务。
		/// </summary>
		private CC98PasswordHashService HashService { get; }

		public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
		{
			var userName = context.UserName;
			var passwordHash = HashService.GetPasswordHash(context.Password);

			var user = await (from i in DbContext.Users
							  where i.Name == userName && i.PasswordHash == passwordHash
							  select i).FirstOrDefaultAsync();

			context.Result = user != null
				? new GrantValidationResult(user.Id.ToString("D"), "CC98", DateTime.Now)
				: new GrantValidationResult(TokenRequestErrors.InvalidGrant);
		}
	}
}
