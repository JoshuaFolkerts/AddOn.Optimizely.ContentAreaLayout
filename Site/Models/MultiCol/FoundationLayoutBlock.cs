using EPiServer.DataAnnotations;
using AddOn.Optimizely.ContentAreaLayout;
using AddOn.Optimizely.ContentAreaLayout.Block;
using AddOn.Optimizely.ContentAreaLayout.Context;

namespace Site.Models.MultiCol
{
    [ContentType(DisplayName = "FoundationLayoutBlock", GUID = "51224645-4175-4462-AFEA-8787704381A3", Description = "")]
    public class FoundationLayoutBlock : RenderingLayoutBlock
    {
        public override IRenderingContentAreaContext NewContext() =>
            new FoundationRenderingContext(new[] { 2, 8, 2 });
    }
}