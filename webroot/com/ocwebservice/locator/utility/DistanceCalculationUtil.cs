using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;

namespace com.ocwebservice.locator.utility
{
    /// <summary>
    /// Summary description for Constants
    /// </summary>
    public class DistanceCalculationUtil
    {
        public const double DEGREES_PER_RADIAN = 57.2958;
        public const double EARTH_RADIUS = 3958.75;
        public static Expression<Func<double, double, double, double, double>> CalculateDistance =
            (lat1, long1, lat2, long2) =>
                Math.Acos(
                    Math.Sin(lat1 / DEGREES_PER_RADIAN) * Math.Sin(lat2 / DEGREES_PER_RADIAN) +
                    Math.Cos(lat1) / DEGREES_PER_RADIAN * Math.Cos(lat2 / DEGREES_PER_RADIAN) *
                    Math.Cos(long2 / DEGREES_PER_RADIAN - long1 / DEGREES_PER_RADIAN)
                ) * EARTH_RADIUS;
        /*
         * OLD METHOD - USE IF ARCCOSINE IS NOT AVAILABLE
         * 
            EARTH_RADIUS * (
                Math.Atan(
                    Math.Sqrt(1 - 
                        Math.Pow(
                            Math.Sin(searchOptions.Latitude / DEGREES_PER_RADIAN) * Math.Sin(zipCode.Latitude.Value / DEGREES_PER_RADIAN) +
                            Math.Cos(searchOptions.Latitude / DEGREES_PER_RADIAN) * Math.Cos(zipCode.Latitude.Value / DEGREES_PER_RADIAN) *
                            Math.Cos(zipCode.Longitude.Value / DEGREES_PER_RADIAN - searchOptions.Longitude / DEGREES_PER_RADIAN)
                        , 2))
                /
                    (Math.Sin(searchOptions.Latitude / DEGREES_PER_RADIAN) * Math.Sin(zipCode.Latitude.Value / DEGREES_PER_RADIAN) +
                    Math.Cos(searchOptions.Latitude / DEGREES_PER_RADIAN) * Math.Cos(zipCode.Latitude.Value / DEGREES_PER_RADIAN) *
                    Math.Cos(zipCode.Longitude.Value / DEGREES_PER_RADIAN - searchOptions.Longitude / DEGREES_PER_RADIAN))
                )
            )
        */
    }
}