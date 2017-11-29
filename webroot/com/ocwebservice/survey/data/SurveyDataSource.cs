using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using System.Configuration;

namespace OwensCorning.SurveyService.Data
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors")]
    public class SurveyDataSource
    {
        public static SurveyInformationDataContext ContactDataContext()
        {
            ConnectionStringSettings connectionSettings = ConfigurationManager.ConnectionStrings["OwensCorning.Common"];
            SurveyInformationDataContext contactRepository = new SurveyInformationDataContext(connectionSettings.ConnectionString);
            return contactRepository;
        }
    }
}
