using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Input.Inking;

namespace TestPaint
{
    public class LayerManager
    {
        private List<Layer> layers;

        public List<Layer> Layers { get => layers; }

        public LayerManager()
        {
            layers = new List<Layer>();
        }

        public void CreateEmptyLayer()
        {
            layers.Add(new Layer(layers.Count.ToString(), true, 1));
        }

        public void AddLayer(Layer layer)
        {
            layers.Add(layer);
        }

        public void RemoveLayer(Layer layer)
        {
            layers.Remove(layer);
        }

        internal IEnumerable<StrokeData> GetStrokes()
        {
            return (IEnumerable<StrokeData>)layers.Select((layer) => layer.Strokes.ToArray());
        }
    }
}
