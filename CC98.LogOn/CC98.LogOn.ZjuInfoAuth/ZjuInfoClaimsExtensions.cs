using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;

namespace CC98.LogOn.ZjuInfoAuth
{
	/// <summary>
	///     提供对于浙大通行证产生的响应数据的声明操作。该类型为静态类型。
	/// </summary>
	public static class ZjuInfoClaimsExtensions
	{
		/// <summary>
		///     用于配置浙大通行证相关声明信息的提取过程的默认方法。
		/// </summary>
		/// <param name="claimActions">包含所有声明操作的集合。</param>
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