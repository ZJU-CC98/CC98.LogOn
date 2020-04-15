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
			var action = new ZjuInfoJsonClaimAction();

			action.Mappings.Add(new ZjuInfoJsonClaimMapping(ClaimTypes.NameIdentifier, ClaimValueTypes.String, "CODE"));
			action.Mappings.Add(new ZjuInfoJsonClaimMapping(ClaimTypes.Name, ClaimValueTypes.String, "XM"));
			action.Mappings.Add(new ZjuInfoJsonClaimMapping(ClaimTypes.Gender, ClaimValueTypes.Integer, "XB", "XBMC"));
			action.Mappings.Add(new ZjuInfoJsonClaimMapping(ClaimTypes.DateOfBirth, ClaimValueTypes.Date, "CSRQ"));
			action.Mappings.Add(new ZjuInfoJsonClaimMapping(ClaimTypes.Email, ClaimValueTypes.Email, "DZYX"));
			action.Mappings.Add(new ZjuInfoJsonClaimMapping(ClaimTypes.MobilePhone, ClaimValueTypes.String, "LXDH"));
			action.Mappings.Add(new ZjuInfoJsonClaimMapping(ZjuInfoClaimTypes.Organization, ClaimValueTypes.Integer, "JGDM", "JGMC"));
			action.Mappings.Add(new ZjuInfoJsonClaimMapping(ZjuInfoClaimTypes.UserType, ClaimValueTypes.Integer, "YHLX", "YHLXMC"));
			action.Mappings.Add(new ZjuInfoJsonClaimMapping(ZjuInfoClaimTypes.CertificateType, ClaimValueTypes.Integer, "ZJLX", "ZJLXMC"));
			action.Mappings.Add(new ZjuInfoJsonClaimMapping(ZjuInfoClaimTypes.CertificateId, ClaimValueTypes.String, "ZJHM"));
			action.Mappings.Add(new ZjuInfoJsonClaimMapping(ZjuInfoClaimTypes.PoliticalStatus, ClaimValueTypes.Integer, "ZZMMDM", "ZZMMMC"));
			action.Mappings.Add(new ZjuInfoJsonClaimMapping(ZjuInfoClaimTypes.Nationality, ClaimValueTypes.String, "GJ", "GJMC"));
			action.Mappings.Add(new ZjuInfoJsonClaimMapping(ZjuInfoClaimTypes.Ethnicity, ClaimValueTypes.Integer, "MDZM", "MZMC"));
			action.Mappings.Add(new ZjuInfoJsonClaimMapping(ZjuInfoClaimTypes.Grade, ClaimValueTypes.String, "NJ"));
			action.Mappings.Add(new ZjuInfoJsonClaimMapping(ZjuInfoClaimTypes.Class, ClaimValueTypes.Integer, "BH", "BJMC"));
			action.Mappings.Add(new ZjuInfoJsonClaimMapping(ZjuInfoClaimTypes.Major, ClaimValueTypes.Integer, "ZYDM", "ZYMC"));
			action.Mappings.Add(new ZjuInfoJsonClaimMapping(ZjuInfoClaimTypes.StudentStatus, ClaimValueTypes.String, "XJZT"));
			action.Mappings.Add(new ZjuInfoJsonClaimMapping(ZjuInfoClaimTypes.StaffStatus, ClaimValueTypes.String, "ZGZT"));
			action.Mappings.Add(new ZjuInfoJsonClaimMapping(ZjuInfoClaimTypes.PlaceOfBirth, ClaimValueTypes.String, "SYD"));
			action.Mappings.Add(new ZjuInfoJsonClaimMapping(ZjuInfoClaimTypes.LengthOfSchooling, ClaimValueTypes.Double, "XZ"));
			action.Mappings.Add(new ZjuInfoJsonClaimMapping(ZjuInfoClaimTypes.EntranceTime, ClaimValueTypes.Date, "RXSJ"));

			claimActions.Add(action);
		}
	}
}