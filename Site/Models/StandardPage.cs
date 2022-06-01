using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Shell.ObjectEditing;
using RenderingLayoutProcessor.EditorDescriptor;

namespace Site.Models
{
    /// <summary>
    /// Used for the pages mainly consisting of manually created content such as text, images, and blocks
    /// </summary>
    [ContentType(GUID = "9CCC8A41-5C8C-4BE0-8E73-520FF3DE8267")]
    [ImageUrl("~/static/page-type-thumbnail-standard.png")]
    public class StandardPage : PageData
    {
        [Display(
            GroupName = SystemTabNames.Content,
            Order = 310)]
        [CultureSpecific]
        public virtual XhtmlString MainBody { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 320)]
        public virtual ContentArea MainContentArea { get; set; }
        
        [Display(Name = "Css Framework", GroupName = SystemTabNames.Settings, Order = 10)]
        [SelectOne(SelectionFactoryType = typeof(EnumSelectionFactory<CssFramework>))]
        [BackingType(typeof(PropertyNumber))]
        public virtual CssFramework CssFramework { get; set; }
    }

    public enum CssFramework
    {
        Bootstrap,
        Foundation,
        NoFramework
    }
}
