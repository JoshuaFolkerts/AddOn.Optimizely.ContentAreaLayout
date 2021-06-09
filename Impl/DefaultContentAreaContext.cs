using EPiServer.Core;
using RenderingLayoutProcessor.Models;
using System;
using System.Web.Mvc;

namespace RenderingLayoutProcessor.Impl
{
    public class DefaultContentAreaContext : IRenderingContentAreaContext
    {
        public static readonly DefaultContentAreaContext Instance = new DefaultContentAreaContext();

        private DefaultContentAreaContext()
        { }

        public IRenderingContentAreaContext ParentContext { get; set; }

        public bool CanContain(IRenderingContentAreaContext childContext) => true;

        public bool CanNestUnder(IRenderingContentAreaContext parentContext) =>
            throw new InvalidOperationException("At no point should this be nested under any other context.");

        public void ContainerOpen(HtmlHelper htmlHelper)
        {
        }

        public void ContainerClose(HtmlHelper htmlHelper)
        {
        }

        public RenderingProcessorAction RenderItem(HtmlHelper htmlHelper, ContentAreaItem current, Action renderItem)
        {
            renderItem();
            return RenderingProcessorAction.Continue;
        }

        public void ItemClose(HtmlHelper htmlHelper)
        {
        }

        public void ItemOpen(HtmlHelper htmlHelper)
        {
        }
    }
}