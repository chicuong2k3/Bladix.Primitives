using Microsoft.JSInterop;

namespace Bladix.Primitives.Core.Primitive
{
    public sealed class CallbackWrapper
    {
        private readonly Func<Task> _cb;

        public CallbackWrapper(Func<Task> cb)
        {
            _cb = cb;
        }

        [JSInvokable]
        public Task Invoke() => _cb();
    }

}
