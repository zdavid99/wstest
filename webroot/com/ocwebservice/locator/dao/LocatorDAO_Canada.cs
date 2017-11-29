using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using com.ocwebservice.locator.data;
using com.ocwebservice.locator.utility;
using OwensCorning.Locator.Data;
using OwensCorning.Utility.Data;
using com.ocwebservice.locator.model;
using System.Linq.Expressions;

namespace com.ocwebservice.locator.dao
{
    public class LocatorDAO_Canada : AbstractDAO
    {
        //private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static string PROVINCE_NOT_SET = "--";
        private static readonly LocatorDAO_Canada self = new LocatorDAO_Canada();

        private String dealerLocatorDb;

        private LocatorDAO_Canada()
        {
            // private constructor
            DealerLocatorDb = ConfigurationManager.AppSettings["db.dealerlocator.prefix"];
        }

        public static LocatorDAO_Canada Instance
        {
            get
            {
                return self;
            }
        }

        public String DealerLocatorDb
        {
            get
            {
                return dealerLocatorDb;
            }
            private set
            {
                dealerLocatorDb = value;
            }
        }

        public override int RecordCountTest
        {
            get
            {
                int count = 0;
                try
                {
                    var dealers = new List<CanadianLocatorResult>(GetDealersForHomeowners(float.Parse("51.089650"), float.Parse("-114.051510"), 10, 0));
                    dealers.AddRange(GetDealersForHomeowners(float.Parse("43.700570"), float.Parse("-79.296330"), 25, 1));
                    count = dealers.Count;
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed Record Count test.");
                }
                return count;
            }
        }

        public override string Name
        {
            get
            {
                return GetType().ToString();
            }
        }

        public override bool IsPass
        {
            get
            {
                return (RecordCountTest > 0);
            }
        }

        public IList<CanadianLocatorResult> GetDealersForHomeowners(double latitude, double longitude, int resultsPerPage, int pageNumber)
        {
            // Note: param should contain the postal code and any product filters (comma delim)
            // If no product filters are defined, dealers for ALL products will be returned.

            var param = new object[]
			            	{
			            		latitude, longitude, "Batts", "Foam"
			            	};

            return GetDealers(latitude, longitude, resultsPerPage, pageNumber, param);
        }

        public IList<CanadianLocatorResult> GetDealersForProfessionals(double latitude, double longitude, int resultsPerPage, int pageNumber)
        {
            // Note: param should contain the postal code and any product filters (comma delim)
            // If no product filters are defined, dealers for ALL products will be returned.

            var param = new object[]
			            	{
			            		latitude, longitude, "CommBatts", "CommFoam", "Batts", "Foam"
			            	};

            return GetDealers(latitude, longitude, resultsPerPage, pageNumber, param);
        }

        public IList<CanadianLocatorResult> GetDealers(CanadaLocatorSearchOptions searchOptions)
        {
            var param = new List<object>();
            param.Add(searchOptions.Latitude);
            param.Add(searchOptions.Longitude);
            param.AddRange(searchOptions.Products.ToArray());

            return GetDealers(searchOptions.Latitude, searchOptions.Longitude, searchOptions.ResultsPerPage, searchOptions.PageNumber, param.ToArray());
        }

        public IList<CanadianLocatorResult> GetDealers(double latitude, double longitude, int resultsPerPage, int pageNumber, object[] param)
        {
            // NOTE: Results were multiplied by 1.609344 to convert from MI to KM

            var sql = new StringBuilder();

            sql.AppendLine(" SELECT * FROM ");
            sql.AppendLine(" ( ");
            sql.AppendLine("   SELECT TOP " + resultsPerPage + " * FROM ");
            sql.AppendLine("   ( ");
            sql.AppendLine("     SELECT TOP " + (resultsPerPage * pageNumber) + " ");
            sql.AppendLine("       min(id) AS id, company, address, city, province, postal_code, phone, website, distanceKilometers ");
            sql.AppendLine("     FROM ");
            sql.AppendLine("     ( ");
            sql.AppendLine("	   SELECT ");
            sql.AppendLine("		 D.id, D.product, D.company, D.address, D.city, D.province, D.postal_code, D.phone, D.website, ");
            sql.AppendLine("         (((SQRT((SQUARE((@1) - DZ.longitude) + ");
            sql.AppendLine("         SQUARE((@0) - DZ.latitude)))) * 60) * 1.15078 * 1.609344) ");
            sql.AppendLine("         AS distanceKilometers ");
            sql.AppendLine("       FROM ZipPostalCodes DZ (NOLOCK) ");
            sql.AppendLine("       INNER JOIN " + dealerLocatorDb + "CanadaDealersInfo D (NOLOCK) ");
            sql.AppendLine("         ON DZ.PostalCode = D.postal_code ");

            for (var i = 2; i < param.Length; i++)
            {
                if (i == 2)
                {
                    sql.Append("         WHERE (product=@" + i);
                }
                else
                {
                    sql.Append(" OR product=@" + i);
                }
                if ((i + 1) == param.Length)
                {
                    sql.AppendLine(" ) ");
                }
            }

            sql.AppendLine("     ) AS inner1 ");
            sql.AppendLine("     GROUP BY company, address, city, province, postal_code, phone, website, distanceKilometers ");
            sql.AppendLine("     ORDER BY distanceKilometers, id ");
            sql.AppendLine("   ) AS inner2 ");
            sql.AppendLine("   ORDER BY distanceKilometers desc, id desc ");
            sql.AppendLine(" ) AS inner3 ");
            sql.AppendLine(" ORDER BY distanceKilometers, id  ");

            SqlDataReaderHandler handler = delegate(SqlDataReader rdr)
                                            {
                                                IList<CanadianLocatorResult> list = new List<CanadianLocatorResult>();
                                                while (rdr.Read())
                                                {
                                                    list.Add(BuildDealerItem(rdr));
                                                }
                                                return list;
                                            };

            return ExecuteQuery<IList<CanadianLocatorResult>>(LocatorDatabase.Common, handler, sql.ToString(), param);
        }

        internal static void PopulateStandardFields(SqlDataReader rdr, CanadianLocatorResult d)
        {
            d.Name = rdr.IsDBNull(rdr.GetOrdinal("company")) ? "" : rdr.GetString(rdr.GetOrdinal("company"));
            d.Address = rdr.IsDBNull(rdr.GetOrdinal("address")) ? "" : rdr.GetString(rdr.GetOrdinal("address"));
            d.City = rdr.IsDBNull(rdr.GetOrdinal("city")) ? "" : rdr.GetString(rdr.GetOrdinal("city"));
            d.Province = rdr.IsDBNull(rdr.GetOrdinal("province")) ? "" : rdr.GetString(rdr.GetOrdinal("province"));
            d.PostalCode = rdr.IsDBNull(rdr.GetOrdinal("postal_code")) ? "" : rdr.GetString(rdr.GetOrdinal("postal_code"));
            var phone = rdr.IsDBNull(rdr.GetOrdinal("phone")) ? "" : rdr.GetString(rdr.GetOrdinal("phone"));
            d.Phone = FormatPhoneNumber(phone);
            d.Distance = (Decimal)(rdr.IsDBNull(rdr.GetOrdinal("distanceKilometers")) ? 0.0 : rdr.GetDouble(rdr.GetOrdinal("distanceKilometers")));
            d.Website = rdr.IsDBNull(rdr.GetOrdinal("website")) ? "" : rdr.GetString(rdr.GetOrdinal("website"));
        }

        private static string FormatPhoneNumber(string phone)
        {
            var sPhone = phone.Trim();
            if (sPhone.Length == 10)
            {
                sPhone = string.Format("{0}-{1}-{2}",
                                       sPhone.Substring(0, 3),
                                       sPhone.Substring(3, 3),
                                       sPhone.Substring(6));
            }
            return sPhone;
        }

        private static CanadianLocatorResult BuildDealerItem(SqlDataReader rdr)
        {
            var d = new CanadianLocatorResult(DealerType.Dealer) { Id = rdr.IsDBNull(rdr.GetOrdinal("id")) ? -1 : (int)rdr.GetDecimal(rdr.GetOrdinal("id")) };
            PopulateStandardFields(rdr, d);
            return d;
        }

        public void PopulateLatLongFromZip(CanadaLocatorSearchOptions searchOptions)
        {
            var sql = new StringBuilder();

            sql.AppendLine("select Latitude, Longitude ");
            sql.AppendLine("FROM  ZipPostalCodes DZ (NOLOCK)  ");
            sql.AppendLine("where PostalCode = @0 ");

            SqlDataReaderHandler handler = delegate(SqlDataReader rdr)
            {
                Boolean isData = false;
                if (rdr.HasRows)
                {
                    if (rdr.Read())
                    {
                        searchOptions.Latitude = (rdr.IsDBNull(rdr.GetOrdinal("Latitude")) ? 0.0 : double.Parse(rdr[rdr.GetOrdinal("Latitude")].ToString()));
                        searchOptions.Longitude = (rdr.IsDBNull(rdr.GetOrdinal("Longitude")) ? 0.0 : double.Parse(rdr[rdr.GetOrdinal("Longitude")].ToString()));
                    }
                }
                return isData;
            };

            ExecuteQuery<Boolean>(LocatorDatabase.Common, handler, sql.ToString(), searchOptions.PostalCode);
        }
    }
}
