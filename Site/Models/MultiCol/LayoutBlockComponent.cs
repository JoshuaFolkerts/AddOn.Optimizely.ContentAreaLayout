using EPiServer;
using EPiServer.Editor;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using RenderingLayoutProcessor.Models;

namespace Site.Models.MultiCol
{
    [TemplateDescriptor(AvailableWithoutTag = true, Inherited = true)]
    public class LayoutBlockComponent : BlockComponent<RenderingLayoutBlock>
    {
        protected override IViewComponentResult InvokeComponent(RenderingLayoutBlock currentContent)
        {
            return PageEditing.PageIsInEditMode
                ? View($"~/Views/Layouts/{currentContent.GetOriginalType().Name}.cshtml", currentContent)
                : Content(string.Empty);
        }
    }
}