using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CC98.LogOn.ZjuInfoAuth
{
	/// <summary>
	/// 提供浙大通行证的额外接口服务。
	/// </summary>
	public class ZjuInfoService
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
		public async Task<ZjuInfoUserInfo> GetUserInfoAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
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

			var data = await response.Content.ReadAsStringAsync();
			var responseObj = JsonConvert.DeserializeObject<ZjuInfoUserDetailResponse>(data);

			return responseObj.UserInfo;
		}

		public async Task ModifyPassswordAsync(string userId, string newPassword, string userCertificateId, CancellationToken cancellationToken = default(CancellationToken))
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

			using (var md5 = MD5.Create())
			{
				var hash = md5.ComputeHash(Encoding.ASCII.GetBytes(str));
				var sb = new StringBuilder();

				foreach (var b in hash)
				{
					sb.AppendFormat("{0:X2}", b);
				}

				return sb.ToString();
			}
		}
	}

	/// <summary>
	/// 定义浙大通行证详细信息接口返回的数据结构。
	/// </summary>
	internal class ZjuInfoUserDetailResponse
	{
		/// <summary>
		/// 获取或设置响应的用户信息。
		/// </summary>
		[JsonProperty("userinfo")]
		public ZjuInfoUserInfo UserInfo { get; set; }
	}

	/// <summary>
	/// 定义浙大通行证提供的用户信息。
	/// </summary>
	public class ZjuInfoUserInfo
	{
		/// <summary>
		/// 用户的唯一编号（学工号）。
		/// </summary>
		[JsonProperty("CODE")]
		public string Id { get; set; }

		/// <summary>
		/// 用户的姓名。
		/// </summary>
		[JsonProperty("XM")]
		public string Name { get; set; }

		/// <summary>
		/// 用户的性别代码。
		/// </summary>
		[JsonProperty("XB")]
		public ZjuInfoGender GenderCode { get; set; }

		/// <summary>
		/// 用户性别。
		/// </summary>
		[JsonProperty("XBMC")]
		public string Gender { get; set; }

		/// <summary>
		/// 用户所在的单位的代码。
		/// </summary>
		[JsonProperty("JGDM")]
		public int OrgnizationCode { get; set; }

		/// <summary>
		/// 用户所在的单位的名称。
		/// </summary>
		[JsonProperty("JGMC")]
		public string Orgnization { get; set; }

		/// <summary>
		/// 表示用户类型的代码。
		/// </summary>
		[JsonProperty("YHLX")]
		public int TypeCode { get; set; }

		/// <summary>
		/// 用户类型的描述。
		/// </summary>
		[JsonProperty("YHLXMC")]
		public string Type { get; set; }

		/// <summary>
		/// 用户的证件类型。
		/// </summary>
		[JsonProperty("ZJLX")]
		public int CertificateTypeCode { get; set; }

		/// <summary>
		/// 用户的证件类型描述。
		/// </summary>
		[JsonProperty("ZJLXMC")]
		public string CertificateType { get; set; }

		/// <summary>
		/// 用户的证件号码。
		/// </summary>
		[JsonProperty("ZJHM")]
		public string CertificateCode { get; set; }

		/// <summary>
		/// 用户的政治面貌代码。
		/// </summary>
		[JsonProperty("ZZMMDM")]
		public int PoliticalStatusCode { get; set; }

		/// <summary>
		/// 用户的政治面貌。
		/// </summary>
		[JsonProperty("ZZMMMC")]
		public string PoliticalStatus { get; set; }

		/// <summary>
		/// 用户的国籍代码。
		/// </summary>
		[JsonProperty("GJ")]
		public int NationalityCode { get; set; }

		/// <summary>
		/// 用户的国籍。
		/// </summary>
		[JsonProperty("GJMC")]
		public string Nationality { get; set; }

		/// <summary>
		/// 用户的民族代码。
		/// </summary>
		[JsonProperty("MZDM")]
		public int EthnicityCode { get; set; }

		/// <summary>
		/// 用户的民族。
		/// </summary>
		[JsonProperty("MZMC")]
		public string Ethnicity { get; set; }

		/// <summary>
		/// 用户的出生日期。
		/// </summary>
		[JsonProperty("CSRQ")]
		public DateTime Birthday { get; set; }

		/// <summary>
		/// 用户的联系电话。
		/// </summary>
		[JsonProperty("LXDH")]
		public string PhoneNumber { get; set; }

		/// <summary>
		/// 用户的电子邮箱地址。
		/// </summary>
		[JsonProperty("DZYX")]
		public string EmailAddress { get; set; }

		/// <summary>
		/// 用户的年级。
		/// </summary>
		[JsonProperty("Grade")]
		public int Grade { get; set; }

		/// <summary>
		/// 用户的班级代码 。
		/// </summary>
		[JsonProperty("BH")]
		public int ClassCode { get; set; }

		/// <summary>
		/// 用户的班级。
		/// </summary>
		[JsonProperty("BJMC")]
		public string Class { get; set; }

		/// <summary>
		/// 用户的专业代码。
		/// </summary>
		[JsonProperty("ZYDM")]
		public int MajorCode { get; set; }

		/// <summary>
		/// 用户的专业。
		/// </summary>
		[JsonProperty("ZYMC")]
		public string Major { get; set; }

		/// <summary>
		/// 用户的学籍状态。
		/// </summary>
		[JsonProperty("XJZT")]
		public int SchoolStatus { get; set; }

		/// <summary>
		/// 用户的出生地。
		/// </summary>
		[JsonProperty("SYD")]
		public string BirthPlace { get; set; }

		/// <summary>
		/// 用户的学制。
		/// </summary>
		[JsonProperty("XZ")]
		public double LengthOfSchooling { get; set; }

		/// <summary>
		/// 用户的入学时间。
		/// </summary>
		[JsonProperty("RXSJ")]
		public DateTime EntranceTime { get; set; }

		/// <summary>
		/// 用户的职工状态。
		/// </summary>
		[JsonProperty("ZGZT")]
		public int StaffStatus { get; set; }
	}

	/// <summary>
	/// 定义浙大通行证中的用户性别。
	/// </summary>
	public enum ZjuInfoGender
	{
		/// <summary>
		/// 未知性别。
		/// </summary>
		Unknown = 0,
		/// <summary>
		/// 男性。
		/// </summary>
		Male = 1,
		/// <summary>
		/// 女性。
		/// </summary>
		Female = 2,
		/// <summary>
		/// 特殊/未说明的性别。
		/// </summary>
		NotAppliable = 9
	}
}
