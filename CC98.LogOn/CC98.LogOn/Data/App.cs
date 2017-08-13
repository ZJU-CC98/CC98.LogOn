using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using IdentityServer4.Models;

namespace CC98.LogOn.Data
{
	/// <summary>
	/// 表示一个应用的信息。
	/// </summary>
	public class App
	{
		/// <summary>
		/// 获取或设置该对象的标识。
		/// </summary>
		[Key]
		public Guid Id { get; set; }

		/// <summary>
		/// 获取或设置该对象的机密。
		/// </summary>
		public Guid Secret { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "DisplayNameName")]
		public string DisplayName { get; set; }

		/// <summary>
		/// 获取或设置该应用的描述。
		/// </summary>
		[DataType(DataType.MultilineText)]
		[Display(Name = "DescriptionName")]
		public string Description { get; set; }

		/// <summary>
		/// 获取或设置该应用相关的介绍网址。
		/// </summary>
		[DataType(DataType.Url)]
		[Display(Name = "WebPageUriName")]
		public string WebPageUri { get; set; }

		/// <summary>
		/// 获取或设置该应用的 LOGOggi 的图标地址。
		/// </summary>
		[DataType(DataType.ImageUrl)]
		[Display(Name = "LogoUriName")]
		public string LogoUri { get; set; }

		/// <summary>
		/// 获取或设置一个值，指示该客户端你是否被禁用。
		/// </summary>
		public bool IsEnabled { get; set; }

		/// <summary>
		/// 获取或设置该客户端允许的所有重定向字符串。
		/// </summary>
		[DataType(DataType.MultilineText)]
		[Column(nameof(RedirectUris))]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string RedirectUrisValue { get; set; }

		/// <summary>
		/// 获取或设置该客户端允许的重定向字符串的集合。
		/// </summary>
		[IgnoreDataMember]
		[NotMapped]
		public string[] RedirectUris
		{
			get => RedirectUrisValue.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			set => RedirectUrisValue = string.Join("\n", value);
		}

		/// <summary>
		/// 获取或设置该客户端允许的所有领域的字符串。
		/// </summary>
		[DataType(DataType.MultilineText)]
		[Column(nameof(AllowedScopes))]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string AllowedScopesValue { get; set; }

		/// <summary>
		/// 获取或设置该客户端允许的所有领域。
		/// </summary>
		[IgnoreDataMember]
		[NotMapped]
		public string[] AllowedScopes
		{
			get => AllowedScopesValue.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			set => AllowedScopesValue = string.Join("\n", value);
		}

		/// <summary>
		/// 获取或设置该应用的所有者名称。
		/// </summary>
		public string OwnerUserName { get; set; }

		/// <summary>
		/// 获取或设置该应用的状态。
		/// </summary>
		public AppState State { get; set; }

		/// <summary>
		/// 获取或设置该应用允许的授权类型。
		/// </summary>
		public AppGrantTypes GrantTypes { get; set; }
	}


	/// <summary>
	/// 定义应用的类型。
	/// </summary>
	public enum AppState
	{
		/// <summary>
		/// 测试应用。测试应用具有所有访问权限但对于授权时间有限制。
		/// </summary>
		TestOnly = 0,
		/// <summary>
		/// 通过身份验证的普通应用。
		/// </summary>
		Normal,
		/// <summary>
		/// 受信任的开发者应用。受信任应用允许使用较为危险的授权类型。
		/// </summary>
		Trusted,
		/// <summary>
		/// 应用被吊销并无法使用。
		/// </summary>
		Revoked
	}

	/// <summary>
	/// 指定应用允许的授权类型。
	/// </summary>
	[Flags]
	public enum AppGrantTypes
	{
		/// <summary>
		/// 不允许任何授权。
		/// </summary>
		None = 0x0,
		/// <summary>
		/// 应用程序授权码验证。
		/// </summary>
		AuthorizationCode = 0x1,
		/// <summary>
		/// 隐式授权验证。
		/// </summary>
		Implicit = 0x2,
		/// <summary>
		/// 客户端凭据授权验证。
		/// </summary>
		ClientCredentials = 0x4,
		/// <summary>
		/// 混合授权验证。
		/// </summary>
		Hybrid = 0x8,
		/// <summary>
		/// 所有者密码授权验证。
		/// </summary>
		ResourceOwnerPassword = 0x10

	}
}