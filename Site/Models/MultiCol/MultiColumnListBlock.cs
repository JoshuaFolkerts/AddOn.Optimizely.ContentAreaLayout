using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Shell.ObjectEditing;
using Microsoft.AspNetCore.Mvc.Rendering;
using RenderingLayoutProcessor.Impl;
using RenderingLayoutProcessor.Impl.EditorDescriptors;
using RenderingLayoutProcessor.Models;

namespace Site.Models.MultiCol
{
    [ContentType(DisplayName = "MultiColumnListBlock", GUID = "413b7a71-8490-4f22-bddc-77325c53424f", Description = "")]
    public class MulticolumnListLayoutBlock : RenderingLayoutBlock
    {
        [Display(Name = "Column Count", GroupName = SystemTabNames.Content, Order = 10)]
        [SelectOne(SelectionFactoryType = typeof(EnumSelectionFactory<NumberOfColumns>))]
        [BackingType(typeof(PropertyNumber))]
        public virtual NumberOfColumns ColumnCount { get; set; }

        [Display(Name = "Render Type", GroupName = SystemTabNames.Content, Order = 11)]
        [SelectOne(SelectionFactoryType = typeof(EnumSelectionFactory<ColumnsRenderType>))]
        [BackingType(typeof(PropertyNumber))]
        public virtual ColumnsRenderType ColumnsRenderingType { get; set; }

        [Display(Name = "Break Point", GroupName = SystemTabNames.Content, Order = 12)]
        [SelectOne(SelectionFactoryType = typeof(EnumSelectionFactory<ColumnBreakPoint>))]
        [BackingType(typeof(PropertyNumber))]
        public virtual ColumnBreakPoint ColumnsBreakPoint
        {
            get
            {
                var breakpoint = this.GetPropertyValue(x => x.ColumnsBreakPoint);
                return breakpoint == 0 ? ColumnBreakPoint.md : breakpoint;
            }

            set
            {
                this.SetPropertyValue(x => x.ColumnsBreakPoint, value);
            }
        }

        [Display(Name = "Ignore Padding?", GroupName = SystemTabNames.Content, Order = 15)]
        public virtual bool NoGutters { get; set; }

        [Display(Name = "Include Container?", GroupName = SystemTabNames.Content, Order = 20)]
        public virtual bool IncludeContainer { get; set; }

        public override IRenderingContentAreaContext NewContext() =>
            new ColumnsContextArea(this);

        public class ColumnsContextArea : IRenderingContentAreaContext
        {
            public MulticolumnListLayoutBlock Block { get; }

            public IRenderingContentAreaContext ParentContext { get; set; }

            private bool calledParentOpen;

            private bool calledParentClose;

            private TagBuilder container;

            private TagBuilder row;

            private TagBuilder col;

            private int totalColumns;

            private int RenderedItemCount;

            private string tag;

            public ColumnsContextArea(MulticolumnListLayoutBlock block)
            {
                Block = block;
            }

            public void ContainerOpen(IHtmlHelper htmlHelper)
            {
            }

            public void ItemOpen(IHtmlHelper htmlHelper)
            {
                if (htmlHelper.ViewData["RenderSettings"] != null)
                {
                    Dictionary<string, object> renderSettings = (Dictionary<string, object>)htmlHelper.ViewData["RenderSettings"];
                    if (renderSettings.Keys != null && renderSettings.Keys.Any())
                    {
                        if (renderSettings.ContainsKey("tag") && renderSettings["tag"] != null)
                        {
                            tag = renderSettings["tag"].ToString();
                        }
                    }
                }

                if (!calledParentOpen)
                {
                    ParentContext.ItemOpen(htmlHelper);
                    calledParentOpen = true;
                }

                if (Block.IncludeContainer)
                {
                    if (container == null)
                    {
                        container = new TagBuilder("div");
                        container.AddCssClass("container");
                        container.RenderOpenTo(htmlHelper);
                    }
                }

                if (row == null)
                {
                    var rowClass = new List<string> { $"row" };

                    if (Block.NoGutters)
                    {
                        rowClass.Add($"gx-{Block.ColumnsBreakPoint}-0");
                    }
                    else
                    {
                        rowClass.Add($"gy-2");
                    }

                    row = new TagBuilder("div");

                    row.AddCssClass(string.Join(" ", rowClass));

                    row.RenderOpenTo(htmlHelper);
                }

                if (col == null)
                {
                    col = new TagBuilder("div");

                    if (Block.ColumnsRenderingType == ColumnsRenderType.Default)
                        col.AddCssClass(Block.ColumnCount.ColumnCssClass(Block.ColumnsBreakPoint));
                    else
                        col.AddCssClass(ColumnsTagCssClass(Block.ColumnsRenderingType, RenderedItemCount, Block.ColumnsBreakPoint));

                    if (RenderedItemCount != 0)
                    {
                        col.AddCssClass($"offset0");
                    }

                    col.RenderOpenTo(htmlHelper);
                }
            }

            public void ItemClose(IHtmlHelper htmlHelper)
            {
                if (col != null)
                {
                    col.RenderCloseTo(htmlHelper);
                    col = null;
                }
            }

            public RenderingProcessorAction RenderItem(IHtmlHelper htmlHelper, ContentAreaItem current, Action renderItem)
            {
                renderItem();
                totalColumns += 1;
                this.RenderedItemCount++;
                return (totalColumns >= (int)Block.ColumnCount) ?
                    RenderingProcessorAction.Close :
                    RenderingProcessorAction.Continue;
            }

            public void ContainerClose(IHtmlHelper htmlHelper)
            {
                if (col != null)
                {
                    col.RenderCloseTo(htmlHelper);
                    col = null;
                }

                if (row != null)
                {
                    row.RenderCloseTo(htmlHelper);
                    row = null;
                }
                if (Block.IncludeContainer)
                {
                    if (container != null)
                    {
                        container.RenderCloseTo(htmlHelper);
                        container = null;
                    }
                }

                if (!calledParentClose)
                {
                    ParentContext.ItemClose(htmlHelper);
                    calledParentClose = true;
                }
            }

            // Tell the child what type of blocks are allowed to be contained
            public bool CanContain(IRenderingContentAreaContext childContext) => true;

            // Dictates what is allowed to be nested under item
            public bool CanNestUnder(IRenderingContentAreaContext parentContext) => true;

            private string ColumnsTagCssClass(ColumnsRenderType renderType, int currentItem, ColumnBreakPoint breakPoint = ColumnBreakPoint.md)
            {
                if (renderType == ColumnsRenderType.Layout50x25x25)
                {
                    switch (currentItem)
                    {
                        case 0:
                            return $"span6";

                        default:
                            return $"span3";
                    }
                }
                else if (renderType == ColumnsRenderType.ContainerNarrow)
                {
                    return $"span10 offset2";
                }
                else if (renderType == ColumnsRenderType.ContainerXNarrow)
                {
                    return $"span8 offset3";
                }

                return string.Empty;
            }
        }
    }

    public enum NumberOfColumns
    {
        One = 1,

        Two = 2,

        Three = 3,

        Four = 4,

        Six = 6
    }

    public enum ColumnsRenderType
    {
        Default = 0,

        // Add different types of layouts.
        Layout50x25x25 = 1,

        ContainerNarrow,

        ContainerXNarrow
    }

    public static class NumberOfColumnsExtensions
    {
        public static string ColumnCssClass(this NumberOfColumns cols, ColumnBreakPoint breakPoint = ColumnBreakPoint.md)
        {
            switch (cols)
            {
                case NumberOfColumns.Six:
                    return $"span2";

                case NumberOfColumns.Four:
                    return $"span3";

                case NumberOfColumns.Three:
                    return $"span4";

                case NumberOfColumns.Two:
                    return $"span6";
            }
            return string.Empty;
        }
    }
}