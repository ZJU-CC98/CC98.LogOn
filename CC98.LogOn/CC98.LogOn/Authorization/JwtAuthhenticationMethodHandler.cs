﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace CC98.LogOn.Authorization
{
	/// <inheritdoc />
	/// <summary>
	/// 提供对于 <see cref="T:CC98.LogOn.Authorization.JwtAuthenticationMethodRequirement" /> 需求的实现过程。
	/// </summary>
	public class JwtAuthhenticationMethodHandler : AuthorizationHandler<JwtAuthenticationMethodRequirement>
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, JwtAuthenticationMethodRequirement requirement)
		{
			var result = context.User.GetAuthenticationMethods().Any(i => i.Value == requirement.AuthenticationMethod);

			if (result)
			{
				context.Succeed(requirement);
			}

			return Task.CompletedTask;
		}
	}
}