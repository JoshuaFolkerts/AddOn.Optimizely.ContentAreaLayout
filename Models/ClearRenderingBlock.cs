using EPiServer.DataAnnotations;
using RenderingLayoutProcessor.Impl;

namespace RenderingLayoutProcessor.Models
{
    [ContentType(DisplayName = "Clear Rendering Layout", GroupName = "Layout", GUID = "b15b4a5b-2f99-47ae-aceb-c666eabaafd2", Description = "Clears multicolumn layout.")]
    public class ClearRenderingBlock : RenderingLayoutBlock
    {
        public override IRenderingContentAreaContext NewContext() =>
            ClearRenderingContentAreaContext.Instance;
    }
}