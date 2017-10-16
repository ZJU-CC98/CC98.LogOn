using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CC98.LogOn.ZjuInfoAuth
{
    public class ZjuInfoRestFulHandler : AuthenticationHandler<ZjuInfoRestFulAuthenticationOptions>
    {
	    /// <inheritdoc />
	    public ZjuInfoRestFulHandler(IOptionsMonitor<ZjuInfoRestFulAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
	    {
	    }

	    /// <inheritdoc />
	    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
	    {
		    throw new NotImplementedException();
	    }
    }

	public class ZjuInfoRestFulAuthenticationOptions : AuthenticationSchemeOptions
	{
		
	}
}
