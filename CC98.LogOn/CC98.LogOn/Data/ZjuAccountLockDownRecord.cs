using System;
using System.ComponentModel.DataAnnotations;

namespace CC98.LogOn.Data
{
	/// <summary>
	/// 浙大通行证锁定记录。
	/// </summary>
	public class ZjuAccountLockDownRecord
	{
		/// <summary>
		/// 获取或设置被锁定的浙大通行证。
		/// </summary>
		[Key]
		[StringLength(50)]
		[Required]
		[Display(Name = "通行证账号")]
		public string ZjuAccountId { get; set; }

		/// <summary>
		/// 获取或设置锁定的原因说明。
		/// </summary>
		[Display(Name = "锁定理由")]
		public string Reason { get; set; }

		/// <summary>
		/// 获取或设置锁定的时间。
		/// </summary>
		[Display(Name = "锁定时间")]
		public DateTimeOffset Time { get; set; }
	}
}