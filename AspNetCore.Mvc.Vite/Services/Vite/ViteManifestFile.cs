using System.Text.Json.Serialization;

namespace AspNetCore.Mvc.Vite.Services.Vite;

public record ViteManifestFile
{
    [JsonPropertyName("file")]
    public string File { get; set; } = string.Empty;

    [JsonPropertyName("src")]
    public string Src { get; set; } = string.Empty;

    [JsonPropertyName("isEntry")]
    public bool IsEntry { get; set; }

    [JsonPropertyName("names")]
    public string[] Names { get; set; } = [];
}

