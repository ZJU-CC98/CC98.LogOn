using System;
using System.ComponentModel.DataAnnotations;

namespace CC98.LogOn.ViewModels.Account
{
    /// <summary>
    /// 登录操作使用的视图模型。
    /// </summary>
    public class LogOnViewModel
    {
        /// <summary>
        /// 获取或设置登录的用户名。
        /// </summary>
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        /// <summary>
        /// 获取或设置登录的密码。
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }


        /// <summary>
        /// 获取或设置登录的有效期。
        /// </summary>
        [DataType(DataType.Duration)]
        [Display(Name = "ValidTime")]
        public TimeSpan? ValidTime { get; set; }
    }
}
