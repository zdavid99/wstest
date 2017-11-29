using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OwensCorning.NotificationService
{
    public class UpdatedDoc
    {
        public int batch { get; set; }
        public DateTime dateUpdated { get; set; }
        public string documentName { get; set; }
        public string documentType { get; set; }
        public string fileSize { get; set; }
        public string url { get; set; }
    }
}
