using System.Collections.Generic;
using System.Globalization;
using CC98.LogOn.Data;
using CC98.LogOn.Services;
using CC98.LogOn.ZjuInfoAuth;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Services;
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
using Microsoft.Framework.DependencyInjection;
using Sakura.AspNetCore.Localization;
using Sakura.AspNetCore.Mvc;

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
		/// <param name="configuration">应用程序的配置信息。</param>
		[UsedImplicitly]
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		/// <summary>
		/// 获取应用程序的配置信息。
		/// </summary>
		private IConfiguration Configuration { get; }

		/// <summary>
		/// 配置应用程序服务。
		/// </summary>
		/// <param name="services">应用程序的服务容器。</param>
		[UsedImplicitly]
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddApplicationInsightsTelemetry();

			services.AddDbContext<CC98IdentityDbContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:CC98IdentityDbContext"]));

			// 添加本地化支持
			services.AddLocalization(options =>
			{
				options.ResourcesPath = "Resources";
			});


			// 添加 MVC 服务
			services.AddMvc(options =>
				{
					options.EnableActionResultExceptionFilter(); // ActionResult 异常处理
					options.AddFlagsEnumModelBinderProvider(); // 标志枚举绑定
				})
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

			// 授权系统
			services.AddAuthorization(options =>
			{
				options.AddPolicy(Policies.AdminApps, builder =>
				{
					builder.RequireRole(Policies.Roles.Administrators, Policies.Roles.AppAdministrators);
				});

				options.AddPolicy(Policies.OperateApps, builder =>
				{
					builder.RequireRole(Policies.Roles.Administrators, Policies.Roles.AppAdministrators, Policies.Roles.AppOperators);
				});
			});

			// 添加身份验证服务
			services.AddAuthentication()
				// 基于 Cookie 的身份验证设置
				.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
				{
					options.Cookie.SameSite = SameSiteMode.Lax;
					options.Cookie.HttpOnly = true;
					options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

					options.LoginPath = new PathString("/Account/LogOn");
					options.LogoutPath = new PathString("/Account/LogOff");
				})
				// 浙大通行证身份验证
				.AddZjuInfo(options =>
				{
					options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
					options.ClientId = Configuration["Authentication:ZjuInfo:ClientId"];
					options.ClientSecret = Configuration["Authentication:ZjuInfo:ClientSecret"]; ;
				});

			// 添加 IdentityServer 服务
			services.AddIdentityServer(options =>
				{
					options.UserInteraction.LoginUrl = "/Account/LogOn";
					options.UserInteraction.LogoutUrl = "/Account/LogOff";
					options.UserInteraction.ConsentUrl = "/Account/Consent";
				})
				.AddInMemoryCaching()
				.AddClientStoreCache<AppClientStore>()
				.AddProfileService<CC98UserProfileService>()
				.AddResourceOwnerValidator<CC98UserPasswordValidator>()
				.AddInMemoryApiResources(new List<ApiResource>())
				.AddInMemoryIdentityResources(resources)
				.AddCorsPolicyService<AppCorsPolicyService>();

			services.AddExternalSignInManager();

			//  分页器
			services.AddBootstrapPagerGenerator(options => options.ConfigureDefault());

			// 加强的临时数据
			services.AddEnhancedTempData();

			//  消息服务
			services.AddOperationMessages();

			services.AddSession();
			services.AddMemoryCache();
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

			app.UseSession();

			// 允许访问静态文件
			app.UseStaticFiles();

			// 启用身份验证服务
			app.UseAuthentication();

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
