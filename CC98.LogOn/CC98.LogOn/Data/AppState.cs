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
}