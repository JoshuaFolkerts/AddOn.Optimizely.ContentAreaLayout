using EPiServer.Core;

namespace AddOn.Optimizely.ContentAreaLayout.Block
{
    public abstract class RenderingLayoutBlock : BlockData, IRenderingLayoutBlock
    {
        public abstract IRenderingContentAreaContext NewContext();
    }
}