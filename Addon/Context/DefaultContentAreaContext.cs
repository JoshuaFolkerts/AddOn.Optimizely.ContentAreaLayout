using AddOn.Optimizely.ContentAreaLayout.Models;
using System;
using EPiServer.Core;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AddOn.Optimizely.ContentAreaLayout.Context
{
    public class DefaultContentAreaContext : IRenderingContentAreaContext, IRenderingContentAreaFallbackContext
    {
        public IRenderingContentAreaContext ParentContext { get; set; }

        public virtual bool CanContain(IRenderingContentAreaContext childContext) => true;

        public virtual bool CanNestUnder(IRenderingContentAreaContext parentContext) =>
            throw new InvalidOperationException("At no point should this be nested under any other context.");

        public virtual void ContainerOpen(IHtmlHelper htmlHelper, BlockRenderingMetadata blockMetadata)
        {
        }

        public virtual void ContainerClose(IHtmlHelper htmlHelper)
        {
        }

        public virtual RenderingProcessorAction RenderItem(IHtmlHelper htmlHelper, ContentAreaItem current, Action renderItem)
        {
            renderItem();
            return RenderingProcessorAction.Continue;
        }
        
        public virtual void ItemOpen(IHtmlHelper htmlHelper, BlockRenderingMetadata blockMetadata)
        {
            htmlHelper.ViewContext.ViewData[RenderingMetadataKeys.Block] = blockMetadata;
        }

        public virtual void ItemClose(IHtmlHelper htmlHelper)
        {
        }
    }
}