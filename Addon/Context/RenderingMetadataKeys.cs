namespace RenderingLayoutProcessor.Context
{
    public static class RenderingMetadataKeys
    {
        public static class Block
        {
            internal const string Index = "ContentAreaRenderer_Block_Index";
            internal const string ContentGuid = "ContentAreaRenderer_Block_ContentGuid";
            internal const string ContentLink = "ContentAreaRenderer_Block_ContentLink";
            internal const string Tag = "ContentAreaRenderer_Block_Tag";
        }

        public static class Layout
        {
            internal const string Index = "ContentAreaRenderer_Layout_Index";
            internal const string ContentGuid = "ContentAreaRenderer_Layout_ContentGuid";
            internal const string ContentLink = "ContentAreaRenderer_Layout_ContentLink";
            internal const string Tag = "ContentAreaRenderer_Layout_Tag";
        }

        public const string MetaData = "ContentAreaMetaData";
    }
}