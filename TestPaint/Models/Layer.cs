using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Input.Inking;

namespace TestPaint
{
    public class Layer
    {
        [JsonConstructor]
        public Layer(string name, bool isVisible, double opacity, IEnumerable<StrokeData> strokes) : this(name, isVisible, opacity)
        {
            Strokes = strokes.ToList();
        }

        public Layer(string name, bool isVisible, double opacity)
        {
            Name = name;
            IsVisible = isVisible;
            Opacity = opacity;
            Strokes = new List<StrokeData>();
        }
        
        public string Name { get; set; }
        public bool IsVisible { get; set; }
        public double Opacity { get; set; }
        public List<StrokeData> Strokes { get; set; }
    }

    public struct StrokeData
    {
        public List<PointData> points;
        public DrawingAttributesData Attributes;

        public StrokeData(IEnumerable<PointData> points, DrawingAttributesData attributes)
        {
            this.points = points.ToList();
            Attributes = attributes;
        }
    }
    public struct PointData
    {
        public double X { get; set; }
        public double Y { get; set; }
        public float Pressure { get; set; }
        public float TiltX { get; set; }
        public float TiltY { get; set; }
        public ulong Timestamp { get; set; }

        public Point GetPoint()
        {
            return new Point(X, Y);
        }
    }
    public struct DrawingAttributesData
    {
        public Size Size { get; set; }
        public PenTipShape PenTip { get; set; }
        public bool IgnorePressure { get; set; }
        public bool FitToCurve { get; set; }
        public Color Color { get; set; }
        public Matrix3x2 PenTipTransform { get; set; }
        public bool DrawAsHighlighter { get; set; }
        public InkDrawingAttributesKind Kind { get; set; }
        public double? Opacity { get; set; }
        public bool IgnoreTilt { get; set; }
        public ModelerAttributesData ModelerAttributes { get; set; }
    }
    public struct ModelerAttributesData 
    {
        public float ScalingFactor { get; set; }
        public TimeSpan PredictionTime { get; set; }
        public bool UseVelocityBasedPressure { get; set; }
    }
}
