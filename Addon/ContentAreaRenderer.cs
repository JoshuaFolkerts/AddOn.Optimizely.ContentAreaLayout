
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EPiServer.Core;
using EPiServer.Core.Html.StringParsing;
using EPiServer.DataAbstraction;
using EPiServer.Framework;
using EPiServer.Framework.Web;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web.Internal;
using EPiServer.Web.Templating;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace EPiServer.Web.Mvc.Html
{
    /// <summary>
    /// The default implementation for rendering a <see cref="ContentArea"/>.
    /// </summary>
    public class ContentAreaRenderer
    {
        private readonly IContentRenderer _contentRenderer;
        private readonly IContentRepository _contentRepository;
        private readonly IContentAreaLoader _contentAreaLoader;
        private readonly IContextModeResolver _contextModeResolver;
        private readonly ContentAreaRenderingOptions _contentAreaRenderingOptions;
        private readonly ModelExplorerFactory _modelExplorerFactory;
        private readonly IModelTemplateTagResolver _modelTagResolver;
        private readonly ITemplateResolver _templateResolver;
        private readonly IContentAreaItemAttributeAssembler _attributeAssembler;

        private bool? _isMethodsOverriden;

        /// <summary>
        /// The content repository
        /// </summary>
        protected IContentRepository ContentRepository => _contentRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentAreaRenderer" /> class.
        /// </summary>
        public ContentAreaRenderer()
            : this(
                ServiceLocator.Current.GetInstance<IContentRenderer>(),
                ServiceLocator.Current.GetInstance<ITemplateResolver>(),
                ServiceLocator.Current.GetInstance<IContentAreaItemAttributeAssembler>(),
                ServiceLocator.Current.GetInstance<IContentRepository>(),
                ServiceLocator.Current.GetInstance<IContentAreaLoader>(),
                ServiceLocator.Current.GetInstance<IContextModeResolver>(),
                ServiceLocator.Current.GetInstance<ContentAreaRenderingOptions>(),
                ServiceLocator.Current.GetInstance<ModelExplorerFactory>(),
                ServiceLocator.Current.GetInstance<IModelTemplateTagResolver>())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentAreaRenderer"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Pubternality", "EP1100:Pubternal type is exposed in a Public API", Justification = "Publicly released API. Need to maintain for compatibility.")]
        public ContentAreaRenderer(IContentRenderer contentRenderer, ITemplateResolver templateResolver, IContentAreaItemAttributeAssembler attributeAssembler,
            IContentRepository contentRepository, IContentAreaLoader contentAreaLoader, IContextModeResolver contextModeResolver,
            ContentAreaRenderingOptions contentAreaRenderingOptions, ModelExplorerFactory modelExplorerFactory, IModelTemplateTagResolver modelTemplateTagResolver)
        {
            _contentRenderer = contentRenderer;
            _templateResolver = templateResolver;
            _attributeAssembler = attributeAssembler;
            _contentRepository = contentRepository;
            _contentAreaLoader = contentAreaLoader;
            _contextModeResolver = contextModeResolver;
            _contentAreaRenderingOptions = contentAreaRenderingOptions;
            _modelExplorerFactory = modelExplorerFactory;
            _modelTagResolver = modelTemplateTagResolver;
        }

        /// <summary>
        /// Renders the <see cref="ContentArea"/> to the writer on the current <see cref="HtmlHelper"/>.
        /// </summary>
        /// <param name="htmlHelper">The Html helper</param>
        /// <param name="contentArea">The content area</param>
        public virtual void Render(IHtmlHelper htmlHelper, ContentArea contentArea)
        {
            if (contentArea == null || contentArea.IsEmpty)
            {
                return;
            }

            var viewContext = htmlHelper.ViewContext;
            TagBuilder containerTag = null;

            // Render a start tag for the container if not in edit mode and should render wrapping element.
            if (!IsInEditMode() && ShouldRenderWrappingElement(htmlHelper))
            {
                containerTag = new TagBuilder(GetContentAreaHtmlTag(htmlHelper, contentArea));

                // Apply the CSS class defined in the template to the container element.
                AddNonEmptyCssClass(containerTag, viewContext.ViewData[RenderSettings.CssClass] as string);

                viewContext.Writer.Write(containerTag.RenderStartTag());
            }

            RenderContentAreaItems(htmlHelper, FilteredItems(contentArea));

            // If we rendered a begin tag then we must render the end tag.
            if (containerTag != null)
            {
                viewContext.Writer.Write(containerTag.RenderEndTag());
            }
        }

        /// <summary>
        /// Render the <see cref="ContentAreaItem"/>s.
        /// </summary>
        /// <param name="htmlHelper">The html helper</param>
        /// <param name="contentAreaItems">The content area items to render</param>
        protected virtual void RenderContentAreaItems(IHtmlHelper htmlHelper, IEnumerable<ContentAreaItem> contentAreaItems)
        {
            foreach (var contentAreaItem in contentAreaItems)
            {
                RenderContentAreaItem(
                    htmlHelper,
                    contentAreaItem,
                    GetContentAreaItemTemplateTag(htmlHelper, contentAreaItem),
                    GetContentAreaItemHtmlTag(htmlHelper, contentAreaItem),
                    GetContentAreaItemCssClass(htmlHelper, contentAreaItem));
            }
        }

        /// <summary>
        /// Render a <see cref="ContentAreaItem"/>.
        /// </summary>
        /// <param name="htmlHelper">The html helper</param>
        /// <param name="contentAreaItem">The content area item to render</param>
        /// <param name="templateTag">The template tag used to resolve the display template</param>
        /// <param name="htmlTag">The html tag for the element wrapping the display template</param>
        /// <param name="cssClass">The css class for the element wrapping the display template</param>
        protected virtual void RenderContentAreaItem(IHtmlHelper htmlHelper, ContentAreaItem contentAreaItem, string templateTag, string htmlTag, string cssClass)
        {
            var renderSettings = new Dictionary<string, object>
            {
                [RenderSettings.ChildrenCustomTag] = htmlTag,
                [RenderSettings.ChildrenCssClass] = cssClass,
                [RenderSettings.Tag] = templateTag
            };

            if (contentAreaItem.RenderSettings != null)
            {
                // Merge the rendersettings from the content area with the render settings for the fragment
                renderSettings = contentAreaItem.RenderSettings.Concat(renderSettings.Where(r => !contentAreaItem.RenderSettings.ContainsKey(r.Key)))
                            .ToDictionary(r => r.Key, r => r.Value);
            }
            // Store the render settings in the view bag.
            htmlHelper.ViewBag.RenderSettings = renderSettings;

            var content = _contentAreaLoader.Get(contentAreaItem);

            if (content == null)
            {
                return;
            }

            var tags = Enumerable.Empty<string>();
            if (IsMethodsOverriden)
            {
                //if partner has overriden our methods we run with partner code instead of default
                var contentAreaTag = GetContentAreaTemplateTag(htmlHelper);
                tags = string.IsNullOrWhiteSpace(templateTag)
                    ? new[] { contentAreaTag }
                    : (_contentAreaRenderingOptions.TemplateTagSelectionStrategy == MissingTemplateTagSelectionStrategy.NoTag || string.IsNullOrEmpty(contentAreaTag)
                        ? new[] { templateTag }
                        : new[] { templateTag, contentAreaTag });
            }
            else
            {
                tags = _modelTagResolver.Resolve(_modelExplorerFactory.CreateFromModel(contentAreaItem), htmlHelper.ViewContext);
            }

            // Resolve the template for the content fragment based on the given template tag.
            var templateModel = ResolveTemplate(htmlHelper, content, tags);
            // If there is no template and not in edit mode then don't render.
            if (templateModel == null && !IsInEditMode())
            {
                return;
            }

            using (new ContentRenderingScope(htmlHelper.ViewContext.HttpContext, content, templateModel, tags))
            {
                var tagBuilder = new TagBuilder(htmlTag);

                AddNonEmptyCssClass(tagBuilder, cssClass);

                // Applies the required edit attributes to the fragment.
                tagBuilder.MergeAttributes(_attributeAssembler.GetAttributes(contentAreaItem, IsInEditMode(), templateModel != null));

                // Allow partners to modify the start tag before rendering.
                BeforeRenderContentAreaItemStartTag(tagBuilder, contentAreaItem);

                htmlHelper.ViewContext.Writer.Write(tagBuilder.RenderStartTag());

                // Render the content
                htmlHelper.RenderContentData(content, true, templateModel, _contentRenderer);

                htmlHelper.ViewContext.Writer.Write(tagBuilder.RenderEndTag());
            }
        }

        /// <summary>
        /// Executed before the <see cref="ContentFragment"/> start tag has been written. Use this to modify the <see cref="ContentFragment"/>s start tag.
        /// </summary>
        /// <param name="tagBuilder">The tag builder</param>
        /// <param name="contentAreaItem">The content area item</param>
        protected virtual void BeforeRenderContentAreaItemStartTag(TagBuilder tagBuilder, ContentAreaItem contentAreaItem)
        {
        }


        /// <summary>
        /// Determine whether a container element should be rendered around the content fragments. Looks in the view data for the 
        /// RenderSettings.HasContainerElement setting. Defaults to true if not defined.
        /// </summary>
        /// <param name="htmlHelper">The html helper</param>
        /// <returns>The value set in the view data; otherwise true</returns>
        protected virtual bool ShouldRenderWrappingElement(IHtmlHelper htmlHelper)
        {
            var hasContainer = (bool?)htmlHelper.ViewContext.ViewData[RenderSettings.HasContainerElement];
            return !hasContainer.HasValue || hasContainer.Value;
        }

        /// <summary>
        /// Resolve the template for the <see cref="ContentAreaItem" /> based on the template tag.
        /// </summary>
        /// <param name="htmlHelper">The html helper</param>
        /// <param name="content">The content.</param>
        /// <param name="templateTags">The template tags used to resolve the display template</param>
        /// <returns>
        /// A template model
        /// </returns>
        protected virtual TemplateModel ResolveTemplate(IHtmlHelper htmlHelper, IContent content, IEnumerable<string> templateTags)
        {
            Validator.ThrowIfNull(nameof(content), content);

            return _templateResolver.Resolve(content,
                                      content.GetOriginalType(),
                                      TemplateTypeCategories.MvcPartial,
                                      templateTags);
        }

        /// <summary>
        /// Gets the template tag for the <see cref="ContentArea"/>. Looks in the view data for the RenderSettings.Tag setting.
        /// </summary>
        /// <param name="htmlHelper">The html helper</param>
        /// <returns>The template tag</returns>
        protected virtual string GetContentAreaTemplateTag(IHtmlHelper htmlHelper)
        {
            return htmlHelper.ViewContext.ViewData[RenderSettings.Tag] as string;
        }

        /// <summary>
        /// Gets the template tag for the <see cref="ContentAreaItem"/>. If a tag is defined on the display options for
        /// the <see cref="ContentAreaItem"/> then that is used; otherwise it uses the content areas template tag.
        /// </summary>
        /// <param name="htmlHelper">The html helper</param>
        /// <param name="contentAreaItem">The content area item</param>
        /// <returns>The template tag for the content area item</returns>
        protected virtual string GetContentAreaItemTemplateTag(IHtmlHelper htmlHelper, ContentAreaItem contentAreaItem)
        {
            var displayOption = _contentAreaLoader.LoadDisplayOption(contentAreaItem);
            if (displayOption != null)
            {
                return displayOption.Tag;
            }

            return GetContentAreaTemplateTag(htmlHelper);
        }

        /// <summary>
        /// Gets the html tag for the container. Looks in the view data for the RenderSettings.CustomTag setting.
        /// </summary>
        /// <param name="htmlHelper">The html helper</param>
        /// <param name="contentArea">The content area</param>
        /// <returns>The container html tag</returns>
        protected virtual string GetContentAreaHtmlTag(IHtmlHelper htmlHelper, ContentArea contentArea)
        {
            var tagName = htmlHelper.ViewContext.ViewData[RenderSettings.CustomTag] as string;

            return string.IsNullOrEmpty(tagName) ? "div" : tagName;
        }

        /// <summary>
        /// Gets the html tag for the content area item. Looks in the view data for the RenderSettings.ChildrenCustomTag setting.
        /// </summary>
        /// <param name="htmlHelper">The html helper</param>
        /// <param name="contentAreaItem">The content area item</param>
        /// <returns>The html tag for the content area item</returns>
        protected virtual string GetContentAreaItemHtmlTag(IHtmlHelper htmlHelper, ContentAreaItem contentAreaItem)
        {
            var tagName = htmlHelper.ViewContext.ViewData[RenderSettings.ChildrenCustomTag] as string;
            if (string.IsNullOrEmpty(tagName))
            {
                tagName = "div";
            }

            return tagName;
        }

        /// <summary>
        /// Gets the css class for the content area item. Looks in the view data for the RenderSettings.ChildrenCssClass setting.
        /// </summary>
        /// <param name="htmlHelper">The html helper</param>
        /// <param name="contentAreaItem">The content area item</param>
        /// <returns>The css class for the content area item</returns>
        protected virtual string GetContentAreaItemCssClass(IHtmlHelper htmlHelper, ContentAreaItem contentAreaItem)
        {
            return htmlHelper.ViewContext.ViewData[RenderSettings.ChildrenCssClass] as string;
        }

        /// <summary>
        /// Determines if the request context is in edit mode.
        /// </summary>
        /// <returns><code>true</code> if the context is in edit mode; otherwise <code>false</code></returns>
        protected virtual bool IsInEditMode() => _contextModeResolver.CurrentMode == ContextMode.Edit;

        /// <summary>
        /// Determines if the request context is in edit mode.
        /// </summary>
        /// <param name="htmlHelper">The html helper</param>
        /// <returns><code>true</code> if the context is in edit mode; otherwise <code>false</code></returns>
        [Obsolete("Use method that does not take HtmlHelper")]
        protected virtual bool IsInEditMode(IHtmlHelper htmlHelper) => _contextModeResolver.CurrentMode == ContextMode.Edit;

        /// <summary>
        /// Add a css class to the <see cref="TagBuilder"/> if it is not null or white space.
        /// </summary>
        /// <param name="tagBuilder">The tag builder</param>
        /// <param name="cssClass">The class to addd</param>
        /// <returns>The given tag builder</returns>
        protected virtual TagBuilder AddNonEmptyCssClass(TagBuilder tagBuilder, string cssClass)
        {
            if (string.IsNullOrWhiteSpace(cssClass))
            {
                return tagBuilder;
            }
            tagBuilder.AddCssClass(cssClass);

            return tagBuilder;
        }

        private static IEnumerable<ContentAreaItem> FilteredItems(ContentArea contentArea)
        {
            return contentArea.Fragments.GetFilteredFragments(PrincipalInfo.CurrentPrincipal).OfType<ContentFragment>().Select(f => new ContentAreaItem(f));
        }

        private bool IsMethodsOverriden
        {
            get
            {
                if (!_isMethodsOverriden.HasValue)
                {
                    var isTemplateTagOverriden = GetType().GetMethod(nameof(GetContentAreaTemplateTag), BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(IHtmlHelper) }, null).DeclaringType != typeof(ContentAreaRenderer);
                    var isItemTemplateTagOverriden = GetType().GetMethod(nameof(GetContentAreaItemTemplateTag), BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(IHtmlHelper), typeof(ContentAreaItem) }, null).DeclaringType != typeof(ContentAreaRenderer);
                    _isMethodsOverriden = isTemplateTagOverriden || isItemTemplateTagOverriden;
                }
                return _isMethodsOverriden.Value;
            }
        }

    }
}

