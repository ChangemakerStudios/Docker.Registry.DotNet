namespace Docker.Registry.DotNet.Models
{
    public class CatalogParameters
    {
        /// <summary>
        ///     Limit the number of entries in each response. It not present, all entries will be returned
        /// </summary>
        public int? Number { get; set; }

        /// <summary>
        ///     Result set will include values lexically after last.
        /// </summary>
        public int? Last { get; set; }
    }
}