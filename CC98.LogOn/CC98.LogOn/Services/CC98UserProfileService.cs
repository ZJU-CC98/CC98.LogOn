using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC98.LogOn.Data;
using IdentityServer4.Models;
using IdentityServer4.Services;

namespace CC98.LogOn.Services
{
    public class CC98UserProfileService : IProfileService
    {
	    public CC98UserProfileService(CC98IdentityDbContext dbContext)
	    {
		    DbContext = dbContext;
	    }

	    private CC98IdentityDbContext DbContext { get; }

	    public Task GetProfileDataAsync(ProfileDataRequestContext context)
	    {
		    return Task.CompletedTask;
	    }

	    public Task IsActiveAsync(IsActiveContext context)
	    {
		    context.IsActive = true;
		    return Task.CompletedTask;
	    }
    }
}
