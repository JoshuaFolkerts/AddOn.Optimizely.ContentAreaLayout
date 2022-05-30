using EPiServer.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using RenderingLayoutProcessor.Models;
using RenderingLayoutProcessor.Context;
using System;
using System.Collections.Generic;
using EPiServer.ServiceLocation;
using EPiServer.DataAbstraction;
using EPiServer;

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
            var blockMetaData = instance.ViewData[RenderingMetadataKeys.Block] as BlockRenderingMetaData ?? null;
            var layoutMetaData = instance.ViewData[RenderingMetadataKeys.Layout] as BlockRenderingMetaData ?? null;
            blockMetaData.ParentMetaData = layoutMetaData ?? new BlockRenderingMetaData();

            return blockMetaData;
        }

        public static string GetContentTypeName(this BlockRenderingMetaData instance, IContentLoader contentLoader = null, IContentTypeRepository contentTypeRepository = null)
        {
            if (ContentReference.IsNullOrEmpty(instance.ContentLink))
            {
                return null;
            }
            contentLoader ??= ServiceLocator.Current.GetInstance<IContentLoader>();
            if (!contentLoader.TryGet<IContent>(instance.ContentLink, out IContent content))
            {
                return null;
            }
            contentTypeRepository ??= ServiceLocator.Current.GetInstance<IContentTypeRepository>();
            var contentType = contentTypeRepository.Load(content.ContentTypeID);
            return contentType.Name;
        }

        public static Dictionary<string, string> BlockMetaDataDictionary(this BlockRenderingMetaData instance, bool allKeys = false)
        {
            var dictionary = new Dictionary<string, string>();
            if (instance is null)
            {
                return dictionary;
            }

            dictionary.Add("block-index", instance.Index.ToString());
            dictionary.Add("block", instance.ContentLink.ID.ToString());
            dictionary.Add("block-tag", instance.Tag);
            if (allKeys)
            {
                dictionary.Add("block-guid", instance.ContentGuid.ToString());
            }
            return dictionary;
        }
    }
}