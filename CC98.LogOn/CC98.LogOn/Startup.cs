﻿using System;
using System.Globalization;
using System.Text.Json;

using CC98.LogOn.Data;
using CC98.LogOn.Services;
using CC98.LogOn.ZjuInfoAuth;

using JetBrains.Annotations;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Framework.DependencyInjection;

using Sakura.AspNetCore.Localization;
using Sakura.AspNetCore.Mvc;

namespace CC98.LogOn
{
	/// <summary>
	///     应用程序的启动类型。
	/// </summary>
	public class Startup
	{
		/// <summary>
		///     初始化一个 <see cref="Startup" /> 类型的新实例。
		/// </summary>
		/// <param name="configuration">应用程序的配置信息。</param>
		[UsedImplicitly]
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		/// <summary>
		///     获取应用程序的配置信息。
		/// </summary>
		private IConfiguration Configuration { get; }

		/// <summary>
		///     配置应用程序服务。
		/// </summary>
		/// <param name="services">应用程序的服务容器。</param>
		[UsedImplicitly]
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<CC98IdentityDbContext>(options =>
				options.UseSqlServer(Configuration["ConnectionStrings:CC98IdentityDbContext"]));

			// 添加本地化支持
			services.AddLocalization(options => { options.ResourcesPath = "Resources"; });


			// 添加 MVC 服务
			services.AddControllersWithViews(options =>
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


			// 添加身份验证服务
			services.AddAuthentication(IdentityConstants.ApplicationScheme)
				.AddCookie(IdentityConstants.ApplicationScheme)
				.AddCookie(IdentityConstants.ExternalScheme)
				// 浙大通行证身份验证
				.AddZjuInfo(options =>
				{
					options.SignInScheme = IdentityConstants.ExternalScheme;
					options.ClientId = Configuration["Authentication:ZjuInfo:ClientId"];
					options.ClientSecret = Configuration["Authentication:ZjuInfo:ClientSecret"];
					options.RedirectPath = new PathString("/LogOn");
				});

			services.ConfigureApplicationCookie(options =>
			{
				options.LoginPath = new PathString("/LogOn");
				options.LogoutPath = new PathString("/LogOff");
				options.AccessDeniedPath = new PathString("/AccessDenied");
				options.Cookie.HttpOnly = true;
				options.Cookie.SameSite = SameSiteMode.Lax;
				options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
			});

			// 用户角色
			services.AddAuthorization(options =>
			{
				options.AddPolicy(Policies.Admin, builder => { builder.RequireRole(Policies.Roles.Adiminstrators); });
				options.AddPolicy(Policies.QueryId,
					builder => { builder.RequireRole(Policies.Roles.Adiminstrators, Policies.Roles.QueryIdOperators); });
				options.AddPolicy(Policies.QueryAccount,
					builder => builder.RequireRole(Policies.Roles.Adiminstrators, Policies.Roles.QueryIdOperators,
						Policies.Roles.QueryAccountOperators));
			});

			services.AddExternalSignInManager();

			//  分页器
			services.AddBootstrapPagerGenerator(options => options.ConfigureDefault());

			// 加强的临时数据
			services.AddEnhancedTempData();

			//  消息服务
			services.AddOperationMessages();

			services.AddSession();
			services.AddMemoryCache();

			// 应用程序配置
			services.Configure<AppSetting>(Configuration.GetSection("AppSetting"));


			services.AddTransient<CC98DataService>();
			services.AddSingleton<CC98PasswordHashService>();
			services.AddSingleton<PasswordCheckService>();
		}

		/// <summary>
		///     配置应用程序设置。
		/// </summary>
		/// <param name="app">应用程序对象。</param>
		/// <param name="env">宿主环境对象。</param>
		[UsedImplicitly]
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{

			// 根据是否属于调试模式，分别启用错误处理
			if (env.IsDevelopment())
			{
				// 详细开发错误页面
				app.UseDeveloperExceptionPage();
			}
			else
			{
				// 通用错误页面
				app.UseExceptionHandler("/Home/Error");
				// 强制 HSTS
				app.UseHsts();
			}

			// 多语言版本支持
			app.UseRequestLocalization(new RequestLocalizationOptions
			{
				DefaultRequestCulture = new RequestCulture("zh-CN"),
				SupportedCultures =
				{
					new CultureInfo("zh-Hans-CN"),
					new CultureInfo("zh-Hans"),
					new CultureInfo("zh-CN"),
					new CultureInfo("zh"),
					new CultureInfo("en")
				},
				FallBackToParentCultures = true,
				FallBackToParentUICultures = true
			});

			// 会话服务
			app.UseSession();

			// 强制 HTTPS
			app.UseHttpsRedirection();

			// 允许访问静态文件
			app.UseStaticFiles();

			// 启用 HTTP 路由
			app.UseRouting();

			// 启用身份验证和授权服务
			app.UseAuthentication();
			app.UseAuthorization();

			// 配置路由映射
			app.UseEndpoints(endpoints =>
			{
				// MVC 路由
				endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
			});

		}
	}
}