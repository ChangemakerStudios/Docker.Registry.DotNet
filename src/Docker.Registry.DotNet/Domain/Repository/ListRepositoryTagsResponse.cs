namespace Docker.Registry.DotNet.Domain.Repository;

public class ListRepositoryTagsResponse
{
    [JsonProperty("count")]
    public int Count { get; set; }

    [JsonProperty("next")]
    public string? Next { get; set; }

    [JsonProperty("previous")]
    public string? Previous { get; set; }

    [JsonProperty("results")]
    public List<RepositoryTag>? Tags { get; set; }
}