using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Helpers
{
    public static class HelperMethods
    {
        
        public static DateTime UnixTimeStampToDateTime( long unixTimeStamp)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            dateTime = dateTime.AddSeconds(unixTimeStamp);

            return dateTime;
        }

        public static string ConvertWeather(int weather)
        {
            switch (weather)
            {
                case 0: return "completely cloudy";
                case 1: return "cloudy";
                case 2: return "clear";
                default: return "";
            }
        }

        public static ICollection<DateTime> GetDateTimeList(DateTime startDate)
        {
            var result = new List<DateTime>();

            while (startDate <= DateTime.UtcNow.Date)
            {
                result.Add(startDate);

                startDate = startDate.AddDays(1);
            }

            return result;
        } 

        public static string GetClientId(this ClaimsPrincipal user)
        {
            ClaimsIdentity? identity = user.Identity as ClaimsIdentity;   //.Claims.ElementAt(11);

            if (identity != null)
            {
                return identity.Claims.First(x => x.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier").Value!;

            }

            return string.Empty;
        }
    }

    public class DistanceCalculator
    {
        private const double Radius = 6371.230;
        public double LatMin { get;}
        public double LatMax { get;}
        public double LngMin { get;}
        public double LngMax { get;}
        public double Distance { get;}
        public double LatDiff { get;}
        public double LngDiff { get;}
        public double Lat { get; set; }
        public double Lng { get; set; }
        public DistanceCalculator(double lat, double lng, double distance)
        {
            Distance = distance;
            LatDiff = distance/Radius * (180 / Math.PI);
            LngDiff = Math.Asin(distance/Radius) / Math.Cos(Math.PI/180 * lat) * (180 / Math.PI);
            LatMin = lat - LatDiff > -90? lat - LatDiff : -180 - (lat - LatDiff);
            LatMax = lat + LatDiff < 90? lat + LatDiff : 180 - (lat + LatDiff);
            LngMin = lng - LngDiff > -180? lng - LngDiff : 360 + (lng - LngDiff);
            LngMax = lng + LngDiff < 180? lng + LngDiff : -360 + (lng + LngDiff);
            Lat = lat;
            Lng = lng;
        }

        public bool IsWithinRange(double lat, double lng)
        {
            return (lat > LatMin && lat < LatMax && lng > LngMin && lng < LngMax);
        }
        public double CalculateDistance(double latCompare, double lngCompare)
        {
            return Math.Pow((Math.Pow((Lat - latCompare), 2) + Math.Pow((Lng - lngCompare), 2)), 0.5);
        }
    }
}

