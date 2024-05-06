using System;
using System.Collections.Generic;
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
        private IEnumerable<CanvasDataObject> CanvasData;

        public HubPage()
        {
            this.InitializeComponent();

            CanvasData = new List<CanvasDataObject>() 
            { 
                new CanvasDataObject("Untitled"), 
                new CanvasDataObject("Hello World") 
            };
        }

        private void listCanvas_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(DrawingPage), e.ClickedItem);
        }
    }
}
