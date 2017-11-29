using System.ServiceModel;
using OwensCorning.SurveyService.Data;

namespace com.ocwebservice
{
    // NOTE: If you change the interface name "ISurvey" here, you must also update the reference to "ISurveyService" in Web.config.
    [ServiceContract]
    public interface ISurvey
    {
        [OperationContract]
        int SaveSurvey(SurveyForm form);
    }
}
