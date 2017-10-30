using JetBrains.Annotations;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace CC98.LogOn
{
	/// <summary>
	/// 应用程序的主类型。
	/// </summary>
	public static class Program
	{
		/// <summary>
		/// 应用程序的入口方法。
		/// </summary>
		/// <param name="args">应用程序的启动参数。</param>
		[UsedImplicitly]
		public static void Main(string[] args)
		{
			BuildWebHost(args).Run();
		}

		/// <summary>
		/// 构建应用程序宿主服务。
		/// </summary>
		/// <param name="args">应用程序的启动参数。</param>
		/// <returns>应用程序宿主对象。</returns>
		public static IWebHost BuildWebHost(string[] args)
		{
			return WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.Build();
		}
	}
}
