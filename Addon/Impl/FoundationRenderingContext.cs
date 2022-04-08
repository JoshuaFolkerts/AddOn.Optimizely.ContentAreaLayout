using System.Collections.Generic;

namespace RenderingLayoutProcessor.Impl
{
    public class FoundationRenderingContext : GridRenderingContext
    {
        public FoundationRenderingContext(int[] itemSizes = null) : base(itemSizes)
        {
        }

        protected override IEnumerable<string> GetRowClasses()
        {
            return new[] { "grid-x" };
        }

        protected override IEnumerable<string> GetColumnClasses()
        {
            var currentColumn = _itemSizes[_renderedItemsCount % _itemSizes.Length];

            _columnsRendered += currentColumn;

            return new[] { $"cell", $"medium-{currentColumn}" };
        }
    }
}