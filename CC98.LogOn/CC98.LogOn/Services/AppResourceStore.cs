using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using CC98.LogOn.Data;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace CC98.LogOn.Services
{
    /// <inheritdoc />
    /// <summary>
    /// 提供资源存储服务。
    /// </summary>
    public class AppResourceStore : IResourceStore
    {
        #region 辅助方法

        /// <summary>
        /// 将数据库中的 <see cref="AppScope"/> 对象转换为 <see cref="IdentityResource"/> 对象。
        /// </summary>
        /// <param name="item">要转换的 <see cref="AppScope"/> 对象。</param>
        /// <returns>转换后的 <see cref="IdentityResource"/> 对象。</returns>
        private static IdentityResource CreateIdentityResourceFromDatabaseItem(AppScope item)
        {
            return new IdentityResource
            {
                Name = item.Id,
                DisplayName = item.DisplayName,
                Description = item.Description,
                Required = item.IsRequired,
                Emphasize = item.IsImportant,
                UserClaims = item.UserClaims
            };
        }

        /// <summary>
        /// 将数据库中的 <see cref="AppScope"/> 对象转换为 <see cref="Scope"/> 对象。
        /// </summary>
        /// <param name="item">要转换的 <see cref="AppScope"/> 对象。</param>
        /// <returns>转换后的 <see cref="Scope"/> 对象。</returns>
        private static Scope CreateScopeFromDatabaseItem(AppScope item)
        {
            return new Scope
            {
                Name = item.Id,
                DisplayName = item.DisplayName,
                Description = item.Description,
                Required = item.IsRequired,
                Emphasize = item.IsImportant,
                UserClaims = item.UserClaims
            };
        }

        /// <summary>
        /// 将数据库中的 <see cref="AppApiResource"/> 对象转换为 <see cref="ApiResource"/> 对象。
        /// </summary>
        /// <param name="item">要转换的 <see cref="AppApiResource"/> 对象。</param>
        /// <returns>转换后的 <see cref="ApiResource"/> 对象。</returns>
        private static ApiResource CreateApiResourceFromDatabaseItem(AppApiResource item)
        {
            return new ApiResource
            {
                Name = item.Id,
                ApiSecrets = new[] { new Secret(item.Secret.ToString("D")) },
                DisplayName = item.DisplayName,
                Description = item.Description,
                Scopes = item.Scopes.Select(CreateScopeFromDatabaseItem).ToArray()
            };
        }

        #endregion

        [UsedImplicitly]
        public AppResourceStore(CC98IdentityDbContext dbContext)
        {
            DbContext = dbContext;
        }

        /// <summary>
        /// 获取数据库上下文对象。
        /// </summary>
        private CC98IdentityDbContext DbContext { get; }

        public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var result = from i in DbContext.AppScopes
                         where i.Type == ScopeType.Identity && scopeNames.Contains(i.Id)
                         select CreateIdentityResourceFromDatabaseItem(i);
            return await result.ToArrayAsync();

        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            return await (from i in DbContext.ApiResources
                            .Include(p => p.Scopes)
                          where scopeNames.Contains(i.Id)
                          select CreateApiResourceFromDatabaseItem(i)).ToArrayAsync();
        }

        public Task<ApiResource> FindApiResourceAsync(string name)
        {
            return (from i in DbContext.ApiResources
                    .Include(p => p.Scopes)
                    where i.Id == name
                    select CreateApiResourceFromDatabaseItem(i)).FirstOrDefaultAsync();
        }

        public async Task<Resources> GetAllResourcesAsync()
        {
            var apiResources = from i in DbContext.ApiResources
                                .Include(p => p.Scopes)
                               select CreateApiResourceFromDatabaseItem(i);

            var identityResources = from i in DbContext.AppScopes
                                    where i.Type == ScopeType.Identity
                                    select CreateIdentityResourceFromDatabaseItem(i);

            return new Resources(await identityResources.ToArrayAsync(), await apiResources.ToArrayAsync());
        }
    }
}
