using System.ComponentModel.DataAnnotations;

namespace CC98.LogOn.Data
{
	/// <summary>
	/// ����Ӧ�õ����͡�
	/// </summary>
	public enum AppState
	{
		/// <summary>
		/// ����Ӧ�á�����Ӧ�þ������з���Ȩ�޵�������Ȩʱ�������ơ�
		/// </summary>
		[Display(Name = "δ���")]
		TestOnly = 0,
		/// <summary>
		/// ͨ�������֤����ͨӦ�á�
		/// </summary>
		[Display(Name = "�����")]
		Normal,
		/// <summary>
		/// �����εĿ�����Ӧ�á�������Ӧ������ʹ�ý�ΪΣ�յ���Ȩ���͡�
		/// </summary>
		[Display(Name = "������")]
		Trusted,
		/// <summary>
		/// Ӧ�ñ��������޷�ʹ�á�
		/// </summary>
		[Display(Name = "�ѵ���")]
		Revoked
	}
}