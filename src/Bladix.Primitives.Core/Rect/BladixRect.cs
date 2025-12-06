using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Bladix.Primitives.Core.Rect
{
    public class BladixRect : IAsyncDisposable
    {
        private readonly IJSRuntime _js;
        private IJSObjectReference? _module;
        private IJSObjectReference? _cleanup;
        private readonly DotNetObjectReference<BladixRect> _self;
        private bool _disposed;

        public event Action<DOMRect>? OnChanged;

        public BladixRect(IJSRuntime js)
        {
            _js = js;
            _self = DotNetObjectReference.Create(this);
        }

        public async Task ObserveAsync(ElementReference element)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(BladixRect));

            _module ??= await _js.InvokeAsync<IJSObjectReference>("import", "./_content/Bladix/rect.js");

            _cleanup = await _module.InvokeAsync<IJSObjectReference>(
                "observeElementRect",
                element,
                _self
            );
        }

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
                await _cleanup.DisposeAsync();

            _self.Dispose();

            if (_module is not null)
                await _module.DisposeAsync();

            GC.SuppressFinalize(this); 
        }
    }
}