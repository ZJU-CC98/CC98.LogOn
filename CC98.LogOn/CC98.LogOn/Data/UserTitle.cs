using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CC98.LogOn.Data
{
	/// <summary>
	/// 定义用户的头衔信息。
	/// </summary>
	[Table("UserTitles")]
	public class UserTitle
	{
		/// <summary>
		/// 头衔的标识。
		/// </summary>
		[Key]
		public int Id { get; set; }
		/// <summary>
		/// 头衔的名称。
		/// </summary>
		[Required]
		[StringLength(50)]
		public string Name { get; set; }
		/// <summary>
		/// 头衔的描述。
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// 头衔的类型。
		/// </summary>
		public UserTitleType Type { get; set; }

		/// <summary>
		/// 头衔的图像地址。
		/// </summary>
		[Required]
		public string IconUri { get; set; }

		/// <summary>
		/// 头衔的排序权重。
		/// </summary>
		public int SortOrder { get; set; }
	}
}