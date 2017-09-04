using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace CC98.LogOn.Data
{
	/// <summary>
	/// ��ʾһ��Ӧ�õ���Ϣ��
	/// </summary>
	public class App
	{
		/// <summary>
		/// ��ȡ�����øö���ı�ʶ��
		/// </summary>
		[Key]
		public Guid Id { get; set; }

		/// <summary>
		/// ��ȡ�����øö���Ļ��ܡ�
		/// </summary>
		public Guid Secret { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "DisplayNameName")]
		public string DisplayName { get; set; }

		/// <summary>
		/// ��ȡ�����ø�Ӧ�õ�������
		/// </summary>
		[DataType(DataType.MultilineText)]
		[Display(Name = "DescriptionName")]
		public string Description { get; set; }

		/// <summary>
		/// ��ȡ�����ø�Ӧ����صĽ�����ַ��
		/// </summary>
		[DataType(DataType.Url)]
		[Display(Name = "WebPageUriName")]
		public string WebPageUri { get; set; }

		/// <summary>
		/// ��ȡ�����ø�Ӧ�õ� LOGOggi ��ͼ���ַ��
		/// </summary>
		[DataType(DataType.ImageUrl)]
		[Display(Name = "LogoUriName")]
		public string LogoUri { get; set; }

		/// <summary>
		/// ��ȡ������һ��ֵ��ָʾ�ÿͻ������Ƿ񱻽��á�
		/// </summary>
		public bool IsEnabled { get; set; }

		/// <summary>
		/// ��ȡ�����øÿͻ�������������ض����ַ�����
		/// </summary>
		[DataType(DataType.MultilineText)]
		[Column(nameof(RedirectUris))]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string RedirectUrisValue { get; set; }

		/// <summary>
		/// ��ȡ�����øÿͻ���������ض����ַ����ļ��ϡ�
		/// </summary>
		[IgnoreDataMember]
		[NotMapped]
		public string[] RedirectUris
		{
			get => RedirectUrisValue.SplitForStore();
			set => RedirectUrisValue = value.JoinForStore();
		}

		/// <summary>
		/// ��ȡ�����øÿͻ������������������ַ�����
		/// </summary>
		[DataType(DataType.MultilineText)]
		[Column(nameof(AllowedScopes))]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string AllowedScopesValue { get; set; }

		/// <summary>
		/// ��ȡ�����øÿͻ����������������
		/// </summary>
		[IgnoreDataMember]
		[NotMapped]
		public string[] AllowedScopes
		{
			get => AllowedScopesValue.SplitForStore();
			set => AllowedScopesValue = value.JoinForStore();
		}

		/// <summary>
		/// ��ȡ�����ø�Ӧ�õ����������ơ�
		/// </summary>
		public string OwnerUserName { get; set; }

		/// <summary>
		/// ��ȡ�����ø�Ӧ�õ�״̬��
		/// </summary>
		public AppState State { get; set; }

		/// <summary>
		/// ��ȡ�����ø�Ӧ���������Ȩ���͡�
		/// </summary>
		public AppGrantTypes GrantTypes { get; set; }

		/// <summary>
		/// ��ȡ�����øÿͻ��������ע�����ض��� URL ��ַ��
		/// </summary>
		[DataType(DataType.MultilineText)]
		[Column(nameof(PostLogoutRedirectUris))]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string PostLogoutRedirectUrisValue { get; set; }

		/// <summary>
		/// ��ȡ�����øÿͻ��������ע�����ض��� URL ��ַ��
		/// </summary>
		[IgnoreDataMember]
		[NotMapped]
		public string[] PostLogoutRedirectUris
		{
			get => PostLogoutRedirectUrisValue.SplitForStore();
			set => PostLogoutRedirectUrisValue = value.JoinForStore();
		}

		/// <summary>
		/// ��ȡ������Ӧ�õĴ���ʱ�䡣
		/// </summary>
		public DateTimeOffset CreateTime { get; set; }
	}

	/// <summary>
	/// ����Ӧ�õĻ�����Ϣ��
	/// </summary>
	public class AppSecretInfo
	{
		/// <summary>
		/// ��ȡ�����û��ܵ�ֵ��
		/// </summary>
		public string Value { get; set; }

		/// <summary>
		/// ��ȡ�����û��ܵ�������
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// ��ȡ�����û��ܵĴ���ʱ�䡣
		/// </summary>
		public DateTime CreateTime { get; set; }

		/// <summary>
		/// ��ȡ�����û��ܵĹ���ʱ�䡣
		/// </summary>
		public DateTime? ExpireTime { get; set; }
	}
}