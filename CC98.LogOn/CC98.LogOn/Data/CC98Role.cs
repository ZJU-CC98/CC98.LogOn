using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CC98.LogOn.Data
{
	/// <summary>
	/// 定义一个 CC98 的角色。
	/// </summary>
	public class CC98Role
	{
		/// <summary>
		/// 获取或设置该项目的标识。
		/// </summary>
		[Key]
		[MaxLength(50)]
		public int Id { get; set; }

		/// <summary>
		/// 获取或设置该项目的名称。
		/// </summary>
		[Required]
		public string Name { get; set; }

		/// <summary>
		/// 获取或设置该项目的显示名称。
		/// </summary>
		public string DisplayName { get; set; }

		/// <summary>
		/// 获取或设置该项目的描述。
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// 获取或设置该角色所属的领域。
		/// </summary>
		public string Region { get; set; }

		/// <summary>
		/// 获取或设置该角色关联的用户的信息。
		/// </summary>
		[InverseProperty(nameof(CC98UserRole.Role))]
		public virtual ICollection<CC98UserRole> Users { get; set; } = new Collection<CC98UserRole>();
	}
}