using EPiServer.Shell;

namespace AddOn.Optimizely.ContentAreaLayout.Block
{
    [UIDescriptorRegistration]
    public class LayoutBlockUiDescriptor : UIDescriptor<IRenderingLayoutBlock>
    {
        public LayoutBlockUiDescriptor()
        {
            IconClass = "epi-iconLayout";
        }
    }
}