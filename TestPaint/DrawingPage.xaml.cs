using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;


namespace TestPaint
{
    public sealed partial class DrawingPage : Page
    {
        CanvasDataObject CanvasDataObject;

        public DrawingPage()
        {
            this.InitializeComponent();

            canvas.InkPresenter.InputDeviceTypes = Windows.UI.Core.CoreInputDeviceTypes.Mouse | Windows.UI.Core.CoreInputDeviceTypes.Pen;
        }

        private void colorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            InkDrawingAttributes drawAttrs = new InkDrawingAttributes();
            drawAttrs.Color = colorPicker.Color;
            drawAttrs.Size = new Size(5, 5);
            canvas.InkPresenter.UpdateDefaultDrawingAttributes(drawAttrs);
        }

        private void size_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (frameCanvas != null)
                frameCanvas.ChangeView(0, 0, (float)e.NewValue, false);
        }

        private void canvas_Loaded(object sender, RoutedEventArgs e)
        {
            InkStrokeBuilder builder = new InkStrokeBuilder();
            InkDrawingAttributes drawAttrs = new InkDrawingAttributes();
            drawAttrs.Color = Color.FromArgb(0, 75, 75, 75);
            drawAttrs.FitToCurve = false;
            drawAttrs.Size = new Size(1, 1);
            builder.SetDefaultDrawingAttributes(drawAttrs);
            for (int i = 0; i < canvas.ActualWidth; i += 10)
            {
                List<InkPoint> horizontalLine = new List<InkPoint>()
                {
                    new InkPoint(new Point(i,0), 0.1f),
                    new InkPoint(new Point(i,canvas.ActualHeight), 0.1f),
                };
                canvas.InkPresenter.StrokeContainer.AddStroke(builder.CreateStrokeFromInkPoints(horizontalLine, Matrix3x2.Identity));

                List<InkPoint> verticalLine = new List<InkPoint>()
                {
                    new InkPoint(new Point(0,i), 0.1f),
                    new InkPoint(new Point(canvas.ActualWidth,i), 0.1f),
                };
                canvas.InkPresenter.StrokeContainer.AddStroke(builder.CreateStrokeFromInkPoints(verticalLine, Matrix3x2.Identity));
            }
        }

        private void hub_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(HubPage));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            CanvasDataObject = (CanvasDataObject)e.Parameter;
            StorageManager.LoadCanvas(canvas, CanvasDataObject);
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (CanvasDataObject != null)
                StorageManager.SaveCanvas(canvas, CanvasDataObject);

            base.OnNavigatingFrom(e);
        }
    }
}
