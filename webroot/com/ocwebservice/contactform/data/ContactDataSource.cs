using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using System.Configuration;

namespace OwensCorning.ContactService.Data
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors")]
    public class ContactDataSource
    {
        public static ContactInformationDataContext ContactDataContext()
        {
            ConnectionStringSettings connectionSettings = ConfigurationManager.ConnectionStrings["dao.owens.sql.connectionstring"];
            ContactInformationDataContext contactRepository = new  ContactInformationDataContext(connectionSettings.ConnectionString);
            return contactRepository;
        }
    }
}
