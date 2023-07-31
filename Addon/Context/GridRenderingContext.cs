using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using AddOn.Optimizely.ContentAreaLayout.Extension;
using AddOn.Optimizely.ContentAreaLayout.Models;

namespace AddOn.Optimizely.ContentAreaLayout.Context
{
    public abstract class GridRenderingContext : IRenderingContentAreaContext
    {
        public IRenderingContentAreaContext ParentContext { get; set; }

        protected readonly int[] _itemSizes;

        protected readonly int _numberOfColumns;

        protected int _renderedItemsCount;

        protected int _columnsRendered;

        public GridRenderingContext(int[] itemSizes = null)
        {
            _itemSizes = itemSizes ?? new[] { 2, 4, 6 };
            _numberOfColumns = _itemSizes.Sum();
        }

        public virtual void ContainerOpen(IHtmlHelper htmlHelper, BlockRenderingMetadata blockMetadata)
        {
            htmlHelper.ViewContext.ViewData[RenderingMetadataKeys.Block] = blockMetadata;
            htmlHelper.ViewContext.ViewData[RenderingMetadataKeys.Layout] = blockMetadata.ParentMetadata;

            var rowTag = new TagBuilder("div");

            foreach (var rowClass in GetRowClasses())
            {
                rowTag.AddCssClass(rowClass);
            }

            rowTag.RenderOpenTo(htmlHelper);
        }

        public virtual void ItemOpen(IHtmlHelper htmlHelper, BlockRenderingMetadata blockMetadata)
        {
            htmlHelper.ViewContext.ViewData[RenderingMetadataKeys.Block] = blockMetadata;

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

        public virtual void ItemClose(IHtmlHelper htmlHelper)
        {
            new TagBuilder("div").RenderCloseTo(htmlHelper);
        }

        public virtual void ContainerClose(IHtmlHelper htmlHelper)
        {
            new TagBuilder("div").RenderCloseTo(htmlHelper);
        }

        // Tell the child what type of blocks are allowed to be contained
        public bool CanContain(IRenderingContentAreaContext childContext) => true;

        // Dictates what is allowed to be nested under item
        public bool CanNestUnder(IRenderingContentAreaContext parentContext) => false;
    }
}