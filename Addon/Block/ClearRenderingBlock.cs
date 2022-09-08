using EPiServer.DataAnnotations;
using AddOn.Optimizely.ContentAreaLayout.Context;

namespace AddOn.Optimizely.ContentAreaLayout.Block
{
    [ContentType(DisplayName = "Clear Rendering Layout", GroupName = "Layout", GUID = "b15b4a5b-2f99-47ae-aceb-c666eabaafd2", Description = "Clears multicolumn layout.", AvailableInEditMode = false)]
    public class ClearRenderingBlock : RenderingLayoutBlock
    {
        public override IRenderingContentAreaContext NewContext() =>
            ClearRenderingContentAreaContext.Instance;
    }
}