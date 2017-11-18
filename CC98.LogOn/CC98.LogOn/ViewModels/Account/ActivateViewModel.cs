using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CC98.LogOn.ViewModels.Account
{
    /// <summary>
    /// 激活时使用的数据模型。
    /// </summary>
    public class ActivateViewModel
    {
        /// <summary>
        /// 获取或设置关联的 CC98 用户名。
        /// </summary>
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "CC98 用户名")]
        public string CC98UserName { get; set; }

        /// <summary>
        /// 获取或设置关联的 CC98 密码。
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "CC98 密码")]
        public string CC98Password { get; set; }
    }
}
