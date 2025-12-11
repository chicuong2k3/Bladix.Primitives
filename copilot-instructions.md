# Copilot Instructions for Blazor Radix UI Port (Bladix Primitives)

## Project Overview
Bladix.Primitives is a faithful port of Radix UI Primitives to Blazor WebAssembly (.NET 10).
All components must match Radix UI's behavior, API, and accessibility patterns 100%.

## Reference Sources
- **Primary Reference**: https://github.com/radix-ui/primitives
- **React Source**: Always check the React implementation before porting
- **Documentation**: https://www.radix-ui.com/primitives/docs/overview/introduction
- **Accessibility**: Follow WAI-ARIA Design Patterns exactly as Radix UI does

## Naming Conventions

### Component Names
- Use PascalCase for all component names and filenames
- Follow Radix UI naming exactly:
  - `AccordionRoot.razor`, `AccordionItem.razor`, `AccordionTrigger.razor`
  - `DialogRoot.razor`, `DialogTrigger.razor`, `DialogContent.razor`
  - `AlertDialogRoot.razor`, `AlertDialogAction.razor`, `AlertDialogCancel.razor`
  - `PopoverRoot.razor`, `PopoverTrigger.razor`, `PopoverContent.razor`

### File Structure

src/Bladix.Primitives/Components/
├── Accordion/
│   ├── AccordionRoot.razor
│   ├── AccordionItem.razor
│   ├── AccordionHeader.razor
│   ├── AccordionTrigger.razor
│   └── AccordionContent.razor
├── AlertDialog/
│   ├── AlertDialogRoot.razor
│   ├── AlertDialogTrigger.razor
│   ├── AlertDialogPortal.razor
│   ├── AlertDialogOverlay.razor
│   ├── AlertDialogContent.razor
│   ├── AlertDialogTitle.razor
│   ├── AlertDialogDescription.razor
│   ├── AlertDialogAction.razor
│   └── AlertDialogCancel.razor
└── [ComponentName]/


### Code Conventions
- **NO separate .razor.cs files** - use `@code` blocks inside `.razor` files
- **Private fields**: prefix with `_`
- **EventCallback properties**: suffix `Changed`
- **Max line length**: 120 characters
- **Indentation**: 4 spaces

## Component Structure Template

### Basic Component Pattern

@inherits BladixComponentBase
@if (AsChild) { @ChildContent } else {
    <[element]
        @attributes="AdditionalAttributes"
        [aria-attributes]
        data-state="@([state])"
        data-[custom-attributes]
        class="@Class"
        style="@Style"
        @ref="ElementRef">
        @ChildContent
    </[element]>
}

@code {
    [CascadingParameter] public [ParentComponent]? [Parent] { get; set; }

    [Parameter] public bool Open { get; set; }
    [Parameter] public EventCallback<bool> OpenChanged { get; set; }
    [Parameter] public bool DefaultOpen { get; set; }

    private bool _isOpen;
    private string _uniqueId = $"bladix-[component]-{Guid.NewGuid():N}";
}

### Root Component Pattern

@inherits BladixComponentBase
<CascadingValue Value="this" IsFixed="true">
    @ChildContent
</CascadingValue>

@code {
    private bool _open;

    [Parameter] public bool Open { get; set; }
    [Parameter] public EventCallback<bool> OpenChanged { get; set; }
    [Parameter] public bool DefaultOpen { get; set; }

    protected override void OnInitialized()
    {
        _open = Open || DefaultOpen;
    }

    public bool IsOpen => _open;

    public async Task SetOpen(bool value)
    {
        if (_open == value) return;

        _open = value;
        await OpenChanged.InvokeAsync(value);
        StateHasChanged();
    }
}

## Required Base Component Features

[Parameter] public RenderFragment? ChildContent { get; set; }
[Parameter] public string? Class { get; set; }
[Parameter] public string? Style { get; set; }
[Parameter] public bool AsChild { get; set; }
[Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }
public ElementReference ElementRef { get; protected set; }

## API Matching Rules

### 1. Props → Parameters
Match Radix UI props EXACTLY:

// Radix UI React
// <Dialog.Root open={isOpen} onOpenChange={setIsOpen}>
// Bladix Blazor
// <DialogRoot Open="_isOpen" OpenChanged="SetIsOpen">

### 2. Event Handlers
// Radix UI: onOpenChange, onValueChange, onSelect
// Bladix: OpenChanged, ValueChanged, OnSelect

### 3. Boolean Props
// Radix UI: disabled, forceMount, defaultOpen
// Bladix: Disabled, ForceMount, DefaultOpen

### 4. Composition Props
// Radix UI: asChild
// Bladix: AsChild="true"

## Data Attributes (Required)

data-state  
data-disabled  
data-orientation  
data-side  
data-align  

## Accessibility Requirements

### ARIA Attributes
- role="dialog", aria-modal="true"
- aria-expanded, aria-controls, aria-selected, etc.

### Keyboard Navigation
- Arrow keys, Home/End, Escape, Tab trapping

### Focus Management
- Via BladixDom

## State Management Patterns

Controlled/uncontrolled with sync in `OnParametersSet()`.

## Cascading Parameters

Use `CascadingValue` with child components receiving parent references.

## JavaScript Interop

Use BladixDom, BladixRect, TimerInterop, or custom JS modules.

## Styling Guidelines

No default styles. Use data attributes for styling.

## ForceMount Pattern

Used for animations.

## AsChild Pattern

Support for composition.

## Portal Pattern

Render outside document flow.

## IAsyncDisposable Pattern

Dispose JS resources properly.

## Testing Requirements

Use bUnit in `tests/Bladix.Primitives.Components.Tests`.

## Documentation Requirements

Provide API, examples, accessibility notes, styling guide.

## Common Pitfalls

Avoid default styling, missing ARIA, wrong naming, skipping cleanup.

## Version Compatibility

.NET 10, Blazor WebAssembly, modern browsers.

## Additional Resources

GitHub repos and ARIA guidelines.

## Quick Checklist

- API matching
- Controlled/uncontrolled
- Keyboard navigation
- ARIA complete
- ForceMount
- AsChild
- Tests
- Docs
