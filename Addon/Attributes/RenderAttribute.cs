using System;

namespace RenderingLayoutProcessor.Attributes
{
    public class RenderAttribute : Attribute
    {
        private string attributeName;
        private bool renderIfEmpty = false;
        public RenderAttribute(string attributeName = null, bool renderIfEmpty = false)
        {
            this.attributeName = attributeName;
            this.renderIfEmpty = renderIfEmpty;
        }

        public virtual string AttributeName
        {
            get { return attributeName; }
        }

        public virtual bool RenderIfEmpty
        {
            get { return renderIfEmpty; }
        }
    }
}
