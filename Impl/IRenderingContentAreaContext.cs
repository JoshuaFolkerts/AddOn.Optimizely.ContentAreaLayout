using EPiServer.Core;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RenderingLayoutProcessor.Impl
{
    public interface IRenderingContentAreaContext
    {
        /// <summary>
        /// Render any opening tags prior to the rendering of an item.
        /// (For example, in bootstrap, opening row div if not already there, opening column for each item.)
        /// </summary>
        void ItemOpen(IHtmlHelper htmlHelper);

        /// <summary>
        /// Render any closing tags after an item
        /// (For example, in bootstrap, closing column for each item.)
        /// </summary>
        void ItemClose(IHtmlHelper htmlHelper);

        RenderingProcessorAction RenderItem(IHtmlHelper htmlHelper, ContentAreaItem current, Action renderItem);

        /// <summary>
        /// Area Context is over, clean up any open tags.
        /// </summary>
        void ContainerClose(IHtmlHelper htmlHelper);

        /// <summary>
        /// The begining of a Context.  This is called whether the context contains any items or not.
        /// </summary>
        void ContainerOpen(IHtmlHelper htmlHelper);

        /// <summary>
        /// Return true if this IContentAreaContext is capable of containg the childContext.
        /// Return false and this context will be closed a new context will be started for the
        /// childContext.
        /// </summary>
        bool CanContain(IRenderingContentAreaContext childContext);

        /// <summary>
        /// Return true if this IContentAreaContext is capable of living under the parentContext.
        /// </summary>
        bool CanNestUnder(IRenderingContentAreaContext parentContext);

        IRenderingContentAreaContext ParentContext { get; set; }
    }
}