using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

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
        /// 获取或设置一个值，指示是否要在领域列表中隐藏该领域。
        /// </summary>
        [Display(Name = "隐藏")]
        public bool IsHidden { get; set; }

        /// <summary>
        /// 获取或设置该领域需要使用的用户声明信息。
        /// </summary>
        [NotMapped]
        [IgnoreDataMember]
        public string[] UserClaims
        {
            get => UserClaimsValue.SplitForStore();
            set => UserClaimsValue = value.JoinForStore();
        }

        /// <summary>
        /// 用于存储 <see cref="UserClaims"/> 属性值的内部属性。不要直接使用该属性。
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Column(nameof(UserClaims))]
        public string UserClaimsValue { get; set; }

        /// <summary>
        /// 获取或设置领域的类型。
        /// </summary>
        [Display(Name ="类型")]
        public ScopeType Type { get; set; }

        /// <summary>
        /// 获取或设置该领域关联到的 API 的标识。
        /// </summary>
        [Display(Name = "API 资源")]
        public string ApiId { get; set; }

        /// <summary>
        /// 获取或设置该领域关联到的 API。仅当 <see cref="Type"/> 属性为 <see cref="ScopeType.Resource"/> 时可用。
        /// </summary>
        [ForeignKey(nameof(ApiId))]
        public AppApiResource Api { get; set; }
    }

    /// <summary>
    /// 定义领域的类型。
    /// </summary>
    public enum ScopeType
    {
        /// <summary>
        /// 资源领域。
        /// </summary>
        [Display(Name = "资源")]
        Resource,
        /// <summary>
        /// 标识领域。
        /// </summary>
        [Display(Name = "标识")]
        Identity
    }
}