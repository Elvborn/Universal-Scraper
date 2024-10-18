using System;
using System.Diagnostics;
using Microsoft.JSInterop;
using System.Web;
using Universal_Scraper.Services;
using Microsoft.VisualBasic.FileIO;
using Universal_Scraper.ViewModels;
using Universal_Scraper.Models;
using CommunityToolkit.Maui.Core.Extensions;

namespace Universal_Scraper
{
    public partial class MainPage : ContentPage
    {
        public static WebView webContent;

        public MainPage(MainViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;

            webContent = WebContent;

            TargetTypePicker.SelectedIndex = 0;
            SelectionTypePicker.SelectedIndex = 0;
            ElementTypePicker.SelectedIndex = 0;
        }
    }
}
