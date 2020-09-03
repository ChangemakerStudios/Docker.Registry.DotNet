namespace Docker.Registry.DotNet.Helpers
{
    internal static class QueryStringExtensions
    {
        /// <summary>
        ///     Adds the value to the query string if it's not null.
        /// </summary>
        /// <param name="queryString"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        internal static void AddIfNotNull<T>(this QueryString queryString, string key, T? value)
            where T : struct
        {
            if (value != null) queryString.Add(key, $"{value.Value}");
        }

        /// <summary>
        /// </summary>
        /// <param name="queryString"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        internal static void AddIfNotEmpty(this QueryString queryString, string key, string value)
        {
            if (!string.IsNullOrWhiteSpace(value)) queryString.Add(key, value);
        }
    }
}