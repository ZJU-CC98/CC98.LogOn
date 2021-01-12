using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using CC98.LogOn.Data;

using CommonPasswordsValidator.Internal;

using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CC98.LogOn.Services
{
	/// <summary>
	/// 提供密码检查服务。
	/// </summary>
	public class PasswordCheckService
	{
		private enum CharCategory
		{
			Digit,
			UpperCaseLetter,
			LowerCaseLetter,
			Others
		}

		/// <summary>
		/// 密码强度设定。
		/// </summary>
		private PasswordStrengthSetting PasswordStrengthSetting { get; }
		private PasswordLists PasswordLists { get; }

		public PasswordCheckService(IOptions<IdentityOptions> identityOptions, IOptions<AppSetting> appSettingOptions, ILoggerFactory loggerFactory)
		{
			PasswordStrengthSetting = appSettingOptions.Value.PasswordStrength;
			PasswordLists = new PasswordLists(identityOptions, loggerFactory.CreateLogger<PasswordLists>());
		}

		/// <summary>
		/// 检查密码是否太弱。
		/// </summary>
		/// <param name="password">要检查的密码。</param>
		/// <param name="cancellationToken">用于取消操作的令牌。</param>
		/// <returns>如果密码太弱，返回 <c>true</c>；否则返回 <c>false</c>。</returns>
		public async Task<bool> IsValidPasswordAsync(string password, CancellationToken cancellationToken = default)
		{
			static CharCategory GetCategory(char c)
			{
				if (char.IsDigit(c))
				{
					return CharCategory.Digit;
				}

				if (char.IsUpper(c))
				{
					return CharCategory.UpperCaseLetter;
				}

				if (char.IsLower(c))
				{
					return CharCategory.LowerCaseLetter;
				}

				return CharCategory.Others;
			}

			if (password.Length < PasswordStrengthSetting.MinLength)
			{
				return false;
			}

			// 种类类别
			var categoryCount = password.Select(GetCategory).Distinct().Count();

			if (categoryCount < PasswordStrengthSetting.MinCategoryCount)
			{
				return false;
			}

			// 常见密码
			if (PasswordLists.Top100000Passwords.Value.Contains(password))
			{
				return false;
			}

			return true;
		}
	}
}
