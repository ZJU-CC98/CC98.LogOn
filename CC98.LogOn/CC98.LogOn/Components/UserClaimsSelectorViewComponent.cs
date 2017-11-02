using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC98.LogOn.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CC98.LogOn.Components
{
	/// <summary>
	/// 提供用户声明的选择器。
	/// </summary>
	public class UserClaimsSelectorViewComponent : ViewComponent
	{
		public UserClaimsSelectorViewComponent(CC98IdentityDbContext dbContext)
		{
			DbContext = dbContext;
		}

		/// <summary>
		/// 数据库上下文对象。
		/// </summary>
		private CC98IdentityDbContext DbContext { get; }

		public async Task<IViewComponentResult> InvokeAsync(string inputName, IEnumerable<string> selectedItems)
		{
			ViewBag.InputName = inputName;

			ViewBag.AllItems = await (from i in DbContext.UserClaims
									  orderby i.Id
									  select i).ToArrayAsync();

			ViewBag.SelectedItems = selectedItems;

			return View();
		}
	}
}
