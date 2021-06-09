using EPiServer.Core;
using RenderingLayoutProcessor.Impl;

namespace RenderingLayoutProcessor.Models
{
    public abstract class RenderingLayoutBlock : BlockData, IRenderingLayoutBlock
    {
        public abstract IRenderingContentAreaContext NewContext();
    }
}