using System;

namespace CC98.LogOn.Data
{
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