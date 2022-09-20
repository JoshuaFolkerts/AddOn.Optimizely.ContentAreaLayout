AddOn.Optimizely.ContentAreaLayout is an extention to Episerver/Optimizely that makes it possible to add layout blocks that will control how the next x blocks will be displayed.


## Usage


1. Register the ContentAreaLayoutRenderer in Startup.cs

```cs
services.AddTransient<ContentAreaRenderer, ContentAreaLayoutRenderer>();
```

2. Create a layout block inheriting from RenderingLayoutBlock with properties that describes how the grids containers should render. This needs to implement the **IRenderingLayoutBlock** interface, the easiest way by inheriting from the **RenderingLayoutBlock** base class.

 In it's simplest form this could be:
```cs
    [ContentType(DisplayName = "BootstrapLayoutBlock", GUID = "413b7a71-8490-4f22-bddc-77325c53424f", Description = "")]
    public class BootstrapLayoutBlock : RenderingLayoutBlock
    {
        public override IRenderingContentAreaContext NewContext() =>
            new BootstrapRenderingContext(new[] { 2, 8, 2 });
    }
```
This example will render a grid with 2 columns, 8 columns and 2 columns, by having this reading from a property of the block you can easily add control over grid sizes, padding colors etc.

3. Choose or tweak/implement a RenderingContext. This class is what controls how the layout will be rendered.

Using the **BootstrapRenderingContext** class will do the heavy lifting of adding the grid classes to the content area blocks and containers. If this does not suit your needs you can create your own context class either inheriting from **GridRenderingContext** or straight from **IRenderingContentAreaContext**.

The context interface gives the possibility to control how and if a context can be nested into the previous context by implementing:
```cs
        bool CanContain(IRenderingContentAreaContext childContext);

        bool CanNestUnder(IRenderingContentAreaContext parentContext);
```
And how it should render the grid and items with:
```cs
        void ItemOpen(IHtmlHelper htmlHelper);

        void ItemClose(IHtmlHelper htmlHelper);

        RenderingProcessorAction RenderItem(IHtmlHelper htmlHelper, ContentAreaItem current, Action renderItem);

        void ContainerClose(IHtmlHelper htmlHelper);

        void ContainerOpen(IHtmlHelper htmlHelper);
```

## Test site

login with:

**admin**

**%1yylyh?pv6E**
