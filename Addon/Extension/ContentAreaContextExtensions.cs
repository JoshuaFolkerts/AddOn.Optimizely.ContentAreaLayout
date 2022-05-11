using System.Collections.Generic;

namespace RenderingLayoutProcessor.Extension
{
    public static class ContentAreaContextExtensions
    {
        public static IEnumerable<IRenderingContentAreaContext> ContextStack(this IRenderingContentAreaContext area)
        {
            while (area != null)
            {
                yield return area;
                area = area.ParentContext;
            }
        }
    }
}