using AspNetCore.Mvc.Vite.Services.Vite;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.ComponentModel;

namespace AspNetCore.Mvc.Vite.TagHelpers.Vite;

[HtmlTargetElement("script", Attributes = ViteSrcAttribute)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class ViteScriptTagHelper(ILogger<ViteScriptTagHelper> logger, IWebHostEnvironment environment, IViteManifest manifest)
    : BaseViteTagHelper(logger, environment, manifest)
{
    private const string SrcAttribute = "src";
    private const string ViteSrcAttribute = $"vite-{SrcAttribute}";

    [HtmlAttributeName(ViteSrcAttribute)]
    public string ViteSrc { get; set; } = string.Empty;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.Attributes.RemoveAll(SrcAttribute);
        output.Attributes.RemoveAll(ViteSrcAttribute);

        var filePath = GetFilePath(ViteSrc);

        output.Attributes.SetAttribute(
            new TagHelperAttribute(SrcAttribute, filePath, HtmlAttributeValueStyle.DoubleQuotes)
        );
    }
}
