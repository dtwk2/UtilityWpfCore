namespace UtilityWpf.Converter
{
    public class BooleanConverter : BaseConverter<bool>
    {
        protected BooleanConverter() : base(true, false)
        { }

        protected override bool Check(object value) => System.Convert.ToBoolean(value);
    }

    public class InverseBooleanConverter : BooleanConverter
    {
        protected override bool Check(object value) => !System.Convert.ToBoolean(value);

        public static InverseBooleanConverter Instance => new InverseBooleanConverter();
    }

    public class NullToInverseBooleanConverter : BooleanConverter
    {
        protected override bool Check(object value) => value != null;

        public static NullToInverseBooleanConverter Instance => new NullToInverseBooleanConverter();
    }

    public class NullOrEmptyToInverseBooleanConverter : BooleanConverter
    {
        protected override bool Check(object value) => !string.IsNullOrEmpty(value.ToString());

        public static NullOrEmptyToInverseBooleanConverter Instance => new NullOrEmptyToInverseBooleanConverter();
    }

    public class NullOrEmptyToBooleanConverter : BooleanConverter
    {
        protected override bool Check(object value) => string.IsNullOrEmpty(value.ToString());

        public static NullOrEmptyToBooleanConverter Instance => new NullOrEmptyToBooleanConverter();
    }
}