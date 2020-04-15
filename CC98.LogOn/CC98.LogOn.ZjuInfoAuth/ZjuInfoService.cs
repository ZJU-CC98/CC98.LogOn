using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CC98.LogOn.ZjuInfoAuth
{
	/// <summary>
	/// 提供浙大通行证的额外接口服务。
	/// </summary>
	public class ZjuInfoService : IDisposable
	{
		/// <summary>
		/// 用于进行网络通讯的 HTTP 客户端对象。
		/// </summary>
		private HttpClient HttpClient { get; } = new HttpClient();
		/// <summary>
		/// 使用浙大通行证服务所需的应用名称。
		/// </summary>
		public string AppKey { get; set; }
		/// <summary>
		/// 使用浙大通行证服务所需的应用机密。
		/// </summary>
		public string AppSecret { get; set; }

		/// <summary>
		/// 通过后台接口向浙大通行证服务器检索给定用户的详细信息。
		/// </summary>
		/// <param name="userId">要检索的用户的标识（学工号）。</param>
		/// <param name="cancellationToken">用于取消操作的令牌。</param>
		/// <returns>表示异步操作的任务。操作结果包含 <paramref name="userId"/> 所给定的用户的详细信息。</returns>
		/// <remarks>该方法在浙大通行证后端具有 IP 白名单限定。</remarks>
		public async Task<ZjuInfoUserInfo> GetUserInfoAsync(string userId, CancellationToken cancellationToken = default)
		{

			// 操作的 URL 地址。
			const string uri = "https://zuinfo.zju.edu.cn/v2/getUserDetails.zf";

			var postData = new Dictionary<string, string>
			{
				["appkey"] = AppKey,
				["appsecret"] = AppSecret,
				["username"] = userId
			};

			var response = await HttpClient.PostAsync(uri, new FormUrlEncodedContent(postData), cancellationToken);
			response.EnsureSuccessStatusCode();

			await using var data = await response.Content.ReadAsStreamAsync();
			var responseObj = await JsonSerializer.DeserializeAsync<ZjuInfoUserDetailResponse>(data, cancellationToken: cancellationToken);

			return responseObj.UserInfo;
		}

		/// <summary>
		/// 通过后台结构修改浙大通行证密码。
		/// </summary>
		/// <param name="userId">要修改密码的用户的标识（学工号）。</param>
		/// <param name="newPassword">要修改的新密码。</param>
		/// <param name="userCertificateId">用户的身份证件号码。</param>
		/// <param name="cancellationToken">用于取消操作的令牌。</param>
		/// <returns>表示异步操作的任务。</returns>
		public async Task ModifyPasswordAsync(string userId, string newPassword, string userCertificateId, CancellationToken cancellationToken = default)
		{
			const string uri = "https://zuinfo.zju.edu.cn/v2/modifyPwd.zf";

			var postData = new Dictionary<string, string>
			{
				["appkey"] = AppKey,
				["appsecret"] = AppSecret,
				["username"] = userId,
				["newpassword"] = newPassword,
				["encryptStr"] = GetModifyPasswordEncryptString(userId, userCertificateId)
			};

			var response = await HttpClient.PostAsync(uri, new FormUrlEncodedContent(postData), cancellationToken);
			response.EnsureSuccessStatusCode();
		}

		/// <summary>
		/// 用于计算用户修改密码时加密串的方法。
		/// </summary>
		/// <param name="userId">用户的标识（学工号。）</param>
		/// <param name="userCertificateId">用户的证件号码。</param>
		/// <returns>修改密码时使用的加密串。</returns>
		private static string GetModifyPasswordEncryptString(string userId, string userCertificateId)
		{
			var str = userId + userCertificateId;

			using var md5 = MD5.Create();
			var hash = md5.ComputeHash(Encoding.ASCII.GetBytes(str));
			var sb = new StringBuilder();

			foreach (var b in hash)
			{
				sb.AppendFormat("{0:X2}", b);
			}

			return sb.ToString();
		}

		/// <inheritdoc />
		public void Dispose()
		{
			HttpClient?.Dispose();
		}
	}
}
