using System.ComponentModel.DataAnnotations;
using CC98.LogOn.Data;

namespace CC98.LogOn.ViewModels.Account
{
	/// <summary>
	///     表示用户注册时提供的信息。
	/// </summary>
	public class RegisterViewModel
	{
		/// <summary>
		///     获取或设置要注册的用户名。
		/// </summary>
		[Required(ErrorMessage = "必须输入用户名")]
		[StringLength(10)]
		[RegularExpression(@"\w+", ErrorMessage = "用户名不能包含空白、标点符号和其它特殊字符")]
		[DataType(DataType.Text)]
		[Display(Name = "用户名")]
		public string UserName { get; set; }

		/// <summary>
		///     获取或设置新账户的密码。
		/// </summary>
		[Required(ErrorMessage = "必须输入密码")]
		[DataType(DataType.Password)]
		[Display(Name = "密码")]
		public string Password { get; set; }

		/// <summary>
		///     获取或设置新账户的确认密码。
		/// </summary>
		[Required(ErrorMessage = "必须确认密码")]
		[DataType(DataType.Password)]
		[Compare(nameof(Password), ErrorMessage = "密码和确认密码的内容不一致")]
		[Display(Name = "确认密码")]
		public string ConfirmPassword { get; set; }

		/// <summary>
		///     获取或设置新账户的性别。
		/// </summary>
		[Display(Name = "性别")]
		[Required(ErrorMessage = "必须选择一个性别")]
		public Gender Gender { get; set; }

		/// <summary>
		///     获取或设置一个值，指示是否要绑定到通行证。
		/// </summary>
		[Display(Name = "绑定到通行证")]
		public bool BindToZjuInfoId { get; set; }
	}
}