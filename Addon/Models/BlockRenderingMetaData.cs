using EPiServer.Core;
using System;
using System.Collections.Generic;

namespace AddOn.Optimizely.ContentAreaLayout.Models
{
    public class BlockRenderingMetadata
    {
        public static readonly BlockRenderingMetadata Empty = new BlockRenderingMetadata();
        
        public int Index { get; set; } = -1;

        public Guid ContentGuid { get; set; } = Guid.Empty;

        public ContentReference ContentLink { get; set; } = ContentReference.EmptyReference;

        public string Tag { get; set; } = string.Empty;

        public BlockRenderingMetadata ParentMetadata { get; set; }

        public int Children { get; set; }
        public bool IsInlineBlock => ContentLink == ContentReference.EmptyReference;
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
    }
}