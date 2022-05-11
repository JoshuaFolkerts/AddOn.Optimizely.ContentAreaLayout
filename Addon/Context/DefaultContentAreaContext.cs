using System;
using EPiServer.Core;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RenderingLayoutProcessor.Context
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

        public void ContainerOpen(IHtmlHelper htmlHelper)
        {
        }

        public void ContainerClose(IHtmlHelper htmlHelper)
        {
        }

        public RenderingProcessorAction RenderItem(IHtmlHelper htmlHelper, ContentAreaItem current, Action renderItem)
        {
            renderItem();
            return RenderingProcessorAction.Continue;
        }

        public void ItemClose(IHtmlHelper htmlHelper)
        {
        }

        public void ItemOpen(IHtmlHelper htmlHelper)
        {
        }
    }
}