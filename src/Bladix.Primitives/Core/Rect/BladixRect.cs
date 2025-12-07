using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Bladix.Primitives.Core.Rect
{
    /// <summary>
    /// Observes element bounding rectangle changes.
    /// </summary>
    public sealed class BladixRect : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> _moduleTask;
        private readonly DotNetObjectReference<BladixRect> _selfReference;
        private IJSObjectReference? _cleanup;
        private bool _disposed;

        public event Action<DOMRect>? OnChanged;

        public BladixRect(IJSRuntime js)
        {
            _moduleTask = new Lazy<Task<IJSObjectReference>>(() =>
                js.InvokeAsync<IJSObjectReference>("import", "./_content/Bladix.Primitives/bladix.js").AsTask());
            _selfReference = DotNetObjectReference.Create(this);
        }

        /// <summary>
        /// Starts observing the specified element's bounding rectangle
        /// </summary>
        public async Task ObserveAsync(ElementReference element)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            var module = await _moduleTask.Value;

            _cleanup = await module.InvokeAsync<IJSObjectReference>(
                "observeElementRect",
                element,
                _selfReference
            );
        }

        /// <summary>
        /// Called from JavaScript when the element's rectangle changes
        /// </summary>
        [JSInvokable]
        public void OnRectChanged(DOMRect rect)
        {
            OnChanged?.Invoke(rect);
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed)
                return;

            _disposed = true;

            if (_cleanup is not null)
            {
                try
                {
                    await _cleanup.DisposeAsync();
                }
                catch
                {
                    // Ignore disposal errors
                }
            }

            _selfReference.Dispose();

            if (_moduleTask.IsValueCreated)
            {
                var module = await _moduleTask.Value;
                await module.DisposeAsync();
            }

            GC.SuppressFinalize(this);
        }
    }
}