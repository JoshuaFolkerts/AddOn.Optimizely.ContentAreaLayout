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
            var blockContentGuid = (Guid?)instance.ViewData[RenderingMetadataKeys.Block.ContentGuid] ?? Guid.Empty;
            var blockContentReference = (ContentReference)instance.ViewData[RenderingMetadataKeys.Block.ContentLink] ?? ContentReference.EmptyReference;
            var blockTag = (string?)instance.ViewData[RenderingMetadataKeys.Block.Tag] ?? string.Empty;

            var layoutIndex = (int?)instance.ViewData[RenderingMetadataKeys.Layout.Index] ?? -1;
            var layoutContentGuid = (Guid?)instance.ViewData[RenderingMetadataKeys.Layout.ContentGuid] ?? Guid.Empty;
            var layoutContentReference = (ContentReference)instance.ViewData[RenderingMetadataKeys.Layout.ContentLink] ?? ContentReference.EmptyReference;
            var layoutTag = (string?)instance.ViewData[RenderingMetadataKeys.Layout.Tag] ?? string.Empty;

            return new BlockRenderingMetaData
            {
                BlockIndex = blockIndex,
                BlockContentGuid = blockContentGuid,
                BlockContentLink = blockContentReference,
                BlockTag = blockTag,
                LayoutIndex = layoutIndex,
                LayoutContentGuid = layoutContentGuid,
                LayoutContentLink = layoutContentReference,
                LayoutTag = layoutTag
            };
        }

        public static Dictionary<string, string> BlockMetaDataDictionary(this BlockRenderingMetaData instance, bool allKeys = false)
        {

            var dictionary = new Dictionary<string, string>();
            if (instance is null)
            {
                return dictionary;
            }

            dictionary.Add("block-index", instance.BlockIndex.ToString());
            dictionary.Add("block", instance.BlockContentLink.ID.ToString());
            dictionary.Add("block-tag", instance.BlockTag);
            if (allKeys)
            {
                dictionary.Add("block-guid", instance.BlockContentGuid.ToString());
            }
            return dictionary;
        }
    }
}