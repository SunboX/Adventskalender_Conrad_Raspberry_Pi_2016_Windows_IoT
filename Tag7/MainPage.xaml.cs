using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Vorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 dokumentiert.

namespace Tag7
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly TouchHelper _touchHelper;

        public MainPage()
        {
            this.InitializeComponent();
            _touchHelper = new TouchHelper();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await _touchHelper.Initialize(this);
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _touchHelper.Teardown(this);
            base.OnNavigatedFrom(e);
        }

        private void ButtonRed_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Red");
        }

        private void ButtonGreen_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Green");
        }

        private void ButtonBlue_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Blue");
        }
    }
}
