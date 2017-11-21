using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;

namespace CC98.LogOn.ZjuInfoAuth
{
	/// <summary>
	///     �ṩ�������ͨ��֤��������Ӧ���ݵ�����������������Ϊ��̬���͡�
	/// </summary>
	public static class ZjuInfoClaimsExtensions
	{
		/// <summary>
		///     �����������ͨ��֤���������Ϣ����ȡ���̵�Ĭ�Ϸ�����
		/// </summary>
		/// <param name="claimActions">�����������������ļ��ϡ�</param>
		public static void ConfigureZjuInfoClaims(this ClaimActionCollection claimActions)
		{
			claimActions.MapJsonWithDescription(ClaimTypes.NameIdentifier, ClaimValueTypes.String, "CODE");
			claimActions.MapJsonWithDescription(ClaimTypes.Name, ClaimValueTypes.String, "XM");
			claimActions.MapJsonWithDescription(ClaimTypes.Gender, ClaimValueTypes.Integer, "XB", "XBMC");
			claimActions.MapJsonWithDescription(ClaimTypes.DateOfBirth, ClaimValueTypes.Date, "CSRQ");
			claimActions.MapJsonWithDescription(ClaimTypes.Email, ClaimValueTypes.Email, "DZYX");
			claimActions.MapJsonWithDescription(ClaimTypes.MobilePhone, ClaimValueTypes.String, "LXDH");

			claimActions.MapJsonWithDescription(ZjuInfoClaimTypes.Organization, ClaimValueTypes.Integer, "JGDM", "JGMC");
			claimActions.MapJsonWithDescription(ZjuInfoClaimTypes.UserType, ClaimValueTypes.Integer, "YHLX", "YHLXMC");
			claimActions.MapJsonWithDescription(ZjuInfoClaimTypes.CertificateType, ClaimValueTypes.Integer, "ZJLX", "ZJLXMC");
			claimActions.MapJsonWithDescription(ZjuInfoClaimTypes.CertificateId, ClaimValueTypes.String, "ZJHM");
			claimActions.MapJsonWithDescription(ZjuInfoClaimTypes.PoliticalStatus, ClaimValueTypes.Integer, "ZZMMDM", "ZZMMMC");
			claimActions.MapJsonWithDescription(ZjuInfoClaimTypes.Nationality, ClaimValueTypes.String, "GJ", "GJMC");
			claimActions.MapJsonWithDescription(ZjuInfoClaimTypes.Ethnicity, ClaimValueTypes.Integer, "MDZM", "MZMC");
			claimActions.MapJsonWithDescription(ZjuInfoClaimTypes.Grade, ClaimValueTypes.String, "NJ");
			claimActions.MapJsonWithDescription(ZjuInfoClaimTypes.Class, ClaimValueTypes.Integer, "BH", "BJMC");
			claimActions.MapJsonWithDescription(ZjuInfoClaimTypes.Major, ClaimValueTypes.Integer, "ZYDM", "ZYMC");
			claimActions.MapJsonWithDescription(ZjuInfoClaimTypes.StudentStatus, ClaimValueTypes.String, "XJZT");
			claimActions.MapJsonWithDescription(ZjuInfoClaimTypes.StaffStatus, ClaimValueTypes.String, "ZGZT");
			claimActions.MapJsonWithDescription(ZjuInfoClaimTypes.PlaceOfBirth, ClaimValueTypes.String, "SYD");
			claimActions.MapJsonWithDescription(ZjuInfoClaimTypes.LengthOfSchooling, ClaimValueTypes.Double, "XZ");
			claimActions.MapJsonWithDescription(ZjuInfoClaimTypes.EntranceTime, ClaimValueTypes.Date, "RXSJ");
		}
	}
}