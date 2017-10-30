using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CC98.LogOn.Data
{
	/// <summary>
	/// 表示一个应用的授权领域。
	/// </summary>
	public class AppScope
	{
		/// <summary>
		/// 获取或设置该领域的标识。
		/// </summary>
		[StringLength(20)]
		[Key]
		[RegularExpression(@"[\w-_\.]+")]
		[Display(Name = "标识")]
		public string Id { get; set; }

		/// <summary>
		/// 获取或设置该领域的显示名称。
		/// </summary>
		[Required]
		[Display(Name = "显示名称")]
		public string DisplayName { get; set; }

		/// <summary>
		/// 获取或设置该领域的描述。
		/// </summary>
		[Display(Name = "描述")]
		public string Description { get; set; }

		/// <summary>
		/// 获取或设置该领域所属的相关使用范围。
		/// </summary>
		[Display(Name = "分类")]
		public string Region { get; set; }

		/// <summary>
		/// 获取或设置一个值，指示是否要在领域列表中隐藏该领域。
		/// </summary>
		[Display(Name = "隐藏")]
		public bool IsHidden { get; set; }

        /// <summary>
        /// 获取或设置该领域关联到的 API 资源的集合。
        /// </summary>
        [InverseProperty(nameof(ApiResourceScope.Scope))]
        public virtual IList<ApiResourceScope> ApiResources { get; set; } = new Collection<ApiResourceScope>();
 	}
}