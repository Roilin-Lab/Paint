using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Collections.Generic;
using System.Numerics;
using TestPaint.Helpers;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace TestPaint
{
    public sealed partial class DrawingPage : Page
    {
        CanvasDataObject CanvasDataObject;
        private CanvasRenderTarget renderTarget;

        public DrawingPage()
        {
            this.InitializeComponent();

            inkCanvas.InkPresenter.InputDeviceTypes =  CoreInputDeviceTypes.Pen | CoreInputDeviceTypes.Mouse;
        }

        private void DrawGridCanvas()
        {
            InkStrokeBuilder builder = new InkStrokeBuilder();
            InkDrawingAttributes drawAttrs = new InkDrawingAttributes();
            drawAttrs.Color = Color.FromArgb(0, 50, 50, 50);
            drawAttrs.Size = new Size(3, 3);
            builder.SetDefaultDrawingAttributes(drawAttrs);
            for (int i = 0; i < inkCanvas.ActualWidth; i += 10)
            {
                List<InkPoint> horizontalLine = new List<InkPoint>()
                {
                    new InkPoint(new Point(i,0), 0.01f),
                    new InkPoint(new Point(i,inkCanvas.ActualHeight), 0.01f),
                };
                inkCanvas.InkPresenter.StrokeContainer.AddStroke(builder.CreateStrokeFromInkPoints(horizontalLine, Matrix3x2.Identity));
            }
            for (int i = 0; i < inkCanvas.ActualHeight; i += 10)
            { 
                List<InkPoint> verticalLine = new List<InkPoint>()
                {
                    new InkPoint(new Point(0,i), 0.01f),
                    new InkPoint(new Point(inkCanvas.ActualWidth,i), 0.01f),
                };
                inkCanvas.InkPresenter.StrokeContainer.AddStroke(builder.CreateStrokeFromInkPoints(verticalLine, Matrix3x2.Identity));
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            CanvasDataObject = (CanvasDataObject)e.Parameter;
            StorageManager.LoadCanvas(inkCanvas, CanvasDataObject);
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (CanvasDataObject != null)
                StorageManager.SaveCanvas(inkCanvas, CanvasDataObject);

            base.OnNavigatingFrom(e);
        }

        private void canvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
        }

        private void canvasControl_CreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
        {
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            CanvasDataObject.LoadFromCanvas(inkCanvas);
            StorageManager.WriteCanvasToJson(CanvasDataObject);
        }

        private async void LoadBtn_Click(object sender, RoutedEventArgs e)
        {
            CanvasDataObject canvasDataObject = await StorageManager.ReadCanvasFromJson();
            var c = canvasDataObject.LoadToCanvas(inkCanvas);
        }

        private async void ImportBtn_Click(object sender, RoutedEventArgs e)
        {
            var img = new Image();
            var source = await ImageImportHelper.PickImageFileAsync();
            if (source != null)
            {
                img.Source = source;
                img.RenderTransform = new CompositeTransform();
                img.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
                img.ManipulationStarted += Image_OnManipulationStarted;
                img.ManipulationDelta += Image_OnManipulationDelta;
                img.ManipulationCompleted += Image_OnManipulationCompleted;
                canvas.Children.Add(img);
            }
        }

        private void Image_OnManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            ((Image)sender).Opacity = 0.4;
        }

        private void Image_OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var image = (Image)sender;
            var transform = (CompositeTransform)image.RenderTransform;

            //image.SetValue(Canvas.LeftProperty, e.Delta.Translation.X / frameCanvas.ZoomFactor);
            //image.SetValue(Canvas.TopProperty, e.Delta.Translation.Y / frameCanvas.ZoomFactor);

            transform.TranslateX += e.Delta.Translation.X / frameCanvas.ZoomFactor;
            transform.TranslateY += e.Delta.Translation.Y / frameCanvas.ZoomFactor;

            transform.ScaleX *= e.Delta.Scale;
            transform.ScaleY *= e.Delta.Scale;

        }
    
        private void Image_OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            ((Image)sender).Opacity = 1;
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
            inkCanvas.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
            inkCanvas.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.None;
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.SizeAll, 1);
        }

        private void PanBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            inkCanvas.ManipulationMode = ManipulationModes.None;
            inkCanvas.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Pen | CoreInputDeviceTypes.Mouse;
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
