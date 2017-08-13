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
			get => RedirectUrisValue.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			set => RedirectUrisValue = string.Join("\n", value);
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
			get => AllowedScopesValue.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			set => AllowedScopesValue = string.Join("\n", value);
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
	}


	/// <summary>
	/// ����Ӧ�õ����͡�
	/// </summary>
	public enum AppState
	{
		/// <summary>
		/// ����Ӧ�á�����Ӧ�þ������з���Ȩ�޵�������Ȩʱ�������ơ�
		/// </summary>
		TestOnly = 0,
		/// <summary>
		/// ͨ�������֤����ͨӦ�á�
		/// </summary>
		Normal,
		/// <summary>
		/// �����εĿ�����Ӧ�á�������Ӧ������ʹ�ý�ΪΣ�յ���Ȩ���͡�
		/// </summary>
		Trusted,
		/// <summary>
		/// Ӧ�ñ��������޷�ʹ�á�
		/// </summary>
		Revoked
	}

	/// <summary>
	/// ָ��Ӧ���������Ȩ���͡�
	/// </summary>
	[Flags]
	public enum AppGrantTypes
	{
		/// <summary>
		/// �������κ���Ȩ��
		/// </summary>
		None = 0x0,
		/// <summary>
		/// Ӧ�ó�����Ȩ����֤��
		/// </summary>
		AuthorizationCode = 0x1,
		/// <summary>
		/// ��ʽ��Ȩ��֤��
		/// </summary>
		Implicit = 0x2,
		/// <summary>
		/// �ͻ���ƾ����Ȩ��֤��
		/// </summary>
		ClientCredentials = 0x4,
		/// <summary>
		/// �����Ȩ��֤��
		/// </summary>
		Hybrid = 0x8,
		/// <summary>
		/// ������������Ȩ��֤��
		/// </summary>
		ResourceOwnerPassword = 0x10

	}
}