using System;

namespace HotelMgt.Web.Models
{
    public class AutocompleteDropdownModel
    {
        public string Label { get; set; } = string.Empty;
        public string InputId { get; set; } = string.Empty;
        public string InputName { get; set; } = string.Empty;
        public string HiddenName { get; set; } = string.Empty;
        public string SearchUrl { get; set; } = string.Empty;
        public string SelectedText { get; set; } = string.Empty;
        public int SelectedId { get; set; }
        public string Placeholder { get; set; } = "Type to search...";
    }

    public class DateTimePickerModel
    {
        public string Label { get; set; } = string.Empty;
        public string InputId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime? Value { get; set; }
        public string Placeholder { get; set; } = "dd.MM.yyyy.";
    }
}
