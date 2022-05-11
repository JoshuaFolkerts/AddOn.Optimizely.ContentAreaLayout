using EPiServer;
using EPiServer.Editor;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Web;
using EPiServer.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using RenderingLayoutProcessor.Block;

namespace Site.Models.MultiCol
{
    [TemplateDescriptor(AvailableWithoutTag = true, Inherited = true)]
    public class LayoutBlockComponent : BlockComponent<RenderingLayoutBlock>
    {
        private readonly IContextModeResolver _contextModeResolver;

        public LayoutBlockComponent(IContextModeResolver contextModeResolver)
        {
            _contextModeResolver = contextModeResolver;
        }

        protected override IViewComponentResult InvokeComponent(RenderingLayoutBlock currentContent)
        {
            return _contextModeResolver.CurrentMode == ContextMode.Edit
                ? View($"~/Views/Layouts/{currentContent.GetOriginalType().Name}.cshtml", currentContent)
                : Content(string.Empty);
        }
    }
}