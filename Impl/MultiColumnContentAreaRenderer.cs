using EPiServer;
using EPiServer.Core;
using EPiServer.Web;
using EPiServer.Web.Mvc;
using EPiServer.Web.Mvc.Html;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Core.Html.StringParsing;
using EPiServer.Web.Internal;
using EPiServer.Web.Templating;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RenderingLayoutProcessor.Impl
{
    /// <summary>
    /// Extends the default <see cref="ContentAreaRenderer"/> to apply custom CSS classes to each <see cref="ContentFragment"/>.
    /// </summary>
    public class MultiColumnContentAreaRenderer : ContentAreaRenderer
    {
        private readonly IContentAreaItemAttributeAssembler attributeAssembler;

        private readonly IContentRenderer contentRenderer;

        private static readonly System.Action NOP = () => { };

        public MultiColumnContentAreaRenderer() : base()
        {
        }
        
        //[DefaultConstructor]
        public MultiColumnContentAreaRenderer(
            IContentRenderer contentRenderer,
            ITemplateResolver templateResolver,
            IContentAreaItemAttributeAssembler attributeAssembler,
            IContentRepository contentRepository,
            IContentAreaLoader contentAreaLoader,
            IContextModeResolver contextModeResolver,
            ContentAreaRenderingOptions contentAreaRenderingOptions,
            ModelExplorerFactory modelExplorerFactory,
            IModelTemplateTagResolver modelTemplateTagResolver)
            : base(contentRenderer, templateResolver, attributeAssembler, contentRepository, contentAreaLoader, contextModeResolver, contentAreaRenderingOptions, modelExplorerFactory, modelTemplateTagResolver)
        {
            this.attributeAssembler = attributeAssembler;
            this.contentRenderer = contentRenderer;
        }

        protected override void RenderContentAreaItems(IHtmlHelper htmlHelper, IEnumerable<ContentAreaItem> contentAreaItems)
        {
            IRenderingContentAreaContext currentContext = DefaultContentAreaContext.Instance;

            foreach (ContentAreaItem current in contentAreaItems)
            {
                if (current.GetContent() is IRenderingLayoutBlock asLayoutBlock)
                {
                    var newContext = asLayoutBlock.NewContext();
                    while (!newContext.CanNestUnder(currentContext))
                    {
                        currentContext.ContainerClose(htmlHelper);
                        currentContext = currentContext.ParentContext;
                    }
                    while (!currentContext.CanContain(newContext))
                    {
                        currentContext.ContainerClose(htmlHelper);
                        currentContext = currentContext.ParentContext;
                    }

                    newContext.ParentContext = currentContext;
                    newContext.ContainerOpen(htmlHelper);

                    RenderContentAreaItem(htmlHelper, current, GetContentAreaItemTemplateTag(htmlHelper, current), GetContentAreaItemHtmlTag(htmlHelper, current), GetContentAreaItemCssClass(htmlHelper, current));

                    if (!(newContext is ClearRenderingContentAreaContext))
                    {
                        currentContext = newContext;
                    }

                    continue;
                }

                currentContext.ItemOpen(htmlHelper);

                var contextResult = currentContext.RenderItem(htmlHelper, current, () =>
                {
                    RenderContentAreaItem(htmlHelper, current, GetContentAreaItemTemplateTag(htmlHelper, current), GetContentAreaItemHtmlTag(htmlHelper, current), GetContentAreaItemCssClass(htmlHelper, current));
                });

                currentContext.ItemClose(htmlHelper);

                while (contextResult == RenderingProcessorAction.Close)
                {
                    currentContext.ContainerClose(htmlHelper);
                    currentContext = currentContext.ParentContext;
                    contextResult = currentContext.RenderItem(htmlHelper, null, NOP);
                }
            }

            while (currentContext != null)
            {
                currentContext.ContainerClose(htmlHelper);
                currentContext = currentContext.ParentContext;
            }
        }

        protected override void RenderContentAreaItem(IHtmlHelper htmlHelper, ContentAreaItem contentAreaItem, string templateTag, string htmlTag, string cssClass)
        {
            var renderSettings = new Dictionary<string, object>
            {
                ["childrencustomtagname"] = htmlTag,
                ["childrencssclass"] = cssClass,
                ["tag"] = templateTag
            };

            htmlHelper.ViewBag.RenderSettings = contentAreaItem.RenderSettings
                .Concat(renderSettings.Where(r => !contentAreaItem.RenderSettings.ContainsKey(r.Key)))
                .ToDictionary(r => r.Key, r => r.Value);

            var content = contentAreaItem.GetContent();
            if (content == null)
                return;

            //using (new ContentRenderingScope(htmlHelper.ViewContext.HttpContext, (IContentData) content, templateModel, templateTags))
            //{
            //    var templateModel = ResolveTemplate(htmlHelper, content, templateTag);
            //    if (templateModel != null || IsInEditMode())
            //    {
            //        // Only wrap individual content area items when necessary
            //        var attributesToMerge = attributeAssembler.GetAttributes(contentAreaItem, IsInEditMode(htmlHelper), templateModel != null);
            //        bool shouldHaveTag = attributesToMerge.Count > 0 || !string.IsNullOrEmpty(cssClass) || htmlTag != "div" || IsInEditMode(htmlHelper);
            //        TagBuilder tagBuilder = shouldHaveTag
            //            ? new TagBuilder(htmlTag)
            //            : null;

            //        if (tagBuilder != null)
            //        {
            //            AddNonEmptyCssClass(tagBuilder, cssClass);
            //            tagBuilder.MergeAttributes(attributesToMerge);
            //            BeforeRenderContentAreaItemStartTag(tagBuilder, contentAreaItem);
            //            htmlHelper.ViewContext.Writer.Write(tagBuilder.ToString(TagRenderMode.StartTag));
            //        }

            //        htmlHelper.RenderContentData(content, true, templateModel, contentRenderer);

            //        if (tagBuilder != null)
            //        {
            //            htmlHelper.ViewContext.Writer.Write(tagBuilder.ToString(TagRenderMode.EndTag));
            //        }
            //    }
            //}
        }

        protected override bool ShouldRenderWrappingElement(IHtmlHelper htmlHelper)
        {
            // Default behavior returns true by default if the hascontainer is not specified.
            // Overriding to return false if not specified
            // This only effects the div surrounding the entire content area, not individual items
            bool? hasContainer = (bool?)htmlHelper.ViewContext.ViewData["hascontainer"];
            string htmlTag = (string)htmlHelper.ViewContext.ViewData["customtag"];
            string cssClass = (string)htmlHelper.ViewContext.ViewData["cssclass"];

            return (hasContainer.HasValue && hasContainer.Value)
                || (htmlTag != null && htmlTag != "div")
                || !string.IsNullOrEmpty(cssClass);
        }

        protected override string GetContentAreaItemCssClass(IHtmlHelper htmlHelper, ContentAreaItem contentAreaItem)
        {
            if (contentAreaItem.GetContent() is IRenderingLayoutBlock)
                return string.Empty;

            var tag = GetContentAreaItemTemplateTag(htmlHelper, contentAreaItem);
            var list = new List<string>();

            if (htmlHelper.ViewData["childrencssclass"] != null)
                list.Add(htmlHelper.ViewData["childrencssclass"].ToString().ToLowerInvariant().Trim());

            if (!string.IsNullOrWhiteSpace(tag))
                list.Add(tag.ToLowerInvariant().Trim());

            return string.Join(" ", list);
        }
    }
}