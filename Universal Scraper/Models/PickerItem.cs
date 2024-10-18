using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal_Scraper.Models
{
    public class PickerItem
    {
        public string SenderName { get; set; }
        public string DisplayName { get; set; }
    }

    public class TargetTypeItem : PickerItem
    {
        public TargetType Type { get; set; }

        public TargetTypeItem(string senderName, TargetType type, string displayName)
        {
            this.SenderName = senderName;
            this.Type = type;
            this.DisplayName = displayName;
        }
    }

    public class SelectionItem : PickerItem
    {
        public SelectionType Type { get; set; }

        public SelectionItem(string senderName, SelectionType type, string displayName)
        {
            this.SenderName = senderName;
            this.Type = type;
            this.DisplayName = displayName;
        }
    }

    public class ElementItem : PickerItem
    {
        public ElementType Type { get; set; }

        public ElementItem(string senderName, ElementType type, string displayName)
        {
            this.SenderName = senderName;
            this.Type = type;
            this.DisplayName = displayName;
        }
    }

    public class ObjectItem : PickerItem
    {
        public ObjectItem(string senderName, string displayName)
        {
            this.SenderName = senderName;
            this.DisplayName = displayName;
        }
    }
}
