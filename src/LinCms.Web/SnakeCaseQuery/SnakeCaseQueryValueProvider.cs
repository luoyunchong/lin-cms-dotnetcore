using System.Globalization;
using LinCms.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LinCms.SnakeCaseQuery
{
    /// <summary>
    /// 下划线写法（SnakeCase）
    /// </summary>
    public class SnakeCaseQueryValueProvider : QueryStringValueProvider
    {
        public SnakeCaseQueryValueProvider(
            BindingSource bindingSource,
            IQueryCollection values,
            CultureInfo culture)
            : base(bindingSource, values, culture)
        {
        }

        public override bool ContainsPrefix(string prefix)
        {
            return base.ContainsPrefix(prefix.ToSnakeCase());
        }

        public override ValueProviderResult GetValue(string key)
        {
            return base.GetValue(key.ToSnakeCase());
        }
    }
}
