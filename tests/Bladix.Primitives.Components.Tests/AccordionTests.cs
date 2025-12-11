using System;
using Microsoft.AspNetCore.Components;
using Bladix.Primitives.Components.Accordion;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Bladix.Primitives.Components.Tests
{
    public class AccordionTests : IDisposable
    {
        private readonly BunitContext ctx;

        public AccordionTests()
        {
            ctx = new BunitContext();
            // Provide any JS interop fallbacks if tests later need them.
            // Most tests below avoid invoking real JS runtime.
            // Configure bUnit to handle the module import used by AccordionTrigger.
            var module = ctx.JSInterop.SetupModule("./_content/Bladix.Primitives/Components/Accordion/AccordionTrigger.razor.js");
            module.Setup<IJSObjectReference>("setupKeyboardNavigation", _ => true)
                  .SetResult(new FakeJSObjectReference());
        }

        public void Dispose() => ctx.Dispose();

        // Helper wrapper component: Single accordion with three items
        private sealed class SingleAccordionWrapper : ComponentBase
        {
            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                int seq = 0;
                builder.OpenComponent<AccordionRoot>(seq++);
                builder.AddAttribute(seq++, nameof(AccordionRoot.Type), AccordionType.Single);
                builder.AddAttribute(seq++, nameof(AccordionRoot.ChildContent), (RenderFragment)((rb) =>
                {
                    int s = 0;

                    // Item 1
                    rb.OpenComponent<AccordionItem>(s++);
                    rb.AddAttribute(s++, nameof(AccordionItem.Value), "item-1");
                    rb.AddAttribute(s++, nameof(AccordionItem.ChildContent), (RenderFragment)((b) =>
                    {
                        int i = 0;
                        b.OpenComponent<AccordionHeader>(i++);
                        b.AddAttribute(i++, nameof(AccordionHeader.ChildContent), (RenderFragment)((h) =>
                        {
                            int j = 0;
                            h.OpenComponent<AccordionTrigger>(j++);
                            h.AddAttribute(j++, nameof(AccordionTrigger.ChildContent), (RenderFragment)((t) =>
                            {
                                t.AddMarkupContent(0, "<span>Item 1</span>");
                            }));
                            h.CloseComponent();
                        }));
                        b.CloseComponent();

                        b.OpenComponent<AccordionContent>(i++);
                        b.AddAttribute(i++, nameof(AccordionContent.ChildContent), (RenderFragment)((c) =>
                        {
                            c.AddMarkupContent(0, "<div>Content 1</div>");
                        }));
                        b.CloseComponent();
                    }));
                    rb.CloseComponent();

                    // Item 2
                    rb.OpenComponent<AccordionItem>(s++);
                    rb.AddAttribute(s++, nameof(AccordionItem.Value), "item-2");
                    rb.AddAttribute(s++, nameof(AccordionItem.ChildContent), (RenderFragment)((b) =>
                    {
                        int i = 0;
                        b.OpenComponent<AccordionHeader>(i++);
                        b.AddAttribute(i++, nameof(AccordionHeader.ChildContent), (RenderFragment)((h) =>
                        {
                            int j = 0;
                            h.OpenComponent<AccordionTrigger>(j++);
                            h.AddAttribute(j++, nameof(AccordionTrigger.ChildContent), (RenderFragment)((t) =>
                            {
                                t.AddMarkupContent(0, "<span>Item 2</span>");
                            }));
                            h.CloseComponent();
                        }));
                        b.CloseComponent();

                        b.OpenComponent<AccordionContent>(i++);
                        b.AddAttribute(i++, nameof(AccordionContent.ChildContent), (RenderFragment)((c) =>
                        {
                            c.AddMarkupContent(0, "<div>Content 2</div>");
                        }));
                        b.CloseComponent();
                    }));
                    rb.CloseComponent();

                    // Item 3
                    rb.OpenComponent<AccordionItem>(s++);
                    rb.AddAttribute(s++, nameof(AccordionItem.Value), "item-3");
                    rb.AddAttribute(s++, nameof(AccordionItem.ChildContent), (RenderFragment)((b) =>
                    {
                        int i = 0;
                        b.OpenComponent<AccordionHeader>(i++);
                        b.AddAttribute(i++, nameof(AccordionHeader.ChildContent), (RenderFragment)((h) =>
                        {
                            int j = 0;
                            h.OpenComponent<AccordionTrigger>(j++);
                            h.AddAttribute(j++, nameof(AccordionTrigger.ChildContent), (RenderFragment)((t) =>
                            {
                                t.AddMarkupContent(0, "<span>Item 3</span>");
                            }));
                            h.CloseComponent();
                        }));
                        b.CloseComponent();

                        b.OpenComponent<AccordionContent>(i++);
                        b.AddAttribute(i++, nameof(AccordionContent.ChildContent), (RenderFragment)((c) =>
                        {
                            c.AddMarkupContent(0, "<div>Content 3</div>");
                        }));
                        b.CloseComponent();
                    }));
                    rb.CloseComponent();
                }));
                builder.CloseComponent();
            }
        }

        // Helper wrapper: Collapsible single with one item
        private sealed class CollapsibleWrapper : ComponentBase
        {
            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                int seq = 0;
                builder.OpenComponent<AccordionRoot>(seq++);
                builder.AddAttribute(seq++, nameof(AccordionRoot.Type), AccordionType.Single);
                builder.AddAttribute(seq++, nameof(AccordionRoot.Collapsible), true);
                builder.AddAttribute(seq++, nameof(AccordionRoot.ChildContent), (RenderFragment)((rb) =>
                {
                    int s = 0;
                    rb.OpenComponent<AccordionItem>(s++);
                    rb.AddAttribute(s++, nameof(AccordionItem.Value), "c-1");
                    rb.AddAttribute(s++, nameof(AccordionItem.ChildContent), (RenderFragment)((b) =>
                    {
                        int i = 0;
                        b.OpenComponent<AccordionHeader>(i++);
                        b.AddAttribute(i++, nameof(AccordionHeader.ChildContent), (RenderFragment)((h) =>
                        {
                            int j = 0;
                            h.OpenComponent<AccordionTrigger>(j++);
                            h.AddAttribute(j++, nameof(AccordionTrigger.ChildContent), (RenderFragment)((t) =>
                            {
                                t.AddMarkupContent(0, "<span>Collapsible</span>");
                            }));
                            h.CloseComponent();
                        }));
                        b.CloseComponent();

                        b.OpenComponent<AccordionContent>(i++);
                        b.AddAttribute(i++, nameof(AccordionContent.ChildContent), (RenderFragment)((c) =>
                        {
                            c.AddMarkupContent(0, "<div>Collapsible content</div>");
                        }));
                        b.CloseComponent();
                    }));
                    rb.CloseComponent();
                }));
                builder.CloseComponent();
            }
        }

        // Helper wrapper: Multiple with default open values
        private sealed class MultipleWrapper : ComponentBase
        {
            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                int seq = 0;
                builder.OpenComponent<AccordionRoot>(seq++);
                builder.AddAttribute(seq++, nameof(AccordionRoot.Type), AccordionType.Multiple);
                builder.AddAttribute(seq++, nameof(AccordionRoot.DefaultValue), new string[] { "m1", "m2" });
                builder.AddAttribute(seq++, nameof(AccordionRoot.ChildContent), (RenderFragment)((rb) =>
                {
                    int s = 0;

                    rb.OpenComponent<AccordionItem>(s++);
                    rb.AddAttribute(s++, nameof(AccordionItem.Value), "m1");
                    rb.AddAttribute(s++, nameof(AccordionItem.ChildContent), (RenderFragment)((b) =>
                    {
                        int i = 0;
                        b.OpenComponent<AccordionHeader>(i++);
                        b.AddAttribute(i++, nameof(AccordionHeader.ChildContent), (RenderFragment)((h) =>
                        {
                            int j = 0;
                            h.OpenComponent<AccordionTrigger>(j++);
                            h.AddAttribute(j++, nameof(AccordionTrigger.ChildContent), (RenderFragment)((t) =>
                            {
                                t.AddMarkupContent(0, "<span>Multi 1</span>");
                            }));
                            h.CloseComponent();
                        }));
                        b.CloseComponent();

                        b.OpenComponent<AccordionContent>(i++);
                        b.AddAttribute(i++, nameof(AccordionContent.ChildContent), (RenderFragment)((c) =>
                        {
                            c.AddMarkupContent(0, "<div>Multi content 1</div>");
                        }));
                        b.CloseComponent();
                    }));
                    rb.CloseComponent();

                    rb.OpenComponent<AccordionItem>(s++);
                    rb.AddAttribute(s++, nameof(AccordionItem.Value), "m2");
                    rb.AddAttribute(s++, nameof(AccordionItem.ChildContent), (RenderFragment)((b) =>
                    {
                        int i = 0;
                        b.OpenComponent<AccordionHeader>(i++);
                        b.AddAttribute(i++, nameof(AccordionHeader.ChildContent), (RenderFragment)((h) =>
                        {
                            int j = 0;
                            h.OpenComponent<AccordionTrigger>(j++);
                            h.AddAttribute(j++, nameof(AccordionTrigger.ChildContent), (RenderFragment)((t) =>
                            {
                                t.AddMarkupContent(0, "<span>Multi 2</span>");
                            }));
                            h.CloseComponent();
                        }));
                        b.CloseComponent();

                        b.OpenComponent<AccordionContent>(i++);
                        b.AddAttribute(i++, nameof(AccordionContent.ChildContent), (RenderFragment)((c) =>
                        {
                            c.AddMarkupContent(0, "<div>Multi content 2</div>");
                        }));
                        b.CloseComponent();
                    }));
                    rb.CloseComponent();

                    rb.OpenComponent<AccordionItem>(s++);
                    rb.AddAttribute(s++, nameof(AccordionItem.Value), "m3");
                    rb.AddAttribute(s++, nameof(AccordionItem.ChildContent), (RenderFragment)((b) =>
                    {
                        int i = 0;
                        b.OpenComponent<AccordionHeader>(i++);
                        b.AddAttribute(i++, nameof(AccordionHeader.ChildContent), (RenderFragment)((h) =>
                        {
                            int j = 0;
                            h.OpenComponent<AccordionTrigger>(j++);
                            h.AddAttribute(j++, nameof(AccordionTrigger.ChildContent), (RenderFragment)((t) =>
                            {
                                t.AddMarkupContent(0, "<span>Multi 3</span>");
                            }));
                            h.CloseComponent();
                        }));
                        b.CloseComponent();

                        b.OpenComponent<AccordionContent>(i++);
                        b.AddAttribute(i++, nameof(AccordionContent.ChildContent), (RenderFragment)((c) =>
                        {
                            c.AddMarkupContent(0, "<div>Multi content 3</div>");
                        }));
                        b.CloseComponent();
                    }));
                    rb.CloseComponent();
                }));
                builder.CloseComponent();
            }
        }

        // Helper wrapper: ForceMount on first item
        private sealed class ForceMountWrapper : ComponentBase
        {
            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                int seq = 0;
                builder.OpenComponent<AccordionRoot>(seq++);
                builder.AddAttribute(seq++, nameof(AccordionRoot.Type), AccordionType.Single);
                builder.AddAttribute(seq++, nameof(AccordionRoot.ChildContent), (RenderFragment)((rb) =>
                {
                    int s = 0;
                    rb.OpenComponent<AccordionItem>(s++);
                    rb.AddAttribute(s++, nameof(AccordionItem.Value), "f1");
                    rb.AddAttribute(s++, nameof(AccordionItem.ChildContent), (RenderFragment)((b) =>
                    {
                        int i = 0;
                        b.OpenComponent<AccordionHeader>(i++);
                        b.AddAttribute(i++, nameof(AccordionHeader.ChildContent), (RenderFragment)((h) =>
                        {
                            int j = 0;
                            h.OpenComponent<AccordionTrigger>(j++);
                            h.AddAttribute(j++, nameof(AccordionTrigger.ChildContent), (RenderFragment)((t) =>
                            {
                                t.AddMarkupContent(0, "<span>Force</span>");
                            }));
                            h.CloseComponent();
                        }));
                        b.CloseComponent();

                        b.OpenComponent<AccordionContent>(i++);
                        b.AddAttribute(i++, nameof(AccordionContent.ForceMount), true);
                        b.AddAttribute(i++, nameof(AccordionContent.ChildContent), (RenderFragment)((c) =>
                        {
                            c.AddMarkupContent(0, "<div>Force content</div>");
                        }));
                        b.CloseComponent();
                    }));
                    rb.CloseComponent();
                }));
                builder.CloseComponent();
            }
        }

        [Fact(DisplayName = "Single: clicking a trigger opens its content and sets aria-expanded")]
        public void Single_Click_OpenBehavior()
        {
            // Arrange
            IRenderedComponent<SingleAccordionWrapper> cut = ctx.Render<SingleAccordionWrapper>();

            // find all triggers (buttons)
            var buttons = cut.FindAll("button");
            Assert.True(buttons.Count >= 3);

            // Act - click first trigger
            buttons[0].Click();

            // Assert - first content should be open
            var panels = cut.FindAll("div[role='region']");
            Assert.True(panels.Count >= 3);

            var firstPanel = panels[0];
            Assert.Equal("open", firstPanel.GetAttribute("data-state"));
            Assert.Equal("true", buttons[0].GetAttribute("aria-expanded"));
        }

        [Fact(DisplayName = "Collapsible: clicking open then clicking again closes")]
        public void Collapsible_Click_TogglesClosed()
        {
            // Arrange
            IRenderedComponent<CollapsibleWrapper> cut = ctx.Render<CollapsibleWrapper>();

            var button = cut.Find("button");

            // Act - open
            button.Click();
            var panel = cut.Find("div[role='region']");
            Assert.Equal("open", panel.GetAttribute("data-state"));
            Assert.Equal("true", button.GetAttribute("aria-expanded"));

            // Act - click again to collapse
            button.Click();

            // Assert - panel should be closed
            // When closed and not ForceMount the content might be not rendered; but our implementation renders only when open or ForceMount
            // So we check that either the panel exists with closed state or it's not present
            var panels = cut.FindAll("div[role='region']");
            if (panels.Count == 0)
            {
                // content removed from DOM — acceptable for non-ForceMount closed panel
                Assert.True(true);
            }
            else
            {
                Assert.Equal("closed", panels[0].GetAttribute("data-state"));
                Assert.Equal("false", button.GetAttribute("aria-expanded"));
            }
        }

        [Fact(DisplayName = "Multiple: DefaultValue opens multiple panels")]
        public void Multiple_DefaultValue_OpensMultiple()
        {
            // Arrange
            IRenderedComponent<MultipleWrapper> cut = ctx.Render<MultipleWrapper>();

            // Act - find panels
            var panels = cut.FindAll("div[role='region']");
            Assert.Equal(3, panels.Count);

            // First two should be open by DefaultValue (m1, m2)
            Assert.Equal("open", panels[0].GetAttribute("data-state"));
            Assert.Equal("open", panels[1].GetAttribute("data-state"));
            Assert.Equal("closed", panels[2].GetAttribute("data-state"));
        }

        [Fact(DisplayName = "Trigger aria-controls links to content id and content aria-labelledby references trigger")]
        public void Aria_Linkage_IsCorrect()
        {
            IRenderedComponent<SingleAccordionWrapper> cut = ctx.Render<SingleAccordionWrapper>();

            var buttons = cut.FindAll("button");
            var panels = cut.FindAll("div[role='region']");
            Assert.True(buttons.Count >= 1);
            Assert.True(panels.Count >= 1);

            // Before clicking, content may be not rendered; click to ensure content exists for first item
            buttons[0].Click();

            panels = cut.FindAll("div[role='region']");
            var button = buttons[0];
            var panel = panels[0];

            var controls = button.GetAttribute("aria-controls");
            Assert.False(string.IsNullOrEmpty(controls));

            // panel id must match aria-controls
            Assert.Equal(controls, panel.Id);

            // panel's aria-labelledby should reference trigger (or header). We expect trigger id present in attribute value.
            var labelledBy = panel.GetAttribute("aria-labelledby");
            Assert.False(string.IsNullOrEmpty(labelledBy));
            Assert.NotNull(button.Id);
            Assert.Contains(button.Id, labelledBy);
        }

        [Fact(DisplayName = "ForceMount: content is present in DOM when closed and has hidden attribute")]
        public void ForceMount_ContentPresentWhenClosed_HiddenTrue()
        {
            // Arrange
            IRenderedComponent<ForceMountWrapper> cut = ctx.Render<ForceMountWrapper>();

            // At initial render, no panel may be open; but ForceMount should still render the panel.
            var panels = cut.FindAll("div[role='region']");
            Assert.Single(panels);

            var panel = panels[0];
            // Since not open, data-state should be "closed" and hidden attr should be present
            Assert.Equal("closed", panel.GetAttribute("data-state"));
            // hidden attribute should be present (value might be empty or "true" according to Razor rendering)
            Assert.True(panel.HasAttribute("hidden"));
        }

        // Minimal fake IJSObjectReference to satisfy component calls in tests
        private sealed class FakeJSObjectReference : IJSObjectReference, IAsyncDisposable
        {
            public ValueTask DisposeAsync() => ValueTask.CompletedTask;

            public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            {
                // Return default for any requested value types (safe for tests).
                return ValueTask.FromResult(default(TValue)!);
            }

            public async ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
            {
                // Simulate an asynchronous operation
                await Task.Delay(100, cancellationToken);

                // Here you would typically invoke the method identified by the identifier
                // For demonstration, we will return default(TValue)
                return default!;
            }

            public ValueTask InvokeVoidAsync(string identifier, object?[]? args)
            {
                // No-op
                return ValueTask.CompletedTask;
            }
        }
    }
}
