using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Universal_Scraper.Models;
using Universal_Scraper.Services;

namespace Universal_Scraper.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<PickerItem> targetTypePicker = new();
        [ObservableProperty]
        PickerItem selectedTargetTypePicker;

        [ObservableProperty]
        ObservableCollection<PickerItem> selectionTypePicker = new();
        [ObservableProperty]
        PickerItem selectedSelectionTypePicker;

        [ObservableProperty]
        ObservableCollection<PickerItem> elementTypePicker = new();
        [ObservableProperty]
        PickerItem selectedElementTypePicker;

        [ObservableProperty]
        ObservableCollection<PickerItem> objectPicker = new();
        [ObservableProperty]
        PickerItem selectedObjectPicker;

        [ObservableProperty]
        bool isUsingMultiPage;

        [ObservableProperty]
        bool canAddToObject;

        [ObservableProperty]
        bool showWebContent;
        [ObservableProperty]
        bool showSelectorContent;
        [ObservableProperty]
        bool showDataContent;

        [ObservableProperty]
        string selectorName;

        [ObservableProperty]
        TargetType targetType;

        [ObservableProperty]
        string targetPage;

        [ObservableProperty]
        string activePage;

        [ObservableProperty]
        int iterationStart;

        [ObservableProperty]
        int iterationEnd;

        [ObservableProperty]
        string selectedXPath;

        [ObservableProperty]
        string highlightedElement;

        [ObservableProperty]
        string projectName;

        [ObservableProperty]
        ProjectData projectData;

        [ObservableProperty]
        ObservableCollection<Selector> selectorData = new();

        [ObservableProperty]
        ObservableCollection<Selector> collectedData = new();

        HttpClient httpClient;
        string prevPage;

        public MainViewModel()
        {
            projectData = new();
            httpClient = new HttpClient();

            // Show Webcontent on startup
            ShowWebContent = true;

            // Setup Picker Data
            TargetTypePicker.Add(new TargetTypeItem("TargetType", TargetType.Single_Page, "Single Page"));
            TargetTypePicker.Add(new TargetTypeItem("TargetType", TargetType.Multi_Page, "Multi Page"));
            TargetTypePicker.Add(new TargetTypeItem("TargetType", TargetType.Pages_From_File, "Pages From File"));

            SelectionTypePicker.Add(new SelectionItem("SelectionType", SelectionType.Single, "Single Element"));
            SelectionTypePicker.Add(new SelectionItem("SelectionType", SelectionType.Multi, "Multiple Elements"));

            ElementTypePicker.Add(new ElementItem("ElementType", ElementType.Text, "Text"));
            ElementTypePicker.Add(new ElementItem("ElementType", ElementType.Link, "Link"));
            ElementTypePicker.Add(new ElementItem("ElementType", ElementType.Image, "Image"));
            ElementTypePicker.Add(new ElementItem("ElementType", ElementType.Object, "OBJECT"));

            // Only for development
            TargetPage = "https://webscraper.io/test-sites";
            SubmitTarget();
        }

        [RelayCommand]
        async Task Scrape()
        {
            ProjectData.TargetUrl = TargetPage;
            ProjectData.IterationStart = IterationStart;
            ProjectData.IterationEnd = IterationEnd;
            ProjectData.Selectors.Clear();

            foreach(Selector s in SelectorData)
            {
                ProjectData.Selectors.Add(s);
            }

            CollectedData.Clear();
            foreach(Selector selector in await Scraper.ScrapeTarget(ProjectData))
            {
                CollectedData.Add(selector);
            }
        }

        [RelayCommand]
        void PickerHandler(PickerItem picker)
        {
            switch (picker)
            {
                case TargetTypeItem targetTypeItem:

                    // If multipage selection, then show iteration fields
                    if (targetTypeItem.Type == TargetType.Multi_Page) IsUsingMultiPage = true;
                    else IsUsingMultiPage = false;

                    break;
                case SelectionItem selectionItem:

                    break;
                case ElementItem elementItem:
                    
                    break;
                case ObjectItem objectItem:
                    
                    break;
                default:
                    Debug.WriteLine("Picker ERROR!");
                    break;
            }
        }

        [RelayCommand]
        private async Task SubmitTarget()
        {
            if (TargetPage == ActivePage) return;

            string url = await WebRequest.IsValid(TargetPage);

            // Display Warning if target is changed
            if (Shell.Current.CurrentPage.IsLoaded)
            {
                bool result = await Shell.Current.CurrentPage.DisplayAlert("Warning", "If you change target, you're selector data will be deleted...", "Continue", "Cancel");
                TargetPage = ActivePage;
                if (result == false) return;
            }

            if (url != null)
            {
                TargetPage = url;
                ActivePage = url;
                ProjectData.TargetUrl = url;
            }
        }

        [RelayCommand]
        void SaveSelector()
        {
            if (!ValidInput()) return;

            // Save data to projectData
            Selector selector = new Selector(
                name: SelectorName,
                xpath: SelectedXPath,
                selectionType: ((SelectionItem)SelectedSelectionTypePicker).Type,
                elementType: ((ElementItem)SelectedElementTypePicker).Type
            );

            foreach (Selector s in SelectorData)
            {
                if (s.Name == selector.Name)
                {
                    Shell.Current.CurrentPage.DisplayAlert("Unable to save selector!", "A selector already exists with that name...", "Okay");
                    return;
                }
            }

            SelectorData.Add(selector);
            ClearInputFields();

            // If selector is an object, then update object picker
            if (selector.ElementType == ElementType.Object) UpdateObjectSelectors();
        }
        [RelayCommand]
        void AddToObject(PickerItem p)
        {
            // Check for valid inputs
            if (!ValidInput()) return;

            if (((SelectionItem)SelectedSelectionTypePicker).Type == SelectionType.Multi)
            {
                Shell.Current.CurrentPage.DisplayAlert("Unable to save selector!", "Selection Type cannot be Multi when adding to an OBJECT.", "Okay");
                return;
            }

            if (((ElementItem)SelectedElementTypePicker).Type == ElementType.Object)
            {
                Shell.Current.CurrentPage.DisplayAlert("Unable to save selector!", "Element Type cannot be OBJECT when adding to an OBJECT.", "Okay");
                return;
            }

            // Check for name duplicats in object
            foreach(Selector s in SelectorData)
            {
                if(s.Name == p.DisplayName)
                {
                    foreach (Selector s2 in s.Selectors)
                    {
                        if(s2.Name == SelectorName)
                        {
                            Shell.Current.CurrentPage.DisplayAlert("Unable to save selector!", $"A selector within object: {p.DisplayName} already exists with that name...", "Okay");
                            return;
                        }
                    }
                }
            }

            Selector selector = new Selector(
                name: SelectorName,
                xpath: SelectedXPath,
                selectionType: ((SelectionItem)SelectedSelectionTypePicker).Type,
                elementType: ((ElementItem)SelectedElementTypePicker).Type
            );

            foreach(Selector s in SelectorData)
            {
                if(s.Name == p.DisplayName)
                {
                    s.Selectors.Add(selector);
                }
            }

            ClearInputFields();
        }

        void UpdateObjectSelectors()
        {
            ObjectPicker.Clear();

            foreach (Selector s in SelectorData)
            {
                if(s.ElementType == ElementType.Object)
                {
                    ObjectPicker.Add(new ObjectItem("Object", s.Name));
                }
            }

            if (ObjectPicker.Count > 0) CanAddToObject = true;
            else CanAddToObject = false;
        }

        void ClearInputFields() {
            SelectorName = string.Empty;
            SelectedXPath = string.Empty;
        }

        bool ValidInput()
        {
            // Check for valid data
            if (SelectorName == null || SelectorName == string.Empty ||
                SelectedXPath == null || SelectorName == string.Empty)
            {
                Shell.Current.CurrentPage.DisplayAlert("Warning", "Please fill out all fields.", "Okay");
                return false;
            }

            return true;
        }

        [RelayCommand]
        void ChangeDisplayedContent(string content)
        {
            ShowWebContent = false;
            ShowSelectorContent = false;
            ShowDataContent = false;

            switch (content)
            {
                case "web":
                    ShowWebContent = true;
                    break;
                case "selector":
                    ShowSelectorContent = true;
                    break;
                case "data":
                    ShowDataContent = true;
                    break;
                default: ShowWebContent = true;
                    break;
            }
        }

        [RelayCommand]
        void DeleteSelector(Selector selector)
        {
            if (selector == null) return;

            SelectorData.Remove(selector);
            UpdateObjectSelectors();
        }

        [RelayCommand]
        void WebNavigating(WebNavigatingEventArgs e)
        {
            // Tjek JSInjection data i url
            var urlParts = e.Url.Split(".");
            if (urlParts[0].ToLower().Equals("http://poc"))
            {
                var funcToCall = urlParts[1].Split("?");
                var methodName = funcToCall[0].Replace("/", "");
                var funcParams = funcToCall[1].Split("&");

                for (int i = 0; i < funcParams.Length; i++)
                {
                    funcParams[i] = funcParams[i].Substring(5);
                }

                if (methodName.ToLower() == nameof(JSHandler).ToLower()) JSHandler(funcParams[0], bool.Parse(funcParams[1]));
            }

            // Naviger kun ved brugen af sidepanelet
            if (TargetPage == e.Url)
            {
                e.Cancel = false;
                return;
            }

            e.Cancel = true;
        }

        [RelayCommand]
        private void WebNavigated(WebNavigatedEventArgs e)
        {
            if (prevPage == TargetPage) return;
            prevPage = TargetPage;

            // Inject JavaScript i WebView til at få XPath data
            JSInjection.InjectXPathReader(MainPage.webContent, true);
        }

        [JSInvokable]
        public void JSHandler(string elementHtml, bool transferToActiveSelector)
        {
            elementHtml = HttpUtility.UrlDecode(elementHtml);
            HighlightedElement = "Active element: " + elementHtml;

            if (transferToActiveSelector) SelectedXPath = elementHtml;
        }
    }
}