using EPiServer.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using RenderingLayoutProcessor.Block;
using RenderingLayoutProcessor.Context;
using System;
using System.Collections.Generic;

namespace RenderingLayoutProcessor.Extension
{
    public static class ContentAreaContextExtensions
    {
        public static IEnumerable<IRenderingContentAreaContext> ContextStack(this IRenderingContentAreaContext area)
        {
            while (area != null)
            {
                yield return area;
                area = area.ParentContext;
            }
        }

        public static BlockRenderingMetaData BlockMetaData(this IHtmlHelper instance)
        {
            // var contentAreaMetaData = instance.ViewData[RenderingMetadataKeys.MetaData] as ContentAreaMetaData ?? ContentSection.ContentAreaMetaData.Empty;
            var blockIndex = (int?)instance.ViewData[RenderingMetadataKeys.Block.Index] ?? -1;
            var contentGuid = (Guid?)instance.ViewData[RenderingMetadataKeys.Block.ContentGuid] ?? Guid.Empty;
            var blockContentReference = (ContentReference)instance.ViewData[RenderingMetadataKeys.Block.ContentLink] ?? ContentReference.EmptyReference;

            return new BlockRenderingMetaData
            {
                BlockIndex = blockIndex,
                BlockContentGuid = contentGuid,
                BlockContentLink = blockContentReference,
            };
        }
    }
}