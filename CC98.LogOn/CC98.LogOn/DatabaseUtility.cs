using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC98.LogOn.Data;
using Microsoft.EntityFrameworkCore;

namespace CC98.LogOn
{
    /// <summary>
    /// 提供数据库辅助方法。
    /// </summary>
    public static class DatabaseUtility
    {
        /// <summary>
        /// 获取给定浙大通行证账户绑定的账户个数。
        /// </summary>
        /// <param name="dbContext">就数据库上下文对象。</param>
        /// <param name="zjuInfoId">要检索的浙大通行证账号。</param>
        /// <returns>该账号绑定的账号个数。</returns>
        public static Task<int> GetActivatedUserCountAsync(this CC98IdentityDbContext dbContext, string zjuInfoId)
        {
            return (from i in dbContext.Users
                    where string.Equals(i.RegisterZjuInfoId, zjuInfoId, StringComparison.OrdinalIgnoreCase)
                    select i).CountAsync();
        }
    }
}
