using EPiServer.Shell;

namespace RenderingLayoutProcessor.Block
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
