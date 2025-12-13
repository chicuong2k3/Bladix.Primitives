using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Bladix.Primitives.Components.Accordion;

/// <summary>
/// JS interop helper to measure accordion content and animate height smoothly.
/// </summary>
public sealed class AccordionContentJs : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;
    private bool _disposed;

    public AccordionContentJs(IJSRuntime js)
    {
        ArgumentNullException.ThrowIfNull(js);
        _moduleTask = new Lazy<Task<IJSObjectReference>>(() =>
            js.InvokeAsync<IJSObjectReference>("import", "./_content/Bladix.Primitives/Components/Accordion/AccordionContent.razor.js").AsTask());
    }

    public async ValueTask SetOpenAsync(ElementReference element)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("setOpen", element);
    }

    public async ValueTask SetClosedAsync(ElementReference element)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("setClosed", element);
    }

    /// <summary>
    /// Update measured height from C# when content size changes (ResizeObserver -> BladixRect).
    /// JS will only apply the height when the element is currently animating or when an inline
    /// pixel height is present (avoids forcing layout when height is 'auto').
    /// </summary>
    public async ValueTask UpdateMeasuredHeightAsync(ElementReference element, double height)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("updateMeasuredHeight", element, height);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;

        if (_moduleTask.IsValueCreated)
        {
            var module = await _moduleTask.Value;
            await module.DisposeAsync();
        }

        GC.SuppressFinalize(this);
    }
}
