using Microsoft.JSInterop;

namespace Bladix.Primitives.Core.Primitive
{
    public sealed class TimerInterop : IAsyncDisposable
    {
        private readonly IJSRuntime _js;
        private readonly Dictionary<int, DotNetObjectReference<CallbackWrapper>> _callbacks = new();

        public TimerInterop(IJSRuntime js)
        {
            _js = js;
        }

        public async ValueTask<int> SetTimeout(Func<Task> callback, int ms)
        {
            var wrapper = new CallbackWrapper(callback);
            var reference = DotNetObjectReference.Create(wrapper);
            var id = await _js.InvokeAsync<int>("bladix.timer.setTimeout", reference, ms);
            _callbacks[id] = reference;
            return id;
        }

        public async ValueTask ClearTimeout(int id)
        {
            await _js.InvokeVoidAsync("bladix.timer.clearTimeout", id);
            if (_callbacks.Remove(id, out var reference))
                reference.Dispose();
        }

        public async ValueTask<int> SetInterval(Func<Task> callback, int ms)
        {
            var wrapper = new CallbackWrapper(callback);
            var reference = DotNetObjectReference.Create(wrapper);
            var id = await _js.InvokeAsync<int>("bladix.timer.setInterval", reference, ms);
            _callbacks[id] = reference;
            return id;
        }

        public async ValueTask ClearInterval(int id)
        {
            await _js.InvokeVoidAsync("bladix.timer.clearInterval", id);
            if (_callbacks.Remove(id, out var reference))
                reference.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            foreach (var reference in _callbacks.Values)
                reference.Dispose();
            _callbacks.Clear();
        }
    }
}
