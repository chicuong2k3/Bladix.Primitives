# Copilot Instructions for Blazor Radix UI Port

- Follow Radix UI component patterns: Button, Dialog, Popover, etc.
- Use PascalCase for component names and filenames:
  - Button.razor, DialogContent.razor, PopoverTrigger.razor
- Do not create separate .razor.cs files; use @code blocks inside .razor
- Private fields: prefix with _
- EventCallback properties: end with "Changed"
- Max line length: 120 chars
- Reference original Radix UI React code when designing component API:
  - https://github.com/radix-ui/primitives
- Maintain CSS/SCSS conventions compatible with Tailwind/Radix styling
