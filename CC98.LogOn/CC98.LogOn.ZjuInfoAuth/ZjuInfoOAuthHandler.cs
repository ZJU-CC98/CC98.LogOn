using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CC98.LogOn.ZjuInfoAuth
{
	/// <summary>
	/// 提供浙大通行证身份验证服务。
	/// </summary>
	public class ZjuInfoOAuthHandler : OAuthHandler<ZjuInfoOAuthOptions>
	{
		/// <summary>
		/// 初始化一个 <see cref="ZjuInfoOAuthHandler"/> 对象的新实例。
		/// </summary>
		/// <param name="options">OAuth 验证选项。</param>
		/// <param name="logger">日志记录程序。</param>
		/// <param name="encoder">URL 编码程序。</param>
		/// <param name="clock">系统时钟服务。</param>
		[UsedImplicitly]
		public ZjuInfoOAuthHandler(IOptionsMonitor<ZjuInfoOAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
			: base(options, logger, encoder, clock)
		{
		}
	}
}
