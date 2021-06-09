using System.Web;
using System.Web.Mvc;

namespace RenderingLayoutProcessor.Impl
{
    public static class TagBuilderExtensions
    {
        public static void WriteStartTo(this TagBuilder builder, HtmlHelper helper) =>
            helper.ViewContext.Writer.Write(builder.ToString(TagRenderMode.StartTag));

        public static void WriteEndTo(this TagBuilder builder, HtmlHelper helper) =>
            helper.ViewContext.Writer.Write(builder.ToString(TagRenderMode.EndTag));

        public static void RenderOpenTo(this TagBuilder builder, HtmlHelper helper)
            => helper.ViewContext.Writer.Write(builder.ToString(TagRenderMode.StartTag));

        public static void RenderCloseTo(this TagBuilder builder, HtmlHelper helper)
            => helper.ViewContext.Writer.Write(builder.ToString(TagRenderMode.EndTag));

        public static void RenderTagTo(this TagBuilder builder, HtmlHelper helper)
            => helper.ViewContext.Writer.Write(builder.ToString(TagRenderMode.Normal));

        public static void RenderSelfClosingTagTo(this TagBuilder builder, HtmlHelper helper)
         => helper.ViewContext.Writer.Write(builder.ToString(TagRenderMode.SelfClosing));

        public static string Html(this string value)
            => string.IsNullOrEmpty(value) ? string.Empty : HttpUtility.HtmlEncode(value);
    }
}