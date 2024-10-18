using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal_Scraper.Models
{
    public class Selector
    {
        public string Name { get; set; }
        public string XPath { get; set; }
        public SelectionType SelectionType { get; set; }
        public ElementType ElementType { get; set; }
        public string Result { get; set; }
        public List<Selector> Selectors { get; set; }
        public bool IsObject { get; set; }

        public Selector(string name, string xpath, SelectionType selectionType, ElementType elementType) {
            Name = name;
            XPath = xpath;
            SelectionType = selectionType;
            ElementType = elementType;
            Result = string.Empty;
            Selectors = new List<Selector>();
            IsObject = CheckIfObject();
        }

        public bool CheckIfObject()
        {
            if (this.ElementType == ElementType.Object) return true;

            return false;
        }
    }
}
