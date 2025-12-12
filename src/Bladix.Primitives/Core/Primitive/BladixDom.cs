using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Bladix.Primitives.Core.Primitive
{
    /// <summary>
    /// Provides DOM utilities for Blazor components, following Radix UI Primitives patterns.
    /// This version is more resilient to SSR/prerender: TryGetModuleAsync returns null when JS isn't available.
    /// Callers should handle null module (no-op or fallback).
    /// </summary>
    public sealed class BladixDom : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> _moduleTask;
        private bool _disposed;

        public BladixDom(IJSRuntime js)
        {
            ArgumentNullException.ThrowIfNull(js);

            // Lazy import; not executed until a method needs the module.
            _moduleTask = new Lazy<Task<IJSObjectReference>>(() =>
                js.InvokeAsync<IJSObjectReference>("import", "./_content/Bladix.Primitives/bladix.js").AsTask());
        }

        /// <summary>
        /// Try to get the JS module, returning null when JS runtime is not available (e.g., prerender).
        /// </summary>
        private async Task<IJSObjectReference?> TryGetModuleAsync()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            try
            {
                var module = await _moduleTask.Value;
                return module;
            }
            catch (JSException)
            {
                // JS runtime not available or module import failed during prerender — treat as no-op
                return null;
            }
            catch (InvalidOperationException)
            {
                // JSRuntime not available (server prerender) — treat as no-op
                return null;
            }
        }

        /// <summary>
        /// Checks if DOM is available in the current environment
        /// Returns false if JS isn't available instead of throwing.
        /// </summary>
        public async ValueTask<bool> CanUseDOM()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            var module = await TryGetModuleAsync();
            if (module == null) return false;

            return await module.InvokeAsync<bool>("getCanUseDOM");
        }

        /// <summary>
        /// Composes multiple event handlers together
        /// Caller should handle InvalidOperationException when JS isn't available.
        /// </summary>
        public async ValueTask<IJSObjectReference> ComposeEventHandlers(
            IJSObjectReference? originalEventHandler,
            IJSObjectReference? ourEventHandler,
            bool checkForDefaultPrevented = true)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            var module = await TryGetModuleAsync();
            if (module == null)
                throw new InvalidOperationException("JS runtime not available; composeEventHandlers cannot run.");

            return await module.InvokeAsync<IJSObjectReference>(
                "composeEventHandlers",
                originalEventHandler,
                ourEventHandler,
                new { checkForDefaultPrevented });
        }

        /// <summary>
        /// Observe an element's bounding rect. The dotNetRef must implement a method OnRectChanged(RectDto).
        /// Returns an IJSObjectReference-like object which should be disposed via DisposeAsync on the returned JS object.
        /// Caller should handle InvalidOperationException when JS isn't available.
        /// </summary>
        public async ValueTask<IJSObjectReference> ObserveElementRect(ElementReference element, DotNetObjectReference<object> dotNetRef)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            var module = await TryGetModuleAsync();
            if (module == null)
                throw new InvalidOperationException("JS runtime not available; observeElementRect cannot run.");

            return await module.InvokeAsync<IJSObjectReference>("observeElementRect", element, dotNetRef);
        }

        /// <summary>
        /// Gets the owner window of an element
        /// </summary>
        public async ValueTask<IJSObjectReference?> GetOwnerWindow(ElementReference? element = null)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            var module = await TryGetModuleAsync();
            if (module == null) return null;

            return await module.InvokeAsync<IJSObjectReference?>("getOwnerWindow", element);
        }

        /// <summary>
        /// Gets the owner document of an element
        /// </summary>
        public async ValueTask<IJSObjectReference?> GetOwnerDocument(ElementReference? element = null)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            var module = await TryGetModuleAsync();
            if (module == null) return null;

            return await module.InvokeAsync<IJSObjectReference?>("getOwnerDocument", element);
        }

        /// <summary>
        /// Gets the currently active element, with support for iframes and aria-activedescendant
        /// </summary>
        public async ValueTask<ElementReference?> GetActiveElement(ElementReference? node = null, bool activeDescendant = false)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            var module = await TryGetModuleAsync();
            if (module == null) return null;

            return await module.InvokeAsync<ElementReference?>("getActiveElement", node, activeDescendant);
        }

        /// <summary>
        /// Checks if an element is an iframe
        /// </summary>
        public async ValueTask<bool> IsFrame(ElementReference element)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            var module = await TryGetModuleAsync();
            if (module == null) return false;

            return await module.InvokeAsync<bool>("isFrame", element);
        }

        /// <summary>
        /// Checks if a parent element contains a child element
        /// </summary>
        public async ValueTask<bool> Contains(ElementReference parent, ElementReference child)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            var module = await TryGetModuleAsync();
            if (module == null) return false;

            return await module.InvokeAsync<bool>("contains", parent, child);
        }

        /// <summary>
        /// Gets all tabbable elements within a container
        /// </summary>
        public async ValueTask<ElementReference[]> GetTabbableElements(ElementReference container)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            var module = await TryGetModuleAsync();
            if (module == null) return Array.Empty<ElementReference>();

            return await module.InvokeAsync<ElementReference[]>("getTabbableElements", container);
        }

        /// <summary>
        /// Focus an element with optional prevention of scroll
        /// If JS isn't available, this becomes a no-op (caller should handle if necessary).
        /// </summary>
        public async ValueTask Focus(ElementReference element, bool preventScroll = false)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            var module = await TryGetModuleAsync();
            if (module == null) return;

            await module.InvokeVoidAsync("focus", element, new { preventScroll });
        }

        /// <summary>
        /// Gets the computed style of an element
        /// </summary>
        public async ValueTask<IJSObjectReference?> GetComputedStyle(ElementReference element)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            var module = await TryGetModuleAsync();
            if (module == null) return null;

            return await module.InvokeAsync<IJSObjectReference?>("getComputedStyle", element);
        }

        /// <summary>
        /// Set native checkbox indeterminate value on an input element
        /// </summary>
        public async ValueTask SetIndeterminate(ElementReference element, bool value)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            var module = await TryGetModuleAsync();
            if (module == null) return;

            await module.InvokeVoidAsync("setIndeterminate", element, value);
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed)
                return;

            _disposed = true;

            if (_moduleTask.IsValueCreated)
            {
                try
                {
                    var module = await _moduleTask.Value;
                    await module.DisposeAsync();
                }
                catch
                {
                    // ignore failures during dispose when JS isn't available
                }
            }

            GC.SuppressFinalize(this);
        }
    }
}
