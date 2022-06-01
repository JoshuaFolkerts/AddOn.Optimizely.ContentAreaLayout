using EPiServer.DataAnnotations;
using RenderingLayoutProcessor;
using RenderingLayoutProcessor.Block;
using RenderingLayoutProcessor.Context;
using RenderingLayoutProcessor.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Site.Models.MultiCol
{
    [ContentType(DisplayName = "No Framework Layout block", GUID = "8d4c928c-0601-41c2-9855-cc31f95e0b1c", Description = "")]
    public class NoFrameworkLayoutBlock : RenderingLayoutBlock
    {
        public override IRenderingContentAreaContext NewContext() =>
            new AttributeRenderingContext();

        [Display(
            Name = "Background",
            Description = "",
            Order = 100)]
        [CultureSpecific]
        [PropertyToHtmlAttribute]
        public virtual string Background { get; set; }

        [Display(
            Name = "Hidden",
            Description = "",
            Order = 200)]
        [CultureSpecific]
        [PropertyToHtmlAttribute("aria-hidden", true)]
        public virtual bool Hidden { get; set; }
    }
}