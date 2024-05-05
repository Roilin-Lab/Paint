using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPaint
{
    class CanvasDataObject
    {
        public string Title;
        public DateTime CreatedAt;

        public CanvasDataObject(string title)
        {
            Title = title;
            CreatedAt = DateTime.Now;
        }
    }
}
