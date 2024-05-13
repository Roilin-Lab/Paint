using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public List<InkPoint> points;
        public InkDrawingAttributes Attributes;

        public StrokeData(IEnumerable<InkPoint> points, InkDrawingAttributes attributes)
        {
            this.points = points.ToList();
            Attributes = attributes;
        }
    }
}
