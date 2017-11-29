using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OwensCorning.NotificationService
{
    public class DocumentProcessOptions
    {
        public string Site { get; set; }
        public string TaxonomyName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<string> FileTypes { get; set; }
    }
}
