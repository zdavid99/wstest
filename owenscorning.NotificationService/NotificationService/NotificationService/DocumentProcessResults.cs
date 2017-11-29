using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OwensCorning.NotificationService
{
    public class DocumentProcessResults
    {
        public String Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DocumentsInBatch { get; set; }
        public int BatchId { get; set; }
    }
}
