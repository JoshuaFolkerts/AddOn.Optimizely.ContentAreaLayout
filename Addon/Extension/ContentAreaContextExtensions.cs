using EPiServer.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using AddOn.Optimizely.ContentAreaLayout.Models;
using AddOn.Optimizely.ContentAreaLayout.Context;
using System;
using System.Collections.Generic;
using EPiServer.ServiceLocation;
using EPiServer.DataAbstraction;
using EPiServer;
using System.Linq;
using AddOn.Optimizely.ContentAreaLayout.Attributes;
using System.Reflection;

namespace AddOn.Optimizely.ContentAreaLayout.Extension
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

        public static BlockRenderingMetadata BlockMetadata(this IHtmlHelper instance)
        {
            var blockMetadata = instance.ViewData[RenderingMetadataKeys.Block] as BlockRenderingMetadata ?? null;
            var layoutMetadata = instance.ViewData[RenderingMetadataKeys.Layout] as BlockRenderingMetadata ?? null;
            blockMetadata.ParentMetadata = layoutMetadata ?? new BlockRenderingMetadata();

            return blockMetadata;
        }

        public static Dictionary<string, string> GetRenderAttributes(this ContentData instance)
        {
            var attributes = new Dictionary<string, string>();

            var renderAttributeProperties = instance.GetType().GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(PropertyToHtmlAttributeAttribute)));
            var contentType = instance.GetType();
            foreach (var attributeProp in renderAttributeProperties)
            {
                var propertyName = attributeProp.Name.Trim().ToLower();

                var propertyValue = instance.GetPropertyValue(attributeProp.Name, string.Empty);

                if (propertyValue == "True" || propertyValue == "False")
                {
                    propertyValue = propertyValue.ToLowerInvariant();
                }

                var renderAttribute = contentType.GetProperty(attributeProp.Name).GetCustomAttribute<PropertyToHtmlAttributeAttribute>();
                if (string.IsNullOrEmpty(propertyValue) && !renderAttribute.RenderIfEmpty)
                {
                    continue;
                }

                var attributeName = renderAttribute.AttributeName ?? String.Format("data-{0}", propertyName);

                attributes.Add(attributeName, propertyValue);
            }

            return attributes;
        }

        public static T GetContent<T>(this BlockRenderingMetadata instance, IContentLoader contentLoader = null) where T : IContentData
        {
            if (ContentReference.IsNullOrEmpty(instance.ContentLink))
            {
                return default(T);
            }
            contentLoader ??= ServiceLocator.Current.GetInstance<IContentLoader>();
            if (!contentLoader.TryGet<T>(instance.ContentLink, out T content))
            {
                return default(T);
            }
            return content;
        }

        public static string GetContentTypeName(this BlockRenderingMetadata instance, IContentTypeRepository contentTypeRepository = null)
        {
            var content = instance.GetContent<IContent>();
            if (content is null)
            {
                return null;
            }
            contentTypeRepository ??= ServiceLocator.Current.GetInstance<IContentTypeRepository>();
            var contentType = contentTypeRepository.Load(content.ContentTypeID);
            return contentType.Name;
        }

        public static Dictionary<string, string> BlockMetadataDictionary(this BlockRenderingMetadata instance, bool allKeys = false)
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

        public static string UniqueBlockId(this BlockRenderingMetadata blockMetadata, string prefix = "", string postfix = "")
        {
            return $"{prefix}-{blockMetadata.ContentLink.ID}-{blockMetadata.Index}-{blockMetadata.ParentMetadata.Index}{postfix}";
        }
        public static string UniqueBlockId(this BlockRenderingMetadata blockMetadata, BlockData block, string prefix = "", string postfix = "")
        {
            prefix = block.GetType().Name.ToLowerInvariant().Replace("proxy", "") + prefix;

            return blockMetadata.UniqueBlockId(prefix, postfix);
        }
    }
}