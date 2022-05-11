using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using RenderingLayoutProcessor.Extension;

namespace RenderingLayoutProcessor.Context
{
    public abstract class GridRenderingContext : IRenderingContentAreaContext
    {
        public IRenderingContentAreaContext ParentContext { get; set; }

        protected readonly int[] _itemSizes;

        protected readonly int _numberOfColumns;

        protected int _renderedItemsCount;

        protected int _columnsRendered;

        protected bool _enableContainerWrapper;

        public GridRenderingContext(int[] itemSizes = null)
        {
            _itemSizes = itemSizes ?? new[] { 2, 4, 6 };
            _numberOfColumns = _itemSizes.Sum();
        }

        public void ContainerOpen(IHtmlHelper htmlHelper)
        {
            if (_enableContainerWrapper)
            {
                ProcessContainer(htmlHelper);
            }

            var rowTag = new TagBuilder("div");

            foreach (var rowClass in GetRowClasses())
            {
                rowTag.AddCssClass(rowClass);
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

        public void ItemOpen(IHtmlHelper htmlHelper)
        {
            var colTag = new TagBuilder("div");
            foreach (var columnClass in GetColumnClasses())
            {
                colTag.AddCssClass(columnClass);
            }

            colTag.RenderOpenTo(htmlHelper);
        }

        protected virtual IEnumerable<string> GetContainerClasses()
        {
            return Enumerable.Empty<string>();
        }

        protected abstract IEnumerable<string> GetRowClasses();

        protected abstract IEnumerable<string> GetColumnClasses();

        public RenderingProcessorAction RenderItem(IHtmlHelper htmlHelper, ContentAreaItem current, Action renderItem)
        {
            renderItem();
            _renderedItemsCount++;
            return _renderedItemsCount >= _itemSizes.Length ?
                RenderingProcessorAction.Close :
                RenderingProcessorAction.Continue;
        }

        public void ItemClose(IHtmlHelper htmlHelper)
        {
            new TagBuilder("div").RenderCloseTo(htmlHelper);
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