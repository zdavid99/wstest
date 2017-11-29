using OwensCorning.SurveyService.Data;

namespace com.ocwebservice
{
    // NOTE: If you change the class name "Survey" here, you must also update the reference to "Survey" in Web.config.
    public class Survey:ISurvey
    {
        public int SaveSurvey(SurveyForm form)
        {
            return SurveyDAO.Instance.Save(form);
        }
    }
}
