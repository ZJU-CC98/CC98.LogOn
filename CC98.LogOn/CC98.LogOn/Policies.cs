using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CC98.LogOn
{
	/// <summary>
	/// 定义用户策略。该类型为静态类型。
	/// </summary>
    public static class Policies
	{
		/// <summary>
		/// 标识所有管理权限。该字段为常量。
		/// </summary>
		public const string Admin = nameof(Admin);

		/// <summary>
		/// 标识查询账号的权限。该字段为常量。
		/// </summary>
		public const string QueryAccount = nameof(QueryAccount);

		/// <summary>
		/// 标识查询学号的权限。该字段为常量。
		/// </summary>
		public const string QueryId = nameof(QueryId);

		/// <summary>
		/// 定义用户组。该类型为静态类型。
		/// </summary>
	    public static class Roles
		{
			/// <summary>
			/// 标识管理员组。该字段为常量。
			/// </summary>
			public const string Adiminstrators = nameof(Adiminstrators);

			/// <summary>
			/// 标识查询账号组。该字段为常量。
			/// </summary>
			public const string QueryAccountOperators = nameof(QueryAccountOperators);

			/// <summary>
			/// 标识查询学号组。该字段为常量。
			/// </summary>
			public const string QueryIdOperators = nameof(QueryIdOperators);
		}
    }
}
