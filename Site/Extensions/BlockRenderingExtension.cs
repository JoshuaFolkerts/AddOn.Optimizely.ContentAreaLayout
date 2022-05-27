using EPiServer.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Site.Helpers
{
    /// <summary>
    /// Extension methods for rendering blocks
    /// </summary>
    public static class BlockRenderingExtension
    {
        /// <summary>
        /// Outputs a dictionary as attributes
        /// </summary>
        /// <param name="dictionary">A <string, string> dictionary</param>
        /// <returns>HtmlString with the dictionary content as data attributes</returns>
        public static HtmlString ToAttributes(this Dictionary<string, string> dictionary)
        {
            if (dictionary is null)
            {
                return new HtmlString(string.Empty);
            }
            StringBuilder sb = new StringBuilder();
            foreach(var row in dictionary)
            {
                if (string.IsNullOrEmpty(row.Value))
                {
                    continue;
                }
                sb.AppendFormat("data-{0}=\"{1}\"", row.Key, row.Value);
            }
            return new HtmlString(sb.ToString());
        }
    }
}

