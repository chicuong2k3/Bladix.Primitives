# Animation

Bladix Primitives can be animated using CSS keyframes or any JavaScript/Blazor interop 
animation library of your choice.

Adding animation to Bladix Primitives works like animating any other component, but 
there are some considerations for exit/unmount animations.

## Animating with CSS

The simplest way is via CSS animations. You can animate both mount and unmount 
phases because Bladix Primitives can suspend removal until the animation finishes.

```css
@keyframes fadeIn {
    from { opacity: 0; }
    to { opacity: 1; }
}

@keyframes fadeOut {
    from { opacity: 1; }
    to { opacity: 0; }
}

/* Open state animation */
.DialogOverlay[data-state="open"],
.DialogContent[data-state="open"] {
    animation: fadeIn 300ms ease-out;
}

/* Closed state animation */
.DialogOverlay[data-state="closed"],
.DialogContent[data-state="closed"] {
    animation: fadeOut 300ms ease-in;
}
```

```razor
<DialogRoot @bind-Open="@_isOpen">
    <DialogTrigger>Open Dialog</DialogTrigger>

    <DialogOverlay class="DialogOverlay" />
    <DialogContent class="DialogContent">
        <h1>Hello from inside the Dialog!</h1>
        <DialogClose>Close</DialogClose>
    </DialogContent>
</DialogRoot>

@code {
    private bool _isOpen;
}
```

## Handling unmounts with C#/JS animations

For more complex animations, especially during unmounting, you may need to leverage 
C# or JavaScript interop to control the animation lifecycle.

- Always render the element but toggle visibility with CSS or hidden.
- Or conditionally render and use `OnAfterRenderAsync` or JS interop to animate 
before removal.

```razor
@inject IJSRuntime JS

@if (isOpen)
{
    <DialogOverlay class="DialogOverlay" @ref="@_overlayRef" />
    <DialogContent class="DialogContent" @ref="@_contentRef">
        <h1>Hello from inside the Dialog!</h1>
        <DialogClose @onclick="CloseDialog">Close</DialogClose>
    </DialogContent>
}

@code {
    private bool _isOpen;
    private ElementReference _overlayRef;
    private ElementReference _contentRef;

    private async Task CloseDialog()
    {
        // Optionally play a JS animation via interop before setting _isOpen = false
        await JS.InvokeVoidAsync("playExitAnimation", _overlayRef, _contentRef);
        _isOpen = false;
    }
}
```

## Summary

- Bladix Primitives are unstyled; animations are handled externally (CSS or JS/Blazor interop).
- CSS animations can animate both opening and closing states using `data-state`.
- JS/Blazor interop animations can control unmounting by delaying the removal 
of the element.