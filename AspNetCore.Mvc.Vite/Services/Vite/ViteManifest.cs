using Microsoft.Extensions.FileProviders;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace AspNetCore.Mvc.Vite.Services.Vite;

public sealed class ViteManifest(ILogger<ViteManifest> logger, IWebHostEnvironment environment)
        : IViteManifest
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly IDictionary<string, ViteManifestFile> _manifestData = GetManifestData(environment, logger);

    public bool TryGetFile(string file, [NotNullWhen(true)] out ViteManifestFile? manifestFile)
        => _manifestData.TryGetValue(file, out manifestFile);

    private static IDictionary<string, ViteManifestFile> GetManifestData(IWebHostEnvironment environment, ILogger logger)
    {
        try
        {
            using var fileProvider = new PhysicalFileProvider(Path.Combine(environment.WebRootPath, ViteConstants.ManifestDirectory));
            var manifestFile = fileProvider.GetFileInfo(ViteConstants.ManifestName);

            if (manifestFile?.Exists != true)
            {
                throw new FileNotFoundException("The Vite manifest file was not found");
            }

            using var stream = manifestFile.CreateReadStream();
            return JsonSerializer.Deserialize<IDictionary<string, ViteManifestFile>>(stream, JsonOptions)!;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "{serviceName} - The following error occurred: {errorMessage}",
                nameof(ViteManifest),
                ex.Message);
        }

        return new Dictionary<string, ViteManifestFile>();
    }
}
