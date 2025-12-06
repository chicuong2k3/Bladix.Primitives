using Microsoft.AspNetCore.Components;

namespace Bladix.Primitives.Components.Accordion
{
    public class AccordionState
    {
        public AccordionType Type { get; set; } = AccordionType.Single;

        public string? SingleValue { get; set; }
        public List<string> MultipleValues { get; set; } = new List<string>();

        public EventCallback<string> OnSingleValueChange { get; set; }
        public EventCallback<List<string>> OnMultipleValuesChange { get; set; }

        public bool IsItemOpen(string value)
        {
            return Type == AccordionType.Single
                ? SingleValue == value
                : MultipleValues.Contains(value);
        }

        public async Task ToggleItemAsync(string value)
        {
            if (Type == AccordionType.Single)
            {
                if (SingleValue == value) SingleValue = null;
                else SingleValue = value;

                if (OnSingleValueChange.HasDelegate)
                    await OnSingleValueChange.InvokeAsync(SingleValue);
            }
            else
            {
                if (MultipleValues.Contains(value))
                    MultipleValues.Remove(value);
                else
                    MultipleValues.Add(value);

                if (OnMultipleValuesChange.HasDelegate)
                    await OnMultipleValuesChange.InvokeAsync(MultipleValues);
            }
        }
    }
}
