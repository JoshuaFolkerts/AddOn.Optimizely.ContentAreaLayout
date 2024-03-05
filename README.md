AddOn.Optimizely.ContentAreaLayout is an extention to Episerver/Optimizely that makes it possible to add layout blocks that will control how the next x blocks will be displayed.


## Usage


1. Register the ContentAreaLayoutRenderer in Startup.cs

```cs
services.AddTransient<ContentAreaRenderer, ContentAreaLayoutRenderer>();
```

If one wishes to have a fallback behaviour when the content area does not contain any layout blocks this can also be registered during startup by adding a generic type pointing top a class that implements **IRenderingContentAreaFallbackContext**


```cs
services.AddTransient<ContentAreaRenderer, ContentAreaLayoutRenderer<MyFallbackContext>>();
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

## Meta data for content area items
Each block gets a meta data object assigned to it's ViewBag. 

A block can pick up the meta data by using the following HTMLHelper in a cshtml view.
```csharp
@{
var metadata = Html.BlockMetadata();
}
```

[You can see the meta data model in BlockRenderingMetaData.cs](https://github.com/JoshuaFolkerts/AddOn.Optimizely.ContentAreaLayout/blob/master/Addon/Models/BlockRenderingMetaData.cs)

## Expanding meta data from content models
The meta data can be expanded by adding the following attribute to properties within content models
```csharp
[BlockRenderingMetadataAttribute("aria-label")]
public virtual string Label { get; set; }
```

The following value will be added to the BlockRenderingMetaData model's **Properties** dictionary.

It can be used to output the value in markup
```
@{
var metadata = Html.BlockMetadata();
}
<div aria-label="@metamodel.Properties.GetValueOrDefault("aria-label", string.Empty)">
</div>
```



## Test site

login with:

**admin**

**%1yylyh?pv6E**
