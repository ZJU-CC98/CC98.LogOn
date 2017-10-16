using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Newtonsoft.Json.Linq;

namespace CC98.LogOn.ZjuInfoAuth
{
	/// <summary>
	/// 提供对于浙大通行证产生的响应数据的声明操作。该类型为静态类型。
	/// </summary>
	public static class ZjuInfoClaimsExtensions
	{
		/// <summary>
		/// 用于配置浙大通行证相关声明信息的提取过程的默认方法。
		/// </summary>
		/// <param name="claimActions">包含所有声明操作的集合。</param>
		public static void ConfigureZjuInfoClaims(this ClaimActionCollection claimActions)
		{
			// 用于提取浙大通行证中特定属性的方法。
			string GetValueInAttributes(JObject objectData, string attributeName)
			{
				return objectData["attribute"].First.Value<string>(attributeName);
			}

			void MapAttributeAndDescription(string claimType, string valueType, string valueAttributeName, string descriptionAttributeName = null)
			{
				var mapValueFunc = new Func<JObject, string>(obj => GetValueInAttributes(obj, valueAttributeName));
				var mapDescriptionFunc = descriptionAttributeName != null
					? new Func<JObject, string>(obj => GetValueInAttributes(obj, descriptionAttributeName))
					: null;

				claimActions.MapCustomJsonWithDescription(claimType, valueType, mapValueFunc, mapDescriptionFunc);

			}


			claimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id", ClaimValueTypes.String);

			MapAttributeAndDescription(ClaimTypes.NameIdentifier, ClaimValueTypes.String, "CODE");
			MapAttributeAndDescription(ClaimTypes.Name, ClaimValueTypes.String, "XM");
			MapAttributeAndDescription(ClaimTypes.Gender, ClaimValueTypes.Integer, "XB", "XBMC");
			MapAttributeAndDescription(ClaimTypes.DateOfBirth, ClaimValueTypes.Date, "CSRQ");
			MapAttributeAndDescription(ClaimTypes.Email, ClaimValueTypes.Email, "DZYX");
			MapAttributeAndDescription(ClaimTypes.MobilePhone, ClaimValueTypes.String, "LXDH");

			MapAttributeAndDescription(ZjuInfoClaimTypes.Organization, ClaimValueTypes.Integer, "JGDM", "JGMC");
			MapAttributeAndDescription(ZjuInfoClaimTypes.UserType, ClaimValueTypes.Integer, "YHLX", "YHLXMC");
			MapAttributeAndDescription(ZjuInfoClaimTypes.CertificateType, ClaimValueTypes.Integer, "ZJLX", "ZJLXMC");
			MapAttributeAndDescription(ZjuInfoClaimTypes.CertificateId, ClaimValueTypes.String, "ZJHM");
			MapAttributeAndDescription(ZjuInfoClaimTypes.PoliticalStatus, ClaimValueTypes.Integer, "ZZMMDM", "ZZMMMC");
			MapAttributeAndDescription(ZjuInfoClaimTypes.Nationality, ClaimValueTypes.String, "GJ", "GJMC");
			MapAttributeAndDescription(ZjuInfoClaimTypes.Ethnicity, ClaimValueTypes.Integer, "MDZM", "MZMC");
			MapAttributeAndDescription(ZjuInfoClaimTypes.Grade, ClaimValueTypes.String, "NJ");
			MapAttributeAndDescription(ZjuInfoClaimTypes.Class, ClaimValueTypes.Integer, "BH", "BJMC");
			MapAttributeAndDescription(ZjuInfoClaimTypes.Major, ClaimValueTypes.Integer, "ZYDM", "ZYMC");
			MapAttributeAndDescription(ZjuInfoClaimTypes.StudentStatus, ClaimValueTypes.String, "XJZT");
			MapAttributeAndDescription(ZjuInfoClaimTypes.StaffStatus, ClaimValueTypes.String, "ZGZT");
			MapAttributeAndDescription(ZjuInfoClaimTypes.PlaceOfBirth, ClaimValueTypes.String, "SYD");
			MapAttributeAndDescription(ZjuInfoClaimTypes.LengthOfSchooling, ClaimValueTypes.Double, "XZ");
			MapAttributeAndDescription(ZjuInfoClaimTypes.EntranceTime, ClaimValueTypes.Date, "RXSJ");


		}
	}
}