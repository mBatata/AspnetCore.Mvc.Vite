using System.Diagnostics.CodeAnalysis;

namespace AspNetCore.Mvc.Vite.Services.Vite;

public interface IViteManifest
{
    bool TryGetFile(string file, [NotNullWhen(true)] out ViteManifestFile? manifestFile);
}
