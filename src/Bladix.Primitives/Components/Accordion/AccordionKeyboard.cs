using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Bladix.Primitives.Components.Accordion;

/// <summary>
/// JavaScript interop for Accordion keyboard navigation
/// Loads scoped JS module for AccordionTrigger component
/// </summary>
public sealed class AccordionKeyboard : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;
    private IJSObjectReference? _cleanupFunction;
    private bool _disposed;

    public AccordionKeyboard(IJSRuntime js)
    {
        ArgumentNullException.ThrowIfNull(js);

        _moduleTask = new Lazy<Task<IJSObjectReference>>(() =>
            js.InvokeAsync<IJSObjectReference>(
                "import",
                "./_content/Bladix.Primitives/Components/Accordion/AccordionTrigger.razor.js"
            ).AsTask());
    }

    /// <summary>
    /// Sets up keyboard navigation for an accordion trigger
    /// </summary>
    public async ValueTask SetupKeyboardNavigation(
        ElementReference triggerElement,
        Orientation orientation = Orientation.Vertical)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var module = await _moduleTask.Value;
        var orientationStr = orientation == Orientation.Vertical ? "vertical" : "horizontal";

        // Call the JS function and store cleanup function
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

        // Call cleanup function if exists
        if (_cleanupFunction != null)
        {
            try
            {
                await _cleanupFunction.InvokeVoidAsync("apply");
            }
            catch (JSDisconnectedException) 
            { 
                // Circuit disconnected, ignore
            }
            catch (JSException)
            {
                // Silently handle
            }
            finally
            {
                await _cleanupFunction.DisposeAsync();
            }
        }

        // Dispose module
        if (_moduleTask.IsValueCreated)
        {
            var module = await _moduleTask.Value;
            await module.DisposeAsync();
        }

        GC.SuppressFinalize(this);
    }
}
