namespace Docker.Registry.DotNet.Models
{
    public class ListImageTagsParameters
    {
        /// <summary>
        /// Limit the number of entries in each response. It not present, all entries will be returned
        /// </summary>
        public int? Number { get; set; }
    }
}