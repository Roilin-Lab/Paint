using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPaint
{
    public class CanvasDataObject
    {
        public string Title;
        public string FullName;
        public DateTime CreatedAt;

        public CanvasDataObject(string title)
        {
            Title = title;
            FullName = title + ".gif";
            CreatedAt = DateTime.Now;
        }
    }
}
