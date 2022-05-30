using EPiServer.Core;
using System;

namespace RenderingLayoutProcessor.Models
{
    public class BlockRenderingMetaData
    {
        public int Index { get; set; } = -1;
        public Guid ContentGuid { get; set; } = Guid.Empty;
        public ContentReference ContentLink { get; set; } = ContentReference.EmptyReference;
        public string Tag { get; set; } = string.Empty;
        public BlockRenderingMetaData ParentMetaData { get; set; }
    }
}