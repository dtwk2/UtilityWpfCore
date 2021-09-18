using System;
using System.Linq;
using System.Reflection;
using System.Windows.Markup;

namespace UtilityWpf.Markup
{
    [ContentProperty(nameof(Member))]
    public class NameOfExtension : MarkupExtension
    {
        public NameOfExtension()
        {
        }

        public Type? Type { get; set; }
        public string? Member { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));
            if (Type == null)
                throw new ArgumentNullException(nameof(Type));
            if (Member == null)
                throw new ArgumentNullException(nameof(Member));

            if (Type == null || string.IsNullOrEmpty(Member) || Member.Contains("."))
                throw new ArgumentException("Syntax for x:NameOf is Type={x:Type [className]} Member=[propertyName]");

            var propertyInfo = Type.GetRuntimeProperties().FirstOrDefault(pi => pi.Name == Member);
            if (propertyInfo != null)
                return Member;
            var fieldInfo = Type.GetRuntimeFields().FirstOrDefault(fi => fi.Name == Member);
            if (fieldInfo != null)
                return Member;
            var eventInfo = Type.GetRuntimeEvents().FirstOrDefault(ei => ei.Name == Member);
            if (eventInfo == null)
                throw new ArgumentException($"No property or field found for {Member} in {Type}");

            return Member;
        }
    }
}
