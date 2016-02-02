using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Button_Crushers_RSS_Client.Properties;
using LINQtoCSV;

namespace Button_Crushers_RSS_Client.Map_Utility
{

    class Location
    {
        [CsvColumn(Name = "ID", FieldIndex = 1)]
        public string ID { get; set; }
        [CsvColumn(FieldIndex = 2)]
        public string Country { get; set; }
        [CsvColumn(FieldIndex = 3, CanBeNull = false, OutputFormat = "C")]
        public string State { get; set; }
        [CsvColumn(FieldIndex = 4)]
        public string City { get; set; }
        [CsvColumn(FieldIndex = 5)]
        public string PostalCode { get; set; }
        [CsvColumn(FieldIndex = 6)]
        public string Latitude { get; set; }
        [CsvColumn(FieldIndex = 7)]
        public string Longitude { get; set; }
        [CsvColumn(FieldIndex = 8)]
        public string MetroCode { get; set; }
        [CsvColumn(FieldIndex = 9)]
        public string AreaCode { get; set; }
    }
    public static class CsvAdapter
    {
        private static CsvFileDescription inputFileDescription = new CsvFileDescription
        {
            SeparatorChar = ',',
            FirstLineHasColumnNames = true
        };

        private static CsvContext cc = new CsvContext();

        private static IEnumerable<Location> Locations;

        private static Dictionary<string, Location> CityAndStateToLocation = new Dictionary<string, Location>();
            

        static CsvAdapter()
        {
            Locations = cc.Read<Location>(@"uslocations.csv", inputFileDescription);

            foreach (var location in Locations)
            {
                if (location.City != null && !CityAndStateToLocation.Keys.Contains(location.City))
                {
                    CityAndStateToLocation.Add(location.City, location);
                }
                else if (location.State != null && !CityAndStateToLocation.Keys.Contains(location.State))
                {
                    CityAndStateToLocation.Add(location.State, location);
                }
            }
        }


        public static Tuple<string, string, string> SearchForLatitudeAndLongitude(IEnumerable<string> queryStrings)
        {
            Tuple<string, string, string> result = null;

            var locationDataObject = Locations.FirstOrDefault(item => queryStrings.Contains(item.City));

            if (locationDataObject != null)
            {
                result = new Tuple<string, string, string>(locationDataObject.City, locationDataObject.Latitude, locationDataObject.Longitude);
            }

            return result; 
        }

        /// <summary>
        /// less brute forcey, much quicker
        /// </summary>
        /// <param name="queryStrings"></param>
        /// <returns></returns>
        public static Tuple<string, string, string> LookUpCity(IEnumerable<string> queryStrings)
        {
            var city = CityAndStateToLocation.Keys.FirstOrDefault(queryStrings.Contains);
            Tuple<string, string, string> result = null;

            if (city != null)
            {
                var locationData = CityAndStateToLocation[city];
                result = new Tuple<string, string, string>(locationData.City, locationData.Latitude, locationData.Longitude);
            }

            return result;
        }

    }
}
