using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml.Controls;

namespace TestPaint
{
    public class CanvasDataObject
    {
        public string Title;
        public const string FileType = ".gif";
        public DateTime CreatedAt;
        public List<StrokeData> Strokes { get; set; }

        public string FullName 
        {
            get => Title + FileType;
        }

        [JsonConstructor]
        public CanvasDataObject(string title, DateTime createdAt, List<StrokeData> strokes) : this(title, createdAt)
        {
            Strokes = strokes;
        }

        public CanvasDataObject(string title, DateTime dateCreated)
        {
            Title = title;
            CreatedAt = dateCreated;
            Strokes = new List<StrokeData>();
        }

        public void LoadFromCanvas(InkCanvas canvas)
        {
            var inkStrokes = canvas.InkPresenter.StrokeContainer.GetStrokes();
            
            foreach (var inkStroke in inkStrokes)
            {
                var inkDrawAttrs = inkStroke.DrawingAttributes;
                var points = inkStroke.GetInkPoints().Select((inkPoint) => new PointData()
                {
                    X = inkPoint.Position.X,
                    Y = inkPoint.Position.Y,
                    Pressure = inkPoint.Pressure,
                    TiltX = inkPoint.TiltX,
                    TiltY = inkPoint.TiltY,
                    Timestamp = inkPoint.Timestamp
                });
                DrawingAttributesData drawAttrs = new DrawingAttributesData()
                {
                    Size = inkDrawAttrs.Size,
                    PenTip = inkDrawAttrs.PenTip,
                    IgnorePressure = inkDrawAttrs.IgnorePressure,
                    FitToCurve = inkDrawAttrs.FitToCurve,
                    Color = inkDrawAttrs.Color,
                    PenTipTransform = inkDrawAttrs.PenTipTransform,
                    DrawAsHighlighter = inkDrawAttrs.DrawAsHighlighter,
                    Kind = inkDrawAttrs.Kind,
                    Opacity = inkDrawAttrs.PencilProperties?.Opacity ?? null,
                    IgnoreTilt = inkDrawAttrs.IgnoreTilt,
                    ModelerAttributes = new ModelerAttributesData()
                    {
                        ScalingFactor = inkDrawAttrs.ModelerAttributes.ScalingFactor,
                        PredictionTime = inkDrawAttrs.ModelerAttributes.PredictionTime,
                        UseVelocityBasedPressure = inkDrawAttrs.ModelerAttributes.UseVelocityBasedPressure,
                    }
                };

                Strokes.Add(new StrokeData(points, drawAttrs));
            }
        }

        public IEnumerable<InkStroke> LoadToCanvas(InkCanvas canvas)
        {
            var inkBuilder = new InkStrokeBuilder();
            var strokes = new List<InkStroke>();

            if (Strokes.Count == 0)
                return strokes;

            foreach(var stroke in Strokes)
            {
                InkDrawingAttributes drawAttrs;
                if (stroke.Attributes.Kind == InkDrawingAttributesKind.Pencil)
                {
                    drawAttrs = InkDrawingAttributes.CreateForPencil();
                    drawAttrs.PencilProperties.Opacity = (double)stroke.Attributes.Opacity;
                }
                else
                {
                    drawAttrs = new InkDrawingAttributes();
                    drawAttrs.PenTip = stroke.Attributes.PenTip;
                    drawAttrs.PenTipTransform = stroke.Attributes.PenTipTransform;
                    drawAttrs.DrawAsHighlighter = stroke.Attributes.DrawAsHighlighter;
                }
                drawAttrs.Size = stroke.Attributes.Size;
                drawAttrs.IgnorePressure = stroke.Attributes.IgnorePressure;
                drawAttrs.FitToCurve = stroke.Attributes.FitToCurve;
                drawAttrs.Color = stroke.Attributes.Color;
                drawAttrs.IgnoreTilt = stroke.Attributes.IgnoreTilt;
                drawAttrs.ModelerAttributes.ScalingFactor = stroke.Attributes.ModelerAttributes.ScalingFactor;
                drawAttrs.ModelerAttributes.PredictionTime = stroke.Attributes.ModelerAttributes.PredictionTime;
                drawAttrs.ModelerAttributes.UseVelocityBasedPressure = stroke.Attributes.ModelerAttributes.UseVelocityBasedPressure;

                var points = stroke.points.Select((p) => new InkPoint(p.GetPoint(), p.Pressure, p.TiltX, p.TiltY, p.Timestamp));
                inkBuilder.SetDefaultDrawingAttributes(drawAttrs);

                strokes.Add(inkBuilder.CreateStrokeFromInkPoints(points, Matrix3x2.Identity));
            }
            canvas.InkPresenter.StrokeContainer.AddStrokes(strokes);

            return strokes;
        }
    }
}
