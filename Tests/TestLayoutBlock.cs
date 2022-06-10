using System;
using EPiServer.Core;
using AddOn.Optimizely.ContentAreaLayout;

namespace Tests
{
    public class TestLayoutBlock : BasicContent, IRenderingLayoutBlock
    {
        private readonly IRenderingContentAreaContext _context;

        public TestLayoutBlock(IRenderingContentAreaContext context) : this(context, 100)
        {
        }

        public TestLayoutBlock(IRenderingContentAreaContext context, int id)
        {
            _context = context;
            base.ContentLink = new ContentReference(id);
        }

        public virtual IRenderingContentAreaContext NewContext() => _context;
    }
}