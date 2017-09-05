using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace CC98.LogOn
{
	/// <summary>
	/// 提供辅助方法。该类型为静态类型。
	/// </summary>
    public static class Utility
    {
		/// <summary>
		/// 获取异常的最终错误消息。
		/// </summary>
		/// <param name="ex">异常对象。</param>
		/// <returns>异常对象的最终错误消息。</returns>
	    public static string GetMessage([NotNull] this Exception ex)
	    {
		    if (ex == null)
		    {
			    throw new ArgumentNullException(nameof(ex));
		    }

		    return ex.GetBaseException().Message;
	    }

    }
}
