using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace TestPaint
{
    public sealed partial class HubPage : Page
    {
        private ObservableCollection<CanvasDataObject> CanvasData;

        public HubPage()
        {
            this.InitializeComponent();
            CanvasData = new ObservableCollection<CanvasDataObject>();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            IEnumerable<CanvasDataObject> canvasDataObjects = await StorageManager.GetAllCanvasFromWorkDirectory();

            foreach (var item in canvasDataObjects)
            {
                CanvasData.Add(item);
            };
            base.OnNavigatedTo(e);
        }

        private void listCanvas_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(DrawingPage), e.ClickedItem);
        }

        private async void newCanvas_Click(object sender, RoutedEventArgs e)
        {
            CanvasDataObject newCanvas = await StorageManager.CreateCanvas();

            CanvasData.Add(newCanvas);
            Frame.Navigate(typeof(DrawingPage), newCanvas);
        }

        private void deleteCanvas_Click(object sender, RoutedEventArgs e)
        {

        }

        private void renameCanvas_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
