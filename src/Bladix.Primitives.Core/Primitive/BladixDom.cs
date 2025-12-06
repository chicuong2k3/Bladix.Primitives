using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Bladix.Primitives.Core.Primitive
{
    public sealed class BladixDom
    {
        private readonly IJSRuntime _js;

        public BladixDom(IJSRuntime js) => _js = js;

        public ValueTask<bool> CanUseDOM()
            => _js.InvokeAsync<bool>("dom.canUseDOM");

        public ValueTask<ElementReference?> GetActiveElement(bool activeDescendant = false)
            => _js.InvokeAsync<ElementReference?>("dom.getActiveElement", null, activeDescendant);
    }
}
