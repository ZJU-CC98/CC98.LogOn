using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace CC98.LogOn.Services
{
	/// <summary>
	/// 提供 CC98 数据的相关服务。
	/// </summary>
	public class CC98DataService
	{
		public CC98DataService(IOptions<AppSetting> appSetting)
		{
			AppSetting = appSetting.Value;
		}

		/// <summary>
		/// 应用程序设置服务。
		/// </summary>
		private AppSetting AppSetting { get; }

		/// <summary>
		/// 获取可用于网站使用的头像地址。
		/// </summary>
		/// <param name="portraitUri">数据库中记载的头像地址。</param>
		/// <returns>可用于网站的实际头像地址。</returns>
		public string GetPortraitUri(string portraitUri)
		{
			if (string.IsNullOrEmpty(portraitUri))
			{
				return null;
			}

			var baseUri = new Uri(AppSetting.BaseUriForPortrait);
			var finalUrl = new Uri(baseUri, portraitUri);
			return finalUrl.AbsoluteUri;
		}
	}
}
