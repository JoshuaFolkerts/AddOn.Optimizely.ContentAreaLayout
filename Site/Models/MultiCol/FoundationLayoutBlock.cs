using EPiServer.DataAnnotations;
using AddOn.Optimizely.ContentAreaLayout;
using AddOn.Optimizely.ContentAreaLayout.Block;
using AddOn.Optimizely.ContentAreaLayout.Context;
using System.ComponentModel.DataAnnotations;

namespace Site.Models.MultiCol
{
    [ContentType(DisplayName = "FoundationLayoutBlock", GUID = "51224645-4175-4462-AFEA-8787704381A3", Description = "")]
    public class FoundationLayoutBlock : RenderingLayoutBlock
    {
        [Display(Name = "Text")]
        public virtual string Text { get; set; }
        public override IRenderingContentAreaContext NewContext() =>
            new FoundationRenderingContext(new[] { 2, 8, 2 });
    }
}