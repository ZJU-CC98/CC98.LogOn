using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace CC98.LogOn.Authorization
{
	/// <inheritdoc />
	/// <summary>
	/// 定义用户标识必须包含给定的 JWT 验证架构类型。
	/// </summary>
    public class JwtAuthenticationMethodRequirement : IAuthorizationRequirement
    {
		/// <summary>
		/// 初始化一个 <see cref="JwtAuthenticationMethodRequirement"/> 对象的新实例。
		/// </summary>
		/// <param name="authenticationMethod">要包含的 JWT 验证架构类型。</param>
		public JwtAuthenticationMethodRequirement(string authenticationMethod)
	    {
		    AuthenticationMethod = authenticationMethod;
	    }

		/// <summary>
		/// 获取要包含的 JWT 验证架构类型。
		/// </summary>
	    public string AuthenticationMethod { get; }
    }
}
