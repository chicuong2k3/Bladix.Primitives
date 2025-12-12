using Microsoft.JSInterop;

namespace Bladix.Primitives.Core.Primitive;

public class CallbackWrapper
{
    private readonly Func<Task> _callback;

    public CallbackWrapper(Func<Task> callback)
    {
        _callback = callback;
    }

    [JSInvokable]
    public async Task Invoke()
    {
        await _callback();
    }
}
