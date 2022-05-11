using EPiServer.Core;

namespace RenderingLayoutProcessor.Block
{
    public abstract class RenderingLayoutBlock : BlockData, IRenderingLayoutBlock
    {
        public abstract IRenderingContentAreaContext NewContext();
    }
}