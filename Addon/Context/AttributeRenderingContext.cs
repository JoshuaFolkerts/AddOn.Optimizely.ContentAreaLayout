using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using AddOn.Optimizely.ContentAreaLayout.Extension;
using AddOn.Optimizely.ContentAreaLayout.Models;

namespace AddOn.Optimizely.ContentAreaLayout.Context
{
    public class AttributeRenderingContext : IRenderingContentAreaContext
    {
        public IRenderingContentAreaContext ParentContext { get; set; }

        protected bool _enableContainerWrapper;

        public virtual void ContainerOpen(IHtmlHelper htmlHelper, BlockRenderingMetadata blockMetadata)
        {
            htmlHelper.ViewContext.ViewData[RenderingMetadataKeys.Block] = blockMetadata;
            htmlHelper.ViewContext.ViewData[RenderingMetadataKeys.Layout] = blockMetadata.ParentMetadata;
            
            if (_enableContainerWrapper)
            {
                ProcessContainer(htmlHelper);
            }

            var rowTag = new TagBuilder("div");

            foreach (var rowClass in GetRowClasses())
            {
                rowTag.AddCssClass(rowClass);
            }
            
            rowTag.MergeAttribute("data-layout", blockMetadata.ParentMetadata.ContentLink.ID.ToString());
            rowTag.MergeAttribute("data-layout-index", blockMetadata.ParentMetadata.Index.ToString());
            rowTag.MergeAttribute("data-layout-children", blockMetadata.ParentMetadata.Children.ToString());
            if (htmlHelper.ViewContext.ViewData.ContainsKey("ContainerCssClass"))
            {
                rowTag.Attributes.Add("class", htmlHelper.ViewContext.ViewData["ContainerCssClass"].ToString());
            }
            foreach (var property in blockMetadata.ParentMetadata.Properties)
            {
                rowTag.MergeAttribute(property.Key, property.Value);
            }

            var block = blockMetadata.GetContent<ContentData>();
            if (block != null)
            {
                var attributes = block.GetRenderAttributes();
                foreach (var attribute in attributes)
                {
                    rowTag.MergeAttribute(attribute.Key, attribute.Value);
                }
            }

            rowTag.RenderOpenTo(htmlHelper);
        }

        private void ProcessContainer(IHtmlHelper htmlHelper)
        {
            var containerTag = new TagBuilder("div");

            foreach (var containerClass in GetContainerClasses())
            {
                containerTag.AddCssClass(containerClass);
            }

            containerTag.RenderOpenTo(htmlHelper);
        }

        public void ItemOpen(IHtmlHelper htmlHelper, BlockRenderingMetadata blockMetadata)
        {
            htmlHelper.ViewContext.ViewData[RenderingMetadataKeys.Block] = blockMetadata;
        }

        protected virtual IEnumerable<string> GetContainerClasses()
        {
            return Enumerable.Empty<string>();
        }

        protected virtual IEnumerable<string> GetRowClasses()
        {
            return Enumerable.Empty<string>();
        }

        protected virtual IEnumerable<string> GetColumnClasses()
        {
            return Enumerable.Empty<string>();
        }

        public RenderingProcessorAction RenderItem(IHtmlHelper htmlHelper, ContentAreaItem current, Action renderItem)
        {
            renderItem();
            return RenderingProcessorAction.Continue;
        }

        public void ItemClose(IHtmlHelper htmlHelper)
        {
        }

        public void ContainerClose(IHtmlHelper htmlHelper)
        {
            new TagBuilder("div").RenderCloseTo(htmlHelper);

            if (_enableContainerWrapper)
            {
                ProcessCloseContainer(htmlHelper);
            }
        }

        private void ProcessCloseContainer(IHtmlHelper htmlHelper)
        {
            new TagBuilder("div").RenderCloseTo(htmlHelper);
        }

        // Tell the child what type of blocks are allowed to be contained
        public bool CanContain(IRenderingContentAreaContext childContext) => true;

        // Dictates what is allowed to be nested under item
        public bool CanNestUnder(IRenderingContentAreaContext parentContext) => false;
    }
}