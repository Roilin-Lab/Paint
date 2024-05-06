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
        public string FileType;
        public DateTime CreatedAt;

        public string FullName 
        {
            get => Title + FileType;
        }

        public CanvasDataObject(string title, string fileType, DateTime dateCreated)
        {
            Title = title;
            FileType = fileType;
            CreatedAt = dateCreated;
        }
    }
}
