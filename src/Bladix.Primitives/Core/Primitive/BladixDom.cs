using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Bladix.Primitives.Core.Primitive
{
    /// <summary>
    /// Provides DOM utilities for Blazor components, following Radix UI Primitives patterns.
    /// </summary>
    public sealed class BladixDom : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> _moduleTask;
        private bool _disposed;

        public BladixDom(IJSRuntime js)
        {
            ArgumentNullException.ThrowIfNull(js);
            
            _moduleTask = new Lazy<Task<IJSObjectReference>>(() =>
                js.InvokeAsync<IJSObjectReference>("import", "./_content/Bladix.Primitives/bladix.js").AsTask());
        }

        /// <summary>
        /// Checks if DOM is available in the current environment
        /// </summary>
        public async ValueTask<bool> CanUseDOM()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            
            var module = await _moduleTask.Value;
            return await module.InvokeAsync<bool>("getCanUseDOM");
        }

        /// <summary>
        /// Composes multiple event handlers together
        /// </summary>
        public async ValueTask<IJSObjectReference> ComposeEventHandlers(
            IJSObjectReference? originalEventHandler,
            IJSObjectReference? ourEventHandler,
            bool checkForDefaultPrevented = true)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            
            var module = await _moduleTask.Value;
            return await module.InvokeAsync<IJSObjectReference>(
                "composeEventHandlers",
                originalEventHandler,
                ourEventHandler,
                new { checkForDefaultPrevented });
        }

        /// <summary>
        /// Gets the owner window of an element
        /// </summary>
        public async ValueTask<IJSObjectReference?> GetOwnerWindow(ElementReference? element = null)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            
            var module = await _moduleTask.Value;
            return await module.InvokeAsync<IJSObjectReference?>("getOwnerWindow", element);
        }

        /// <summary>
        /// Gets the owner document of an element
        /// </summary>
        public async ValueTask<IJSObjectReference?> GetOwnerDocument(ElementReference? element = null)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            
            var module = await _moduleTask.Value;
            return await module.InvokeAsync<IJSObjectReference?>("getOwnerDocument", element);
        }

        /// <summary>
        /// Gets the currently active element, with support for iframes and aria-activedescendant
        /// </summary>
        public async ValueTask<ElementReference?> GetActiveElement(ElementReference? node = null, bool activeDescendant = false)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            
            var module = await _moduleTask.Value;
            return await module.InvokeAsync<ElementReference?>("getActiveElement", node, activeDescendant);
        }

        /// <summary>
        /// Checks if an element is an iframe
        /// </summary>
        public async ValueTask<bool> IsFrame(ElementReference element)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            
            var module = await _moduleTask.Value;
            return await module.InvokeAsync<bool>("isFrame", element);
        }

        /// <summary>
        /// Checks if a parent element contains a child element
        /// </summary>
        public async ValueTask<bool> Contains(ElementReference parent, ElementReference child)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            
            var module = await _moduleTask.Value;
            return await module.InvokeAsync<bool>("contains", parent, child);
        }

        /// <summary>
        /// Gets all tabbable elements within a container
        /// </summary>
        public async ValueTask<ElementReference[]> GetTabbableElements(ElementReference container)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            
            var module = await _moduleTask.Value;
            return await module.InvokeAsync<ElementReference[]>("getTabbableElements", container);
        }

        /// <summary>
        /// Focus an element with optional prevention of scroll
        /// </summary>
        public async ValueTask Focus(ElementReference element, bool preventScroll = false)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("focus", element, new { preventScroll });
        }

        /// <summary>
        /// Gets the computed style of an element
        /// </summary>
        public async ValueTask<IJSObjectReference?> GetComputedStyle(ElementReference element)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            
            var module = await _moduleTask.Value;
            return await module.InvokeAsync<IJSObjectReference?>("getComputedStyle", element);
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed)
                return;

            _disposed = true;

            if (_moduleTask.IsValueCreated)
            {
                var module = await _moduleTask.Value;
                await module.DisposeAsync();    
            }

            GC.SuppressFinalize(this);
        }
    }
}
