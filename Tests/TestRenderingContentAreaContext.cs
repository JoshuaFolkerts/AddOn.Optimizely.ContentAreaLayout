using System;
using EPiServer.Core;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using RenderingLayoutProcessor.Impl;

namespace Tests
{
    public class TestRenderingContentAreaContext : IRenderingContentAreaContext
    {
        private readonly RenderingProcessorAction _action = RenderingProcessorAction.Continue;
        private readonly int _id;

        public TestRenderingContentAreaContext(int id) => _id = id;
        
        public TestRenderingContentAreaContext(int id, RenderingProcessorAction action) : this(id) => _action = action;

        public void ContainerClose(IHtmlHelper htmlHelper) => new TagBuilder($"c_{_id}").RenderCloseTo(htmlHelper);
        public void ContainerOpen(IHtmlHelper htmlHelper) => new TagBuilder($"c_{_id}").RenderOpenTo(htmlHelper);
        public void ItemOpen(IHtmlHelper htmlHelper) => new TagBuilder($"iw_{_id}").RenderOpenTo(htmlHelper);
        public void ItemClose(IHtmlHelper htmlHelper) => new TagBuilder($"iw_{_id}").RenderCloseTo(htmlHelper);

        public RenderingProcessorAction RenderItem(IHtmlHelper htmlHelper, ContentAreaItem current, Action renderItem)
        {
            new TagBuilder($"i").RenderTagTo(htmlHelper);
            return _action;
        }
        
        public virtual bool CanContain(IRenderingContentAreaContext childContext) => true;

        public virtual bool CanNestUnder(IRenderingContentAreaContext parentContext) => true;

        public IRenderingContentAreaContext ParentContext { get; set; }
    }
}