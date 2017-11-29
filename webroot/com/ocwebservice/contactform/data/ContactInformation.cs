using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;
using System;

namespace OwensCorning.ContactService.Data
{
    partial class ContactInformationDataContext
    {

        [Function(Name = "dbo.GetDmaXmlFromFormData", IsComposable = true)]
        [return: Parameter(DbType = "ntext")]
        public string GetDmaXmlFromFormData([Parameter(Name = "FormData",
            DbType = "ntext")] string @FormData)
        {
            return ((string)(this.ExecuteMethodCall(this,
                ((MethodInfo)(MethodInfo.GetCurrentMethod())),
                @FormData).ReturnValue));
        }
    }
}
