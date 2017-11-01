using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using CC98.LogOn.Data;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.EntityFrameworkCore;

namespace CC98.LogOn.Services
{
    /// <inheritdoc />
    /// <summary>
    /// 提供资源存储服务。
    /// </summary>
    public class AppResourceStore : IResourceStore
    {
        /// <summary>
        /// 静态资源对象。
        /// </summary>
        private static IdentityResource[] IdentityResourcesStore { get; } =
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        };

        public AppResourceStore(CC98IdentityDbContext dbContext)
        {
            DbContext = dbContext;
        }

        /// <summary>
        /// 数据库上下文对象。
        /// </summary>
        private CC98IdentityDbContext DbContext { get; }

        public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            return Task.FromResult(Enumerable.Empty<IdentityResource>());
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var names = scopeNames?.ToArray() ?? new string[0];

            return await (from i in DbContext.ApiResources
                          where names.Contains(i.Id)
                          select ConvertFromDatabaseItem(i)).ToArrayAsync();
        }

        public async Task<ApiResource> FindApiResourceAsync(string name)
        {
            var item = await (from i in DbContext.ApiResources
                              where i.Id == name
                              select i).FirstOrDefaultAsync();

            return item == null ? null : ConvertFromDatabaseItem(item);
        }

        public async Task<Resources> GetAllResourcesAsync()
        {
            var apiResources = await (from i in DbContext.ApiResources
                                      select ConvertFromDatabaseItem(i)).ToArrayAsync();

            return new Resources(Enumerable.Empty<IdentityResource>(), apiResources);
        }

        /// <summary>
        /// 从 <see cref="AppApiResource"/> 创建 <see cref="ApiResource"/> 对象。
        /// </summary>
        /// <param name="item"><see cref="AppApiResource"/> 对象。</param>
        /// <returns>创建的 <see cref="ApiResource"/> 对象。</returns>
        private static ApiResource ConvertFromDatabaseItem(AppApiResource item)
        {
            var result = new ApiResource(item.Id, item.DisplayName);
            result.ApiSecrets.Add(new Secret(item.Secret.ToString("D", CultureInfo.InvariantCulture)));

            //// 添加声明
            //foreach (var claim in item.Claims)
            //{
            //    result.UserClaims.Add(claim);
            //}

            return result;
        }
    }
}
