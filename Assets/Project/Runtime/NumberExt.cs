namespace Project.Runtime.Features
{
    public static class NumberExt
    {
        public static string FormatValue(this int value) => ((double)value).FormatValue();
        public static string FormatValue(this long value) => ((double)value).FormatValue();
        public static string FormatValue(this float value) => ((double)value).FormatValue();
        public static string FormatValue(this ulong value) => ((double)value).FormatValue();
        public static string FormatValue(this double value)
        {
            return value switch
            {
                >= 1000000 => (value / 1000000).ToString("#.#") + "М",
                >= 1000 => (value / 1000).ToString("#.#") + "К",
                _ => $"{(long)value}"
            };
        }
    }
}