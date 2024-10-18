using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal_Scraper.Models
{
    public class ProjectData
    {
        public string ProjectName { get; set; }
        public string TargetUrl { get; set; }
        public int IterationStart { get; set; }
        public int IterationEnd { get; set; }
        public TargetType TargetType { get; set; }
        public List<Selector> Selectors { get; set; }

        public ProjectData() {
            Selectors = new List<Selector>();
        }
    }

    public enum TargetType
    {
        Single_Page = 0,
        Multi_Page = 1,
        Pages_From_File = 2
    }

    public enum SelectionType
    {
        Single = 0,
        Multi = 1
    }
    public enum ElementType
    {
        Text = 0,
        Link = 1,
        Image = 2,
        Object = 3
    }
}
