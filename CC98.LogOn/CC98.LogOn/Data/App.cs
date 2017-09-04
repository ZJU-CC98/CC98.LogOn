using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using JetBrains.Annotations;

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
			get => RedirectUrisValue.SplitForStore();
			set => RedirectUrisValue = value.JoinForStore();
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
			get => AllowedScopesValue.SplitForStore();
			set => AllowedScopesValue = value.JoinForStore();
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

		/// <summary>
		/// 获取或设置该客户端允许的注销后重定向 URL 地址。
		/// </summary>
		[DataType(DataType.MultilineText)]
		[Column(nameof(PostLogoutRedirectUris))]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string PostLogoutRedirectUrisValue { get; set; }

		/// <summary>
		/// 获取或设置该客户端允许的注销后重定向 URL 地址。
		/// </summary>
		[IgnoreDataMember]
		[NotMapped]
		public string[] PostLogoutRedirectUris
		{
			get => PostLogoutRedirectUrisValue.SplitForStore();
			set => PostLogoutRedirectUrisValue = value.JoinForStore();
		}

		/// <summary>
		/// 获取或设置应用的创建时间。
		/// </summary>
		public DateTimeOffset CreateTime { get; set; }
	}

	/// <summary>
	/// 定义应用的机密信息。
	/// </summary>
	public class AppSecretInfo
	{
		/// <summary>
		/// 获取或设置机密的值。
		/// </summary>
		public string Value { get; set; }

		/// <summary>
		/// 获取或设置机密的描述。
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// 获取或设置机密的创建时间。
		/// </summary>
		public DateTime CreateTime { get; set; }

		/// <summary>
		/// 获取或设置机密的过期时间。
		/// </summary>
		public DateTime? ExpireTime { get; set; }
	}
}