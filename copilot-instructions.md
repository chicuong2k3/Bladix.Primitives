# Copilot Instructions for Blazor Radix UI Port (Bladix Primitives)

## Project Overview
Bladix.Primitives is a faithful port of Radix UI Primitives to Blazor WebAssembly (.NET 10).
All components must match Radix UI's behavior, API, and accessibility patterns 100%.

## Reference Sources
- **Primary Reference**: https://github.com/radix-ui/primitives
- **React Source**: Always check the React implementation before porting
- **Documentation**: https://www.radix-ui.com/primitives/docs/overview/introduction
- **Accessibility**: Follow WAI-ARIA Design Patterns exactly as Radix UI does

## Implementation Audit (Mandatory)
Always perform a formal 100% compliance audit for any component implemented or modified. The audit must be documented in the PR and signed off by a reviewer.

Audit steps (minimum):
- Compare API surface: ensure props/parameters, names and signatures match Radix (props → parameters).
- Verify event naming and payloads (e.g., `onValueChange` → `OnValueChange`) and that EventCallback payload types match.
- Behavioral parity: controlled vs uncontrolled behavior, default values, open/close semantics, focus management, portal behavior, ForceMount, AsChild.
- Accessibility parity: roles, aria-* attributes, keyboard interactions, and focus trapping exactly as Radix does.
- Interaction tests: run bUnit tests for behavior and manual/automated keyboard tests where necessary.
- Visual / example parity: ensure example docs demonstrate identical usage patterns and observable behavior.
- Document references: include Radix source links or specific lines/sections used for verification.
- PR requirement: attach audit checklist and test results; reviewer must confirm "100% Radix compliance" before merging.

## Reference Sources
- **Primary Reference**: https://github.com/radix-ui/primitives
- **React Source**: Always check the React implementation before porting
- **Documentation**: https://www.radix-ui.com/primitives/docs/overview/introduction
- **Accessibility**: Follow WAI-ARIA Design Patterns exactly as Radix UI does

## Naming Conventions

### Project convention

Follow Radix UI naming for change callbacks.

- Use `OnValueChange` instead of `ValueChanged` for component value change EventCallbacks.
  - Example component parameter:
    - ` [Parameter] public EventCallback<T?> OnValueChange { get; set; } `
  - Example usage in Razor markup:
    - `<MyComponent Value="value" OnValueChange="HandleChange" />`
  - Example invocation inside component:
    - `await OnValueChange.InvokeAsync(newValue);`

Rationale: aligns naming with Radix UI conventions and improves consistency across components.

Note: rename any local variables or helpers that contained `ValueChanged` (e.g., `SingleValueChangedCallback`) to use `OnValueChange` in their identifier to keep code consistent.


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
    [Parameter] public EventCallback<bool> OnOpenChange { get; set; }
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
    [Parameter] public EventCallback<bool> OnOpenChange { get; set; }
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
// <DialogRoot Open="_isOpen" OnOpenChange="SetIsOpen">

### 2. Event Handlers
// Radix UI: onOpenChange, onValueChange, onSelect
// Bladix: OnOpenChange, OnValueChange, OnSelect

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
- Implementation audit completed and documented in PR (must be signed off by reviewer)
