using EPiServer.Core;
using System;

namespace RenderingLayoutProcessor.Block
{
    public class BlockRenderingMetaData
    {
        public int BlockIndex { get; set; }
        public Guid BlockContentGuid { get; set; }
        public ContentReference BlockContentLink { get; set; }
        public string BlockTag { get; set; }
        public int LayoutIndex { get; set; }
        public Guid LayoutContentGuid { get; set; }
        public ContentReference LayoutContentLink { get; set; }
        public string LayoutTag { get; set; }
    }
}