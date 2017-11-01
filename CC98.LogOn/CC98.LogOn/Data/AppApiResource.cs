using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace CC98.LogOn.Data
{
    /// <summary>
    /// 定义一个 API 资源。
    /// </summary>
    public class AppApiResource
    {
        /// <summary>
        /// 获取或设置 API 资源的标识。
        /// </summary>
        [Key]
        [MaxLength(50)]
        [Display(Name = "标识")]
        public string Id { get; set; }

        /// <summary>
        /// 获取或设置资源的密钥。
        /// </summary>
        [Display(Name = "机密")]
        public Guid Secret { get; set; }

        /// <summary>
        /// 获取或设置资源的显示名称。
        /// </summary>
        [Required]
        [Display(Name = "显示名称")]
        public string DisplayName { get; set; }

        /// <summary>
        /// 获取或设置该资源的描述。
        /// </summary>
        [Display(Name = "描述")]
        public string Description { get; set; }
        /// <summary>
        /// 获取或设置该对象关联的领域的集合。
        /// </summary>
        [InverseProperty(nameof(AppScope.Api))]
        public virtual IList<AppScope> Scopes { get; set; } = new Collection<AppScope>();
    }
}