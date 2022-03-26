using EPiServer.DataAnnotations;
using RenderingLayoutProcessor.Impl;
using RenderingLayoutProcessor.Models;

namespace Site.Models.MultiCol
{
    [ContentType(DisplayName = "FoundationLayoutBlock", GUID = "51224645-4175-4462-AFEA-8787704381A3", Description = "")]
    public class FoundationLayoutBlock : RenderingLayoutBlock
    {
        public override IRenderingContentAreaContext NewContext() =>
            new FoundationRenderingContext(new[] { 2, 8, 2 });
    }
}