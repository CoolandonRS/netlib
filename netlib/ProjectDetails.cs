using System.Text.Json.Serialization;

// Only for json use, not needed
#pragma warning disable CS8618 

namespace netlib; 

public class ProjectDetails {
    [JsonInclude] public readonly string Ver;
    [JsonInclude] public readonly string Author;
    [JsonInclude] public readonly string Desc;
    [JsonInclude] public readonly string[] SupportedPlatforms;
}