using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CC98.LogOn
{
	/// <summary>
	/// 多行文本框绑定标签帮助器。
	/// </summary>
	[HtmlTargetElement("textarea", Attributes = MultilineForAttributeName)]
	public class MultilineTextAreaTagHelper : TagHelper
	{
		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			if (!MultilineFor.Metadata.IsEnumerableType)
			{
				throw new InvalidOperationException($"The '{MultilineForAttributeName}' attribute can only accept values with type of '{typeof(IEnumerable).AssemblyQualifiedName}', while the type '{typeof(string).FullName}' is excluded.");
			}

			// Set name for control
			output.Attributes.SetAttribute("name", MultilineFor.Name);

			// Generate and set content
			var str = MultilineFor.Model is IEnumerable<object> values ? string.Join('\n', values) : string.Empty;
			output.PostContent.Append(str);
		}

		public const string MultilineForAttributeName = "asp-multiline-for";

		[HtmlAttributeName(MultilineForAttributeName)]
		public ModelExpression MultilineFor { get; set; }
	}
}
