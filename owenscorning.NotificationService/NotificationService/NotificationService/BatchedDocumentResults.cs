using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OwensCorning.NotificationService
{
    public class BatchedDocumentResults
    {
        public String Status { get; set; }
        public int BatchId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<UpdatedDoc> DocumentList { get; set; }
    }
}
