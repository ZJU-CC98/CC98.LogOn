using JetBrains.Annotations;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace CC98.LogOn
{
	/// <summary>
	///     应用程序的主类型。
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
			CreateHostBuilder(args).Build().Run();
		}

		/// <summary>
		/// 创建宿主构建程序。
		/// </summary>
		/// <param name="args">应用程序的启动参数。</param>
		/// <returns>用于启动宿主的 <see cref="IHostBuilder"/> 对象。</returns>
		private static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}