using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Bladix.Primitives.Core.Primitive
{
    public sealed class BladixDom
    {
        private readonly IJSRuntime _js;

        public BladixDom(IJSRuntime js) => _js = js;

        public ValueTask<bool> CanUseDOM()
            => _js.InvokeAsync<bool>("bladix.dom.canUseDOM");

        public ValueTask<ElementReference?> GetActiveElement(bool activeDescendant = false)
            => _js.InvokeAsync<ElementReference?>("bladix.dom.getActiveElement", null, activeDescendant);
    }
}
