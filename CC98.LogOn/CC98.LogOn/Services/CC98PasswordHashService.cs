using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace CC98.LogOn.Services
{
	/// <summary>
	/// 表示 CC98 网站使用的密码散列服务。
	/// </summary>
	public class CC98PasswordHashService
	{
		/// <summary>
		/// 获得给定密码的散列。
		/// </summary>
		/// <param name="password">要计算的密码。</param>
		/// <returns>密码的散列值。</returns>
		public string GetPasswordHash([NotNull] string password)
		{
			if (password == null)
			{
				throw new ArgumentNullException(nameof(password));
			}

			using (var md5 = MD5.Create())
			{
				var passwordBytes = Encoding.UTF8.GetBytes(password);
				var hashBytes = md5.ComputeHash(passwordBytes);

				// CC98 的 MD5 哈希实际上只用用到中间八位。
				var realBytes = hashBytes.Skip(4).Take(8).ToArray();

				return BytesToString(realBytes);
			}
		}

		/// <summary>
		/// 提供将字节序列转换为字符串形式的辅助方法。
		/// </summary>
		/// <param name="bytes">字节序列集合。</param>
		/// <returns>转换后的字符串序列。</returns>
		private static string BytesToString([NotNull]IReadOnlyCollection<byte> bytes)
		{
			var result = new StringBuilder(bytes.Count * 2);

			foreach (var b in bytes)
			{
				result.AppendFormat("{0:X2}", b);
			}

			return result.ToString();
		}
	}
}
