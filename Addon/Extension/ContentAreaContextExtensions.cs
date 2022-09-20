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
            var blockMetadata = instance.ViewData[RenderingMetadataKeys.Block] as BlockRenderingMetadata ?? BlockRenderingMetadata.Empty;
            var layoutMetadata = instance.ViewData[RenderingMetadataKeys.Layout] as BlockRenderingMetadata ?? BlockRenderingMetadata.Empty;
            blockMetadata.ParentMetadata = layoutMetadata;

            return blockMetadata;
        }
        
        public static Dictionary<string, string> GetBlockMetadataProperties(this IContent instance)
        {
            var properties = new Dictionary<string, string>();
            if (instance is null || !(instance is ContentData content))
            {
                return properties;
            }
            var sourceProperties = content.GetPropertyAttributes<BlockRenderingMetadataAttributeAttribute>();
            if (!sourceProperties?.Any() ?? false)
            {
                return properties;
            }

            foreach (var property in sourceProperties)
            {
                var contentType = content.GetType();
                var prop = contentType.GetProperty(property.Key);
                var propertyAttribute = prop?.GetCustomAttribute<BlockRenderingMetadataAttributeAttribute>();
                if (!string.IsNullOrEmpty(propertyAttribute?.Name))
                {
                    properties.Add(propertyAttribute.Name, property.Value);
                }
                else
                {
                    properties.Add(property.Key, property.Value);
                }
            }
            return properties;
        }

        public static Dictionary<string, string> GetPropertyAttributes<T>(this ContentData instance) where T : System.Attribute
        {
            var attributes = new Dictionary<string, string>();
            if (instance is null)
            {
                return attributes;
            }

            var renderAttributeProperties = instance.GetType().GetProperties().Where((prop) => System.Attribute.IsDefined(prop, typeof(T)));

            foreach (var attributeProp in renderAttributeProperties)
            {
                var propertyValue = instance.GetPropertyValue(attributeProp.Name, string.Empty);

                if (attributeProp.PropertyType == typeof(bool))
                {
                    propertyValue = (attributeProp.GetValue(instance) as bool? ?? false).ToString().ToLowerInvariant();
                }
                attributes.Add(attributeProp.Name, propertyValue);
            }

            return attributes;
        }

        public static Dictionary<string, string> GetRenderAttributes(this ContentData instance)
        {
            var attributes = new Dictionary<string, string>();
            if (instance is null)
            {
                return attributes;
            }
            var sourceAttributes = instance.GetPropertyAttributes<PropertyToHtmlAttributeAttribute>();
            foreach (var attribute in sourceAttributes)
            {
                var contentType = instance.GetType();
                var prop = contentType.GetProperty(attribute.Key);
                var renderAttribute = prop?.GetCustomAttribute<PropertyToHtmlAttributeAttribute>();

                if (string.IsNullOrEmpty(attribute.Value) && renderAttribute?.RenderIfEmpty != false)
                {
                    continue;
                }
                // This overrides the original dictionary item with it's custom name
                if (!string.IsNullOrEmpty(renderAttribute?.AttributeName))
                {
                    attributes.Add(renderAttribute.AttributeName, attribute.Value);
                }
                else
                {
                    attributes.Add(attribute.Key, attribute.Value);
                }
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