using EPiServer.DataAnnotations;
using AddOn.Optimizely.ContentAreaLayout;
using AddOn.Optimizely.ContentAreaLayout.Block;
using AddOn.Optimizely.ContentAreaLayout.Context;
using System.ComponentModel.DataAnnotations;

namespace Site.Models.MultiCol
{
    [ContentType(DisplayName = "BootstrapLayoutBlock", GUID = "413b7a71-8490-4f22-bddc-77325c53424f", Description = "")]
    public class BootstrapLayoutBlock : RenderingLayoutBlock
    {
        [Display(Name = "Text")]
        public virtual string Text { get; set; }
        public override IRenderingContentAreaContext NewContext() =>
            new BootstrapRenderingContext(new[] { 2, 8, 2 });
    }
}