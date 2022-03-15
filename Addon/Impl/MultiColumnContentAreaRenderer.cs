using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EPiServer;
using EPiServer.Core;
using EPiServer.Core.Html.StringParsing;
using EPiServer.Web;
using EPiServer.Web.Internal;
using EPiServer.Web.Mvc;
using EPiServer.Web.Mvc.Html;
using EPiServer.Web.Templating;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RenderingLayoutProcessor.Impl
{
    /// <summary>
    ///     Extends the default <see cref="ContentAreaRenderer" /> to apply custom CSS classes to each
    ///     <see cref="ContentFragment" />.
    /// </summary>
    public class MultiColumnContentAreaRenderer : ContentAreaRenderer
    {
        private readonly IContentRenderer _contentRenderer;
        private readonly IContentAreaLoader _contentAreaLoader;
        private readonly IContentAreaItemAttributeAssembler _attributeAssembler;
        private readonly ContentAreaRenderingOptions _contentAreaRenderingOptions;
        private readonly IModelTemplateTagResolver _modelTagResolver;
        private readonly ModelExplorerFactory _modelExplorerFactory;

        private static readonly Action NOP = () => { };
        private bool? _isMethodsOverriden;

        public MultiColumnContentAreaRenderer()
        {
        }

        public MultiColumnContentAreaRenderer(IContentRenderer contentRenderer, ITemplateResolver templateResolver,
            IContentAreaItemAttributeAssembler attributeAssembler,
            IContentRepository contentRepository, IContentAreaLoader contentAreaLoader,
            IContextModeResolver contextModeResolver,
            ContentAreaRenderingOptions contentAreaRenderingOptions, ModelExplorerFactory modelExplorerFactory,
            IModelTemplateTagResolver modelTemplateTagResolver) :
            base(
                contentRenderer, templateResolver, attributeAssembler,
                contentRepository, contentAreaLoader, contextModeResolver,
                contentAreaRenderingOptions, modelExplorerFactory, modelTemplateTagResolver
            )
        {
            _contentRenderer = contentRenderer;
            _attributeAssembler = attributeAssembler;
            _contentAreaLoader = contentAreaLoader;
            _contentAreaRenderingOptions = contentAreaRenderingOptions;
            _modelExplorerFactory = modelExplorerFactory;
            _modelTagResolver = modelTemplateTagResolver;
        }

        protected override void RenderContentAreaItems(IHtmlHelper htmlHelper,
            IEnumerable<ContentAreaItem> contentAreaItems)
        {
            IRenderingContentAreaContext currentContext = DefaultContentAreaContext.Instance;

            foreach (var current in contentAreaItems)
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

                    RenderContentAreaItem(
                        htmlHelper,
                        current,
                        GetContentAreaItemTemplateTag(htmlHelper, current),
                        GetContentAreaItemHtmlTag(htmlHelper, current),
                        GetContentAreaItemCssClass(htmlHelper, current));

                    if (newContext is not ClearRenderingContentAreaContext) currentContext = newContext;

                    continue;
                }

                currentContext.ItemOpen(htmlHelper);

                var contextResult = currentContext.RenderItem(htmlHelper, current,
                    () =>
                    {
                        RenderContentAreaItem(
                            htmlHelper,
                            current,
                            GetContentAreaItemTemplateTag(htmlHelper, current),
                            GetContentAreaItemHtmlTag(htmlHelper, current),
                            GetContentAreaItemCssClass(htmlHelper, current));
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

        protected override void RenderContentAreaItem(IHtmlHelper htmlHelper, ContentAreaItem contentAreaItem,
            string templateTag, string htmlTag, string cssClass)
        {
            var renderSettings = new Dictionary<string, object>
            {
                [RenderSettings.ChildrenCustomTag] = htmlTag,
                [RenderSettings.ChildrenCssClass] = cssClass,
                [RenderSettings.Tag] = templateTag
            };

            if (contentAreaItem.RenderSettings != null)
                // Merge the rendersettings from the content area with the render settings for the fragment
                renderSettings = contentAreaItem.RenderSettings
                    .Concat(renderSettings.Where(r => !contentAreaItem.RenderSettings.ContainsKey(r.Key)))
                    .ToDictionary(r => r.Key, r => r.Value);
            // Store the render settings in the view bag.
            htmlHelper.ViewBag.RenderSettings = renderSettings;

            var content = _contentAreaLoader.Get(contentAreaItem);

            if (content == null) return;

            var tags = Enumerable.Empty<string>();
            if (IsMethodsOverriden)
            {
                //if partner has overriden our methods we run with partner code instead of default
                var contentAreaTag = GetContentAreaTemplateTag(htmlHelper);
                tags = string.IsNullOrWhiteSpace(templateTag)
                    ? new[] {contentAreaTag}
                    : _contentAreaRenderingOptions.TemplateTagSelectionStrategy ==
                    MissingTemplateTagSelectionStrategy.NoTag || string.IsNullOrEmpty(contentAreaTag)
                        ? new[] {templateTag}
                        : new[] {templateTag, contentAreaTag};
            }
            else
            {
                tags = _modelTagResolver.Resolve(_modelExplorerFactory.CreateFromModel(contentAreaItem),
                    htmlHelper.ViewContext);
            }

            // Resolve the template for the content fragment based on the given template tag.
            var templateModel = ResolveTemplate(htmlHelper, content, tags);
            // If there is no template and not in edit mode then don't render.
            if (templateModel == null && !IsInEditMode()) return;

            using (new ContentRenderingScope(htmlHelper.ViewContext.HttpContext, content, templateModel, tags))
            {
                var tagBuilder = new TagBuilder(htmlTag);

                AddNonEmptyCssClass(tagBuilder, cssClass);

                // Applies the required edit attributes to the fragment.
                tagBuilder.MergeAttributes(
                    _attributeAssembler.GetAttributes(contentAreaItem, IsInEditMode(), templateModel != null));

                // Allow partners to modify the start tag before rendering.
                BeforeRenderContentAreaItemStartTag(tagBuilder, contentAreaItem);

                htmlHelper.ViewContext.Writer.Write(tagBuilder.RenderStartTag());

                // Render the content
                htmlHelper.RenderContentData(content, true, templateModel, _contentRenderer);

                htmlHelper.ViewContext.Writer.Write(tagBuilder.RenderEndTag());
            }
        }

        protected override bool ShouldRenderWrappingElement(IHtmlHelper htmlHelper)
        {
            // Default behavior returns true by default if the hascontainer is not specified.
            // Overriding to return false if not specified
            // This only effects the div surrounding the entire content area, not individual items
            var hasContainer = (bool?) htmlHelper.ViewContext.ViewData["hascontainer"];
            var htmlTag = (string) htmlHelper.ViewContext.ViewData["customtag"];
            var cssClass = (string) htmlHelper.ViewContext.ViewData["cssclass"];

            return hasContainer.HasValue && hasContainer.Value
                   || htmlTag != null && htmlTag != "div"
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

        private bool IsMethodsOverriden
        {
            get
            {
                if (!_isMethodsOverriden.HasValue)
                {
                    var isTemplateTagOverriden =
                        GetType().GetMethod(nameof(GetContentAreaTemplateTag),
                                BindingFlags.Instance | BindingFlags.NonPublic, null, new[] {typeof(IHtmlHelper)}, null)
                            .DeclaringType != typeof(ContentAreaRenderer);
                    var isItemTemplateTagOverriden = GetType().GetMethod(nameof(GetContentAreaItemTemplateTag),
                                                         BindingFlags.Instance | BindingFlags.NonPublic, null,
                                                         new[] {typeof(IHtmlHelper), typeof(ContentAreaItem)}, null)
                                                     .DeclaringType !=
                                                     typeof(ContentAreaRenderer);
                    _isMethodsOverriden = isTemplateTagOverriden || isItemTemplateTagOverriden;
                }

                return _isMethodsOverriden.Value;
            }
        }
    }
}