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
		public string ZjuAccountId { get; set; }

		/// <summary>
		/// 获取或设置锁定的时间。
		/// </summary>
		public DateTimeOffset Time { get; set; }
	}
}