using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using Moq;
using AddOn.Optimizely.ContentAreaLayout.Context;
using Xunit;

namespace Tests
{
    public class MultiColumnContentAreaRendererTests : TestBase
    {
        private const Mock<TestRenderingContentAreaContext> ContentContext = (Mock<TestRenderingContentAreaContext>)null;

        [Fact]
        public void ShouldWrapItemInDivIfAttributesExist()
        {
            var (renderer, contentAreaItems, htmlHelper, writer) = SetupRenderer(
                attributes: new Dictionary<string, string> { { "attr1", "attr1_value" } });

            renderer.RenderContentAreaItemsInternal(htmlHelper, contentAreaItems);

            Assert.Equal(@"<div attr1=""attr1_value""></div>", writer.ToString());
        }

        [Fact]
        public void ShouldCloseContainerIfCanNestUnderIsFalse()
        {
            var contexts = new List<Mock<TestRenderingContentAreaContext>>
            {
                SetupRenderingContext(true, false),
                SetupRenderingContext(true, false)
            };

            var (renderer, contentAreaItems, htmlHelper, writer) = SetupRenderer(
                contexts.Select(c => new Mock<TestLayoutBlock>(c.Object) { CallBase = true }.Object));

            renderer.RenderContentAreaItemsInternal(htmlHelper, contentAreaItems);

            Assert.Equal("<c_1></c_1><c_2></c_2>", writer.ToString());
        }

        [Fact]
        public void ShouldNestContainerIfCanNestUnderIsTrue()
        {
            var contexts = new List<Mock<TestRenderingContentAreaContext>>
            {
                SetupRenderingContext(true, false),
                SetupRenderingContext(true, true)
            };

            var (renderer, contentAreaItems, htmlHelper, writer) = SetupRenderer(
                contexts.Select(c => new Mock<TestLayoutBlock>(c.Object) { CallBase = true }.Object));

            renderer.RenderContentAreaItemsInternal(htmlHelper, contentAreaItems);

            Assert.Equal("<c_1><c_2></c_2></c_1>", writer.ToString());
        }

        [Fact]
        public void ShouldClearNestingUntilCanNestUnderIsTrue()
        {
            var contexts = new List<Mock<TestRenderingContentAreaContext>>();
            contexts.Add(SetupRenderingContext(true, true));
            contexts.Add(SetupRenderingContext(true, true));
            contexts.Add(SetupRenderingContext(true, true));
            contexts.Add(SetupRenderingContext(_ => true, c => c == contexts[0].Object));

            var (renderer, contentAreaItems, htmlHelper, writer) = SetupRenderer(
                contexts.Select(c => new Mock<TestLayoutBlock>(c.Object) { CallBase = true }.Object));

            renderer.RenderContentAreaItemsInternal(htmlHelper, contentAreaItems);

            Assert.Equal("<c_1><c_2><c_3></c_3></c_2><c_4></c_4></c_1>", writer.ToString());
        }

        [Fact]
        public void ShouldClearNestingUntilCanContainIsTrue()
        {
            var contexts = new List<Mock<TestRenderingContentAreaContext>>();
            contexts.Add(SetupRenderingContext(true, true));
            contexts.Add(SetupRenderingContext(c => c == contexts[2].Object, _ => true));
            contexts.Add(SetupRenderingContext(false, true));
            contexts.Add(SetupRenderingContext(_ => true, c => c == contexts[0].Object));

            var (renderer, contentAreaItems, htmlHelper, writer) = SetupRenderer(
               contexts.Select(c => new Mock<TestLayoutBlock>(c.Object) { CallBase = true }.Object));

            renderer.RenderContentAreaItemsInternal(htmlHelper, contentAreaItems);

            Assert.Equal("<c_1><c_2><c_3></c_3></c_2><c_4></c_4></c_1>", writer.ToString());
        }

        [Fact]
        public void ShouldRenderContentInContainer()
        {
            var contexts = new List<Mock<TestRenderingContentAreaContext>>();
            contexts.Add(SetupRenderingContext(true, true));
            contexts.Add(ContentContext);

            var (renderer, contentAreaItems, htmlHelper, writer) = SetupRenderer(contexts.Select(c => (
                    c == ContentContext
                        ? new BasicContent { ContentLink = new ContentReference(1) }
                        : new Mock<TestLayoutBlock>(c.Object) { CallBase = true }.Object))
                );

            renderer.RenderContentAreaItemsInternal(htmlHelper, contentAreaItems);

            Assert.Equal("<c_1><iw_1><i></i></iw_1></c_1>", writer.ToString());
        }

        [Fact]
        public void ShouldCloseContainerIfActionIsClose()
        {
            var (renderer, contentAreaItems, htmlHelper, writer) = SetupRenderer(
                new List<IContent>
                {
                    new Mock<TestLayoutBlock>(SetupRenderingContext(true, true, RenderingProcessorAction.Close).Object) {CallBase = true}.Object,
                    new BasicContent { ContentLink = new ContentReference(1)},
                    new Mock<TestLayoutBlock>( SetupRenderingContext(true, true, RenderingProcessorAction.Close).Object) {CallBase = true}.Object,
                    new BasicContent { ContentLink = new ContentReference(2)}
                }
            );

            renderer.RenderContentAreaItemsInternal(htmlHelper, contentAreaItems);

            Assert.Equal("<c_1><iw_1><i></i></iw_1></c_1><c_2><iw_2><i></i></iw_2></c_2>", writer.ToString());
        }
    }
}