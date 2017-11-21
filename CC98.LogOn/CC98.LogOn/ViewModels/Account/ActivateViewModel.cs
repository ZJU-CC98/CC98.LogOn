using System.ComponentModel.DataAnnotations;

namespace CC98.LogOn.ViewModels.Account
{
	/// <summary>
	///     激活时使用的数据模型。
	/// </summary>
	public class ActivateViewModel
	{
		/// <summary>
		///     获取或设置关联的 CC98 用户名。
		/// </summary>
		[Required(ErrorMessage = "必须输入用户名")]
		[DataType(DataType.Text)]
		[Display(Name = "CC98 用户名")]
		public string CC98UserName { get; set; }

		/// <summary>
		///     获取或设置关联的 CC98 密码。
		/// </summary>
		[Required(ErrorMessage = "必须输入密码")]
		[DataType(DataType.Password)]
		[Display(Name = "CC98 密码")]
		public string CC98Password { get; set; }
	}
}