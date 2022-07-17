using EPiServer.Framework.DataAnnotations;
using EPiServer.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Site.Models;

namespace Site.Controllers
{
    /// <summary>
    /// Concrete controller that handles all page types that don't have their own specific controllers.
    /// </summary>
    /// <remarks>
    /// Note that as the view file name is hard coded it won't work with DisplayModes (ie Index.mobile.cshtml).
    /// For page types requiring such views add specific controllers for them. Alternatively the Index action
    /// could be modified to set ControllerContext.RouteData.Values["controller"] to type name of the currentPage
    /// argument. That may however have side effects.
    /// </remarks>
    [TemplateDescriptor(Inherited = true)]
    public class StandardPageController : PageController<StandardPage>
    {
        public ViewResult Index(StandardPage currentPage)
        {
            return currentPage.CssFramework switch
            {
                CssFramework.Bootstrap => View($"~/Views/StandardPage/Bootstrap.cshtml", currentPage),
                CssFramework.Foundation => View($"~/Views/StandardPage/Foundation.cshtml", currentPage),
                _ => View($"~/Views/StandardPage/NoFramework.cshtml", currentPage),
            };
        }
    }
}