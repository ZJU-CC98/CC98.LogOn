using System.ComponentModel.DataAnnotations;

namespace CC98.LogOn.ViewModels.Account
{
    /// <summary>
    /// 表示用户注册时提供的信息。
    /// </summary>
    public class ResetPasswordViewModel
    {
        /// <summary>
        /// 获取或设置要重置的用户。
        /// </summary>
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "要重置的用户")]
        public string UserName { get; set; }

        /// <summary>
        /// 获取或设置账户的新密码。
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "新密码")]
        public string NewPassword { get; set; }

        /// <summary>
        /// 获取或设置新账户的确认新密码。
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword))]
        [Display(Name = "确认新密码")]
        public string ConfirmNewPassword { get; set; }
    }
}
