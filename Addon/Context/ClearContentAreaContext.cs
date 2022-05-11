using System;
using EPiServer.Core;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RenderingLayoutProcessor.Context
{
    /// <summary>
    /// Clears all other context's
    /// </summary>
    public class ClearRenderingContentAreaContext : IRenderingContentAreaContext
    {
        private ClearRenderingContentAreaContext()
        { }

        public static readonly ClearRenderingContentAreaContext Instance = new ClearRenderingContentAreaContext();

        public IRenderingContentAreaContext ParentContext { get; set; }

        public bool CanContain(IRenderingContentAreaContext childContext) => false;

        public bool CanNestUnder(IRenderingContentAreaContext parentContext) => parentContext is DefaultContentAreaContext;

        public void ContainerClose(IHtmlHelper htmlHelper)
        {
        }

        public RenderingProcessorAction RenderItem(IHtmlHelper htmlHelper, ContentAreaItem current, Action renderItem) =>
            RenderingProcessorAction.Close;

        public void ItemClose(IHtmlHelper htmlHelper)
        {
        }

        public void ItemOpen(IHtmlHelper htmlHelper)
        {
        }

        public void ContainerOpen(IHtmlHelper htmlHelper)
        {
        }
    }
}