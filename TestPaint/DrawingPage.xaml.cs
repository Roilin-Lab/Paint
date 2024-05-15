using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
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
        private CanvasRenderTarget renderTarget;

        public DrawingPage()
        {
            this.InitializeComponent();

            canvas.InkPresenter.InputDeviceTypes =  CoreInputDeviceTypes.Pen | CoreInputDeviceTypes.Mouse;
        }

        private void DrawGridCanvas()
        {
            InkStrokeBuilder builder = new InkStrokeBuilder();
            InkDrawingAttributes drawAttrs = new InkDrawingAttributes();
            drawAttrs.Color = Color.FromArgb(0, 50, 50, 50);
            drawAttrs.Size = new Size(3, 3);
            builder.SetDefaultDrawingAttributes(drawAttrs);
            for (int i = 0; i < canvas.ActualWidth; i += 10)
            {
                List<InkPoint> horizontalLine = new List<InkPoint>()
                {
                    new InkPoint(new Point(i,0), 0.01f),
                    new InkPoint(new Point(i,canvas.ActualHeight), 0.01f),
                };
                canvas.InkPresenter.StrokeContainer.AddStroke(builder.CreateStrokeFromInkPoints(horizontalLine, Matrix3x2.Identity));
            }
            for (int i = 0; i < canvas.ActualHeight; i += 10)
            { 
                List<InkPoint> verticalLine = new List<InkPoint>()
                {
                    new InkPoint(new Point(0,i), 0.01f),
                    new InkPoint(new Point(canvas.ActualWidth,i), 0.01f),
                };
                canvas.InkPresenter.StrokeContainer.AddStroke(builder.CreateStrokeFromInkPoints(verticalLine, Matrix3x2.Identity));
            }
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

        private void canvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            using(CanvasDrawingSession session = renderTarget.CreateDrawingSession())
            {
                session.DrawText("Hello World", 100, 100, Colors.Black);
            }

            args.DrawingSession.DrawImage(renderTarget);
        }

        private void canvasControl_CreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
        {
            renderTarget = new CanvasRenderTarget(canvasControl, canvasControl.Size);
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            var strokes = canvas.InkPresenter.StrokeContainer.GetStrokes()
                .Select<InkStroke, StrokeData>((inkStroke, strokeData) => new StrokeData(inkStroke.GetInkPoints(), inkStroke.DrawingAttributes));
            Layer layer = new Layer("test", true, 1, strokes);
            CanvasDataObject.LayerManager.AddLayer(layer);
        }

        private void hub_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(HubPage));
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            this.canvasControl.RemoveFromVisualTree();
            this.canvasControl = null;
        }

        private void PanBtn_Checked(object sender, RoutedEventArgs e)
        {
            canvas.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
            canvas.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.None;
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.SizeAll, 1);
        }

        private void PanBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            canvas.ManipulationMode = ManipulationModes.None;
            canvas.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Pen | CoreInputDeviceTypes.Mouse;
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
        }

        private void canvas_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var offsetY = frameCanvas.VerticalOffset;
            var offsetX = frameCanvas.HorizontalOffset;

            offsetY -= e.Delta.Translation.Y;
            offsetX -= e.Delta.Translation.X;

            frameCanvas.ChangeView(offsetX, offsetY, frameCanvas.ZoomFactor, true);
        }

        private void frameCanvas_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Space)
            {
                if ((bool)!PanBtn.IsChecked)
                    PanBtn.IsChecked = true;
                e.Handled = true;
            }
        }

        private void frameCanvas_PreviewKeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Space)
            {
                if ((bool)PanBtn.IsChecked)
                    PanBtn.IsChecked = false;
                e.Handled = true;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            frameCanvas.Focus(FocusState.Programmatic);
        }
    }
}
