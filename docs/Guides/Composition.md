

# Composition

Bladix Primitives allow you to compose their behavior onto alternative elements 
or your own Blazor components using the `AsChild` parameter.

All Bladix primitive parts that render a DOM element support `AsChild`. When 
`AsChild="true"`, the primitive does not render its default element. Instead, 
it attaches its behavior, attributes, and event handling to your child component 
or element.

## Changing the Element Type

Most primitives provide sensible defaults, so changing the element is rarely needed. 
However, there are cases where it is useful.

Example: `TooltipTrigger`
By default, `TooltipTrigger` renders as a <button>. You may want to attach it to 
a `<a>` link instead:

```razor
<TooltipRoot>
    <TooltipTrigger AsChild="true">
        <a href="https://bladix.io">Bladix Docs</a>
    </TooltipTrigger>
    <TooltipPortal>
        Tooltip content...
    </TooltipPortal>
</TooltipRoot>
```

> ⚠️ Make sure your element remains accessible and focusable. For example, 
a `<div>` cannot receive keyboard focus and would break accessibility.

### Composing with Your Own Components

You can wrap your own Blazor components in primitives via `AsChild`.

**Requirements:**

**1. Forward all attributes**

Your component must pass incoming attributes to the rendered element:

```razor
<!-- MyButton.razor -->
<button @attributes="Attributes">
    @ChildContent
</button>

@code {
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> Attributes { get; set; } = new();

    [Parameter] public RenderFragment? ChildContent { get; set; }
}
```

**2. Expose a reference if needed**

Some primitives need a reference to the element for focus management or measurements:

```razor
<!-- MyButton.razor -->
<button @attributes="Attributes" @ref="ElementRef">
    @ChildContent
</button>

@code {
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> Attributes { get; set; } = new();

    [Parameter] public RenderFragment? ChildContent { get; set; }

    public ElementReference ElementRef { get; private set; } = new();
}
```

## Composing Multiple Primitives

`AsChild` can be nested arbitrarily, allowing multiple primitives to share 
the same element.

**Example:** combining `TooltipTrigger` and `DialogTrigger` on a custom button:

```razor
<DialogRoot>
    <TooltipRoot>
        <TooltipTrigger AsChild="true">
            <DialogTrigger AsChild="true">
                <MyButton>Open Dialog</MyButton>
            </DialogTrigger>
        </TooltipTrigger>
        <TooltipPortal>Tooltip content</TooltipPortal>
    </TooltipRoot>

    <DialogPortal>
        Dialog content here...
    </DialogPortal>
</DialogRoot>
```

## Summary

- Use `AsChild` to replace the default element or wrap a custom component.
- Your component must forward **attributes** and optionally expose a **reference**.
- Nested `AsChild` allows multiple primitives to share behavior on a single element.
- Always ensure accessibility and focusability are preserved.