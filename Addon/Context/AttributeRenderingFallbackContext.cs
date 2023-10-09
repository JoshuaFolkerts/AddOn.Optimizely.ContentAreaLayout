using AddOn.Optimizely.ContentAreaLayout.Extension;
using AddOn.Optimizely.ContentAreaLayout.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AddOn.Optimizely.ContentAreaLayout.Context
{
    public class AttributeRenderingContentAreaFallbackContext : IRenderingContentAreaFallbackContext
    {
        private TagBuilder contentAreaContainer;
        private string[] excludeAttributes = { "tag", "cssclass" };
        
        public void ContainerOpen(IHtmlHelper htmlHelper, BlockRenderingMetadata blockMetadata)
        {
            // Create the content area wrapping div
            contentAreaContainer = new TagBuilder("div");
            contentAreaContainer.Attributes.Add("data-contentarea", String.Empty);
            contentAreaContainer.Attributes.Add("data-contentarea-children", blockMetadata.ParentMetadata.Children.ToString());
            if (htmlHelper.ViewContext.ViewData.ContainsKey("ContainerCssClass"))
            {
                contentAreaContainer.Attributes.Add("class", htmlHelper.ViewContext.ViewData["ContainerCssClass"].ToString());
            }
            
            // If a viewbag value that contains any property that is not excluded we add it as a data attribute
            var contentAreaViewDataAttributes = htmlHelper.ViewContext.ViewData
                .Where(x => !excludeAttributes.Contains(x.Key.ToLowerInvariant())).ToList();
            
            foreach (var viewDataAttribute in contentAreaViewDataAttributes)
            {
                var keyName = Regex.Split(viewDataAttribute.Key, @"(?<!^)(?=[A-Z])");
                contentAreaContainer.Attributes.Add($"data-{string.Join("-", keyName).ToLowerInvariant()}", viewDataAttribute.Value?.ToString() ?? string.Empty);
            }
        
            // If a viewbag value that contains "layout" we add some general layout attributes to the contentarea div
            if (contentAreaViewDataAttributes.Any(x => x.Key.ToLowerInvariant().Contains("layout")))
            {
                contentAreaContainer.Attributes.Add("data-layout", string.Empty);
                contentAreaContainer.Attributes.Add("data-layout-children",  blockMetadata.ParentMetadata.Children.ToString());
            }
        
            contentAreaContainer.RenderOpenTo(htmlHelper);
        }

        public void ContainerClose(IHtmlHelper htmlHelper)
        {
            contentAreaContainer?.RenderCloseTo(htmlHelper);
        }
    }
}