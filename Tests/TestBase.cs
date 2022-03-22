using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Framework.Web;
using EPiServer.Web;
using EPiServer.Web.Internal;
using EPiServer.Web.Mvc;
using EPiServer.Web.Templating;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Moq;
using RenderingLayoutProcessor.Impl;

namespace Tests
{
    public abstract class TestBase
    {
        private int _containerIdSource = 1;
        
        protected Mock<TestRenderingContentAreaContext> SetupRenderingContext(bool canContain, bool canNestUnder, RenderingProcessorAction action = RenderingProcessorAction.Continue)
        {
            return SetupRenderingContext<TestRenderingContentAreaContext>(_ => canContain, _ => canNestUnder, action);
        }
        
        protected Mock<TestRenderingContentAreaContext> SetupRenderingContext(Func<IRenderingContentAreaContext, bool> canContain, Func<IRenderingContentAreaContext, bool> canNestUnder, RenderingProcessorAction action = RenderingProcessorAction.Continue)
        {
            return SetupRenderingContext<TestRenderingContentAreaContext>(canContain, canNestUnder, action);
        }
        
        protected Mock<T> SetupRenderingContext<T>(bool canContain, bool canNestUnder, RenderingProcessorAction action = RenderingProcessorAction.Continue) where T : class, IRenderingContentAreaContext
        {
            return SetupRenderingContext<T>(_ => canContain, _ => canNestUnder, action);
        }
        
        protected Mock<T> SetupRenderingContext<T>(Func<IRenderingContentAreaContext, bool> canContain, Func<IRenderingContentAreaContext, bool> canNestUnder, RenderingProcessorAction action = RenderingProcessorAction.Continue) where T : class, IRenderingContentAreaContext
        {
            var context = new Mock<T>(_containerIdSource++, action) { CallBase = true };
            context.Setup(x => x.CanContain(It.IsAny<IRenderingContentAreaContext>())).Returns(canContain);
            context.Setup(x => x.CanNestUnder(It.IsAny<IRenderingContentAreaContext>())).Returns(canNestUnder);

            return context;
        }
        
        protected (MultiColumnContentAreaRenderer, List<ContentAreaItem>, IHtmlHelper, StringWriter) SetupRenderer(IEnumerable<IContent> contentItems = null, IDictionary<string, string> attributes = null)
        {
            var contentRenderer = new Mock<IContentRenderer>();
            var templateResolver = new Mock<ITemplateResolver>();
            var contentAreaItemAttributeAssembler = new Mock<IContentAreaItemAttributeAssembler>();
            var contentRepository = new Mock<IContentRepository>();
            var contentAreaLoader = new Mock<IContentAreaLoader>();
            var contextModeResolver = new Mock<IContextModeResolver>();
            var contentAreaRenderingOptions = new Mock<ContentAreaRenderingOptions>();
            var modelMetadataProvider = new Mock<IModelMetadataProvider>();
            var modelExplorerFactory = new Mock<ModelExplorerFactory>(modelMetadataProvider.Object);
            var modelTemplateTagResolver = new Mock<IModelTemplateTagResolver>();

            var htmlHelper = new Mock<IHtmlHelper>();
            var httpContext = new Mock<HttpContext>();
            var writer = new StringWriter();
            httpContext.Setup(x => x.Items)
                .Returns(new Dictionary<object, object?>
                {
                    {"Epi:ContentRenderingContext", new ContentRenderingContext(null)}, 
                    {"Epi:ContentAreaStack", null}
                });
            
            var viewContext = new ViewContext {HttpContext = httpContext.Object, Writer = writer, RouteData = new RouteData()};
            
            htmlHelper.Setup(x => x.ViewContext).Returns(viewContext);
            htmlHelper.Setup(x => x.ViewData).Returns(viewContext.ViewData);
            htmlHelper.Setup(x => x.ViewBag).Returns(new ExpandoObject());

            templateResolver.Setup(x =>
                    x.ResolveAll( It.IsAny<object>(), It.IsAny<Type>(), It.IsAny<TemplateTypeCategories>(), It.IsAny<IEnumerable<string>>()))
                .Returns(new List<TemplateModel> {new Mock<TemplateModel>().Object});

            var contentAreaItems = new List<Mock<ContentAreaItem>>();
            foreach (var content in contentItems ?? new List<IContent> {new BasicContent {ContentLink = new ContentReference(100)}})
            {
                var contentAreaItem = new Mock<ContentAreaItem>();
                contentAreaLoader.Setup(x => x.Get(contentAreaItem.Object))
                    .Returns(content);
                contentAreaItems.Add(contentAreaItem);
            }
            
            contentAreaLoader.Setup(x => x.LoadDisplayOption(It.IsAny<ContentAreaItem>()))
                .Returns((DisplayOption)null);

            contextModeResolver.Setup(x => x.CurrentMode).Returns(ContextMode.Default);

            contentAreaItemAttributeAssembler.Setup(x =>
                    x.GetAttributes(It.IsAny<ContentAreaItem>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(attributes ?? new Dictionary<string, string>());

            modelTemplateTagResolver.Setup(x => x.Resolve(It.IsAny<ModelExplorer>(), It.IsAny<ViewContext>()))
                .Returns(new List<string>());
            
            var renderer = new MultiColumnContentAreaRenderer(
                contentRenderer.Object,
                templateResolver.Object,
                contentAreaItemAttributeAssembler.Object,
                contentRepository.Object,
                contentAreaLoader.Object,
                contextModeResolver.Object,
                contentAreaRenderingOptions.Object,
                modelExplorerFactory.Object,
                modelTemplateTagResolver.Object);

            return (renderer, contentAreaItems.Select(c => c.Object).ToList(), htmlHelper.Object, writer);
        }
    }
}