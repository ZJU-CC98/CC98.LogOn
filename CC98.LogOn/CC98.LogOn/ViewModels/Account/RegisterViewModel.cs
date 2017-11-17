using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CC98.LogOn.ViewModels.Account
{
    /// <summary>
    /// 表示用户注册时提供的信息。
    /// </summary>
    public class RegisterViewModel
    {
        /// <summary>
        /// 获取或设置要注册的用户名。
        /// </summary>
        [Required]
        [StringLength(10)]
        [RegularExpression(@"\w+")]
        [DataType(DataType.Text)]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        /// <summary>
        /// 获取或设置新账户的密码。
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        /// <summary>
        /// 获取或设置新账户的确认密码。
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        [Display(Name = "确认密码")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// 获取或设置一个值，指示是否要绑定到通行证。
        /// </summary>
        [Display(Name = "绑定到通行证")]
        public bool BindToZjuInfoId { get; set; }
    }
}
