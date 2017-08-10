using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CC98.LogOn.Data;
using CC98.LogOn.Services;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Test;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sakura.AspNetCore.Localization;

namespace CC98.LogOn
{
	/// <summary>
	/// 应用程序的启动类型。
	/// </summary>
	public class Startup
	{
		/// <summary>
		/// 初始化一个 <see cref="Startup"/> 类型的新实例。
		/// </summary>
		/// <param name="env">应用程序的宿主环境。</param>
		[UsedImplicitly]
		public Startup(IHostingEnvironment env)
		{
			// 加载应用程序配置
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath) // 配置文件基础路径
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // 基本配置
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true) // 特性环境配置
				.AddEnvironmentVariables();

			// 在开发环境中导入用户机密设置
			if (env.IsDevelopment())
			{
				builder.AddUserSecrets<Startup>();
			}

			// 生成配置文件
			Configuration = builder.Build();
		}

		/// <summary>
		/// 获取应用程序的配置信息。
		/// </summary>
		private IConfigurationRoot Configuration { get; }

		/// <summary>
		/// 配置应用程序服务。
		/// </summary>
		/// <param name="services">应用程序的服务容器。</param>
		[UsedImplicitly]
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<CC98IdentityDbContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:CC98IdentityDbContext"]));


			// 添加本地化支持
			services.AddLocalization(options =>
			{
				options.ResourcesPath = "Resources";
			});


			// 添加 MVC 服务
			services.AddMvc()
				.AddDataAnnotationsLocalization() // 数据批注本地化
				.AddViewLocalization(LanguageViewLocationExpanderFormat.SubFolder); // 视图本地化

			// 动态本地化服务
			services.AddDynamicLocalizer();

			// CC98 密码散列服务
			services.AddSingleton<CC98PasswordHashService>();

			services.AddTransient<AppClientStore>();

			var resources = new List<IdentityResource>
			{
				new IdentityResources.OpenId(),
				new IdentityResources.Profile()
			};

			// 添加 IdentityServer 服务
			services.AddIdentityServer(options =>
				{
					options.UserInteraction.LoginUrl = "/Account/LogOn";
					options.UserInteraction.LogoutUrl = "/Account/LogOff";
					options.UserInteraction.ConsentUrl = "/Authorize";
				})
				.AddInMemoryCaching()
				.AddTemporarySigningCredential()
				.AddClientStoreCache<AppClientStore>()
				.AddTestUsers(new List<TestUser>())
				.AddInMemoryApiResources(new List<ApiResource>())
				.AddInMemoryIdentityResources(resources);
		}

		/// <summary>
		/// 配置应用程序设置。
		/// </summary>
		/// <param name="app">应用程序对象。</param>
		/// <param name="env">宿主环境对象。</param>
		/// <param name="loggerFactory">日志工厂对象。</param>
		[UsedImplicitly]
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			// 添加控制台输出
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));

			// 添加调试窗口输出
			loggerFactory.AddDebug();

			// 根据是否属于调试模式，分别启用错误处理
			if (env.IsDevelopment())
			{
				// 详细开发错误页面
				app.UseDeveloperExceptionPage();
				// 浏览器同步功能
				app.UseBrowserLink();
			}
			else
			{
				// 通用错误页面
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseRequestLocalization(new RequestLocalizationOptions
			{
				DefaultRequestCulture = new RequestCulture("zh-Hans-CN"),
				SupportedCultures = { new CultureInfo("zh-Hans-CN"), new CultureInfo("zh-Hans"), new CultureInfo("zh"), new CultureInfo("en") },
				FallBackToParentCultures = true,
				FallBackToParentUICultures = true
			});

			app.UseIdentityServer();

			// 允许访问静态文件
			app.UseStaticFiles();

			app.UseCookieAuthentication(new CookieAuthenticationOptions
			{
				AuthenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme,
				CookieHttpOnly = true,
				CookieSecure = CookieSecurePolicy.None,
				LoginPath = new PathString("/Account/LogOn"),
				LogoutPath = new PathString("/Account/LogOff"),
				AutomaticChallenge = true
			});

			app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
			{
				AuthenticationScheme = OpenIdConnectDefaults.AuthenticationScheme,
				SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme,
				Authority = "http://localhost:5000",
				RequireHttpsMetadata = false,
				ClientId = "mvc",
				SaveTokens = true
			});

			// MVC 路由配置
			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
