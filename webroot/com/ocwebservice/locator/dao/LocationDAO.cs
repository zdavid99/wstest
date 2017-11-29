using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using com.ocwebservice.locator.data;
using com.ocwebservice.locator.utility;
using com.ocwebservice.locator.model;

namespace com.ocwebservice.locator.dao
{
    public class LocationDAO
    {
        private const int Max_ZipCodeCount = 500;

        private static readonly LocationDAO self = new LocationDAO();
        public static LocationDAO Instance
        {
            get { return self; }
        }

        /// <summary>
        /// Gets the zip codes or postal codes in radius, ordered by proximity.
        /// </summary>
        /// <param name="centerLatitude">The center latitude.</param>
        /// <param name="centerLongitude">The center longitude.</param>
        /// <param name="radius">The radius.</param>
        /// <returns></returns>
        public List<string> GetPostalCodesInRadius(double centerLatitude, double centerLongitude, double radius)
        {
            using (LocationDataContext dataContext = new LocationDataContext())
            {
                List<string> zips =
                    (from zipCode in dataContext.Locations
                     let distance =
                        Math.Acos(
                        Math.Sin(centerLatitude / DistanceCalculationUtil.DEGREES_PER_RADIAN) * Math.Sin((double)zipCode.Latitude.Value / DistanceCalculationUtil.DEGREES_PER_RADIAN) +
                        Math.Cos(centerLatitude) / DistanceCalculationUtil.DEGREES_PER_RADIAN * Math.Cos((double)zipCode.Latitude.Value / DistanceCalculationUtil.DEGREES_PER_RADIAN) *
                        Math.Cos((double)zipCode.Longitude.Value / DistanceCalculationUtil.DEGREES_PER_RADIAN - centerLongitude / DistanceCalculationUtil.DEGREES_PER_RADIAN)
                        ) * DistanceCalculationUtil.EARTH_RADIUS
                     where distance <= radius
                     orderby distance, zipCode.PostalCode
                     select zipCode.PostalCode).Take(Max_ZipCodeCount).ToList();
                return zips;
            }
        }

        /// <summary>
        /// Gets the location with postal code.
        /// </summary>
        /// <param name="postalCode">The postal code.</param>
        /// <returns></returns>
        public Location GetLocationWithPostalCode(string postalCode)
        {
            using (LocationDataContext dataContext = new LocationDataContext())
            {
                var results =
                    (from location in dataContext.Locations
                     where location.PostalCode == postalCode
                     select location);
                if (results.Count() == 0)
                {
                    throw new ArgumentException("Invalid zip code specified: " + postalCode);
                }
                return results.Take(1).Single();
            }
        }
    }
}