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
        public const string FileType = ".gif";
        public DateTime CreatedAt;
        public LayerManager LayerManager;

        public string FullName 
        {
            get => Title + FileType;
        }

        public CanvasDataObject(string title, DateTime dateCreated)
        {
            Title = title;
            CreatedAt = dateCreated;
            LayerManager = new LayerManager();
        }
    }
}
