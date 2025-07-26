using AspNetCore.Mvc.Vite.Services.Vite;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AspNetCore.Mvc.Vite.TagHelpers.Vite;

public abstract class BaseViteTagHelper(ILogger logger, IWebHostEnvironment environment, IViteManifest manifest)
    : TagHelper
{
    public override int Order => int.MinValue;

    protected string GetFilePath(string partialFilePath)
    {
        try
        {
            partialFilePath = partialFilePath.TrimStart('~', '/');

            if (!environment.IsDevelopment())
            {
                if (!manifest.TryGetFile(partialFilePath, out var viteManifestFile))
                {
                    throw new KeyNotFoundException($"The entry '{partialFilePath}' was not found in the Vite Manifrst file.");
                }

                return $"{ViteConstants.BaseDirectory}/{viteManifestFile.File}";
            }

            return partialFilePath;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "{tagHelper} - {methodName} The following error occured: {errorMessage}",
                GetType().Name,
                nameof(GetFilePath),
                ex.Message);
        }

        return string.Empty;
    }
}
