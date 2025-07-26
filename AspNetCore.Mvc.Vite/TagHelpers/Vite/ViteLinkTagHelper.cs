using AspNetCore.Mvc.Vite.Services.Vite;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.ComponentModel;

namespace AspNetCore.Mvc.Vite.TagHelpers.Vite;

[HtmlTargetElement("link", Attributes = ViteHrefAttribute)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class ViteLinkTagHelper(ILogger<ViteLinkTagHelper> logger, IWebHostEnvironment environment, IViteManifest manifest)
    : BaseViteTagHelper(logger, environment, manifest)
{
    private const string HrefAttribute = "href";
    private const string ViteHrefAttribute = $"vite-{HrefAttribute}";

    [HtmlAttributeName(ViteHrefAttribute)]
    public string ViteHref { get; set; } = string.Empty;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.Attributes.RemoveAll(HrefAttribute);
        output.Attributes.RemoveAll(ViteHrefAttribute);

        var filePath = GetFilePath(ViteHref);

        output.Attributes.SetAttribute(
            new TagHelperAttribute(HrefAttribute, filePath, HtmlAttributeValueStyle.DoubleQuotes)
        );
    }
}
