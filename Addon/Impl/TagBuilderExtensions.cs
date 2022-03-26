using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using System.Text.Encodings.Web;

namespace RenderingLayoutProcessor.Impl
{
    public static class TagBuilderExtensions
    {
        public static void WriteStartTo(this TagBuilder builder, IHtmlHelper helper) =>
            builder.RenderStartTag().WriteTo(helper.ViewContext.Writer, HtmlEncoder.Default);

        public static void WriteEndTo(this TagBuilder builder, IHtmlHelper helper) =>
            builder.RenderEndTag().WriteTo(helper.ViewContext.Writer, HtmlEncoder.Default);

        public static void RenderOpenTo(this TagBuilder builder, IHtmlHelper helper)
            => builder.RenderStartTag().WriteTo(helper.ViewContext.Writer, HtmlEncoder.Default);

        public static void RenderCloseTo(this TagBuilder builder, IHtmlHelper helper)
            => builder.RenderEndTag().WriteTo(helper.ViewContext.Writer, HtmlEncoder.Default);

        public static void RenderTagTo(this TagBuilder builder, IHtmlHelper helper)
            => builder.WriteTo(helper.ViewContext.Writer, HtmlEncoder.Default);

        public static void RenderSelfClosingTagTo(this TagBuilder builder, IHtmlHelper helper)
            => builder.RenderSelfClosingTag().WriteTo(helper.ViewContext.Writer, HtmlEncoder.Default);

        public static string Html(this string value)
            => string.IsNullOrEmpty(value) ? string.Empty : WebUtility.HtmlEncode(value);
    }
}