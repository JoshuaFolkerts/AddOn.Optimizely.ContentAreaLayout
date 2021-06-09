using EPiServer.Core;
using System;
using System.Web.Mvc;

namespace RenderingLayoutProcessor.Impl
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

        public void ContainerClose(HtmlHelper htmlHelper)
        {
        }

        public RenderingProcessorAction RenderItem(HtmlHelper htmlHelper, ContentAreaItem current, Action renderItem) =>
            RenderingProcessorAction.Close;

        public void ItemClose(HtmlHelper htmlHelper)
        {
        }

        public void ItemOpen(HtmlHelper htmlHelper)
        {
        }

        public void ContainerOpen(HtmlHelper htmlHelper)
        {
        }
    }
}