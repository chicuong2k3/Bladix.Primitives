using Bladix.Primitives.Core.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Bladix.Primitives.Components.Tabs;

/// <summary>
/// JavaScript interop for Tabs keyboard navigation
/// </summary>
public sealed class TabsKeyboard : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;
    private IJSObjectReference? _cleanupFunction;
    private bool _disposed;

    public TabsKeyboard(IJSRuntime js)
    {
        ArgumentNullException.ThrowIfNull(js);

        _moduleTask = new Lazy<Task<IJSObjectReference>>(() =>
            js.InvokeAsync<IJSObjectReference>(
                "import",
                "./_content/Bladix.Primitives/Components/Tabs/TabsTrigger.razor.js"
            ).AsTask());
    }

    public async ValueTask SetupKeyboardNavigation(
        ElementReference triggerElement,
        Orientation orientation = Orientation.Horizontal)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var module = await _moduleTask.Value;
        var orientationStr = orientation == Orientation.Vertical ? "vertical" : "horizontal";

        _cleanupFunction = await module.InvokeAsync<IJSObjectReference>(
            "setupKeyboardNavigation",
            triggerElement,
            orientationStr
        );
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;

        if (_cleanupFunction != null)
        {
            try
            {
                await _cleanupFunction.InvokeVoidAsync("apply");
            }
            catch (JSDisconnectedException) 
            { 
            }
            catch (JSException)
            {
            }
            finally
            {
                await _cleanupFunction.DisposeAsync();
            }
        }

        if (_moduleTask.IsValueCreated)
        {
            var module = await _moduleTask.Value;
            await module.DisposeAsync();
        }

        GC.SuppressFinalize(this);
    }
}
