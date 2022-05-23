using EPiServer.Core;
using System;

namespace RenderingLayoutProcessor.Block
{
    public class BlockRenderingMetaData
    {
        public int BlockIndex { get; set; }
        public Guid BlockContentGuid { get; set; }
        public ContentReference BlockContentLink { get; set; }
    }
}