using System;

namespace AddOn.Optimizely.ContentAreaLayout.Attributes
{
    public class BlockRenderingMetadataAttributeAttribute : Attribute
    {
        private string name;
        public BlockRenderingMetadataAttributeAttribute(string name = null)
        {
            this.name = name;
        }

        public virtual string Name
        {
            get { return name; }
        }
    }
}