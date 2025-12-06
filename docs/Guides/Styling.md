
# Styling

Bladix Primitives are unstyled by design and work with any styling solution, 
giving you full control over presentation.

## Styling Overview

### Functional styles

You control all styling aspects. For example, a `DialogOverlay` does not automatically 
cover the viewport. You must provide that via CSS or inline styles.

### Classes

All Bladix components and their parts accept a `Class` parameter. This class is 
passed directly to the underlying HTML element.

### Data attributes

Stateful components expose their state through a `data-state` attribute.

**Example:** An `Accordion Item` that is opened will render with `data-state="open"`.

## Styling with CSS

### Styling a part

```razor
<AccordionRoot>
    <AccordionItem Class="AccordionItem" Value="item1">
        Item 1
    </AccordionItem>
</AccordionRoot>
```

```css
.AccordionItem {
    border-bottom: 1px solid gainsboro;
}

.AccordionItem[data-state="open"] {
    border-bottom-width: 2px;
}
```

### Styling a state

You can target `data-state` in CSS to style a component based on its state:

```css
.AccordionItem[data-state="open"] {
    background-color: #f0f0f0;
    border-bottom-width: 2px;
}
```

## Extending a Primitive

You can extend a Bladix Primitive by wrapping it in a new Blazor component:

```razor
@inherits AccordionItemBase

<div @attributes="Attributes" Class="@Class" @ref="ElementRef">
    @ChildContent
</div>

@code {
    [Parameter] public RenderFragment? ChildContent { get; set; }

    [Parameter(CaptureUnmatchedValues = true)] 
    public Dictionary<string, object?> Attributes { get; set; } = new();
}
```

## Summary

Bladix Primitives are designed to:
- Encapsulate accessibility and complex behaviors.
- Expose component state via `data-state` for styling.
- Be completely unstyled, letting you fully control appearance.
- Be extended via simple Blazor component wrappers.