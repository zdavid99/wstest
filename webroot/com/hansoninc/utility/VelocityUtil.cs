using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

using NVelocity;
using NVelocity.App;

namespace com.hansoninc.utility
{
    public class VelocityUtil
    {
        public static string RenderTemplate(HttpRequest request, String templateName, VelocityContext context)
        {
            Velocity.Init();
            StringWriter body = new StringWriter();
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(request.PhysicalApplicationPath + "/App_LocalResources/" + templateName);
                Velocity.Evaluate(context, body, "", sr);
            }
            finally
            {
                if (sr != null)
                    sr.Close();
            }

            return body.ToString();
        }
    }
}
