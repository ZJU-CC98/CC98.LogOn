using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CC98.LogOn
{
    /// <summary>
    /// 提供对于数组元素的检索绑定。
    /// </summary>
    [HtmlTargetElement("input", Attributes = ArrayForAttributeName)]
    public class ArrayCheckBoxTagHelper : TagHelper
    {
        /// <summary>
        /// 定义 <see cref="ArrayFor"/> 关联的 HTML 属性名称。该字段为常量。
        /// </summary>
        public const string ArrayForAttributeName = "asp-array-for";

        [HtmlAttributeName(ArrayForAttributeName)]
        public ModelExpression ArrayFor { get; set; }

        /// <summary>
        /// 定义 <see cref="ArrayFor"/> 关联的 HTML 属性名称。该字段为常量。
        /// </summary>
        public const string ArrayValueAttributeName = "asp-array-value";

        [HtmlAttributeName(ArrayValueAttributeName)]
        public object ArrayValue { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!ArrayFor.Metadata.IsEnumerableType)
            {
                throw new InvalidOperationException($"'{ArrayForAttributeName}' 属性只能绑定到集合属性。");
            }

            if (context.AllAttributes.TryGetAttribute("type", out var attr))
            {
                if (!string.Equals(attr.Value?.ToString(), "checkbox", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        $"'{ArrayForAttributeName}' 只能绑定到 'type' 设置为 'checkbox' 的 'input' 元素。");
                }
            }
            else
            {
                output.Attributes.SetAttribute("type", "checkbox");
            }

            if (context.AllAttributes.ContainsName("name") || context.AllAttributes.ContainsName("value"))
            {
                throw new InvalidOperationException($"'{ArrayForAttributeName}' 关联的 'input' 元素不能具有 'name' 或者 'value' 属性。");
            }

            output.Attributes.SetAttribute("name", ArrayFor.Name);
            output.Attributes.SetAttribute("value", ArrayValue?.ToString());

            var source = (IEnumerable<object>) ArrayFor.Model;
            if (source.Contains(ArrayValue))
            {
                output.Attributes.SetAttribute("checked", "checked");
            }
        }
    }
}
