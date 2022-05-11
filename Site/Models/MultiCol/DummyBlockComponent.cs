using EPiServer.Framework.DataAnnotations;
using EPiServer.Web.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace Site.Models.MultiCol
{
    [TemplateDescriptor(AvailableWithoutTag = true, Inherited = true)]
    public class DummyBlockComponent : BlockComponent<DummyBlock>
    {
        protected override IViewComponentResult InvokeComponent(DummyBlock currentContent)
        {
            return View($"~/Views/Shared/Blocks/DummyBlock.cshtml", currentContent);
        }
    }
}