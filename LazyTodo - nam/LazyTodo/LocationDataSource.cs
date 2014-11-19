using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Devices.Geolocation;

namespace LazyTodo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace LazyTodo
    {
        public class Location : IComparable<Location>
        {
            public string Name { get; private set; }
            public string UniqueId { get; private set; }
            public double Latitude { get; private set; }
            public double Longitude { get; private set; }
            public double Distance 
            { 
                get 
                {
                    return GetDistanceTo(LocationDataSource._locationDataSource.CurrentLocation);
                }
            }
            public Location(Double latitude, Double longitude)
            {
                this.Name = "@ " + latitude + ", " + longitude;
                this.Latitude = latitude;
                this.Longitude = longitude;
                this.UniqueId = "" + Guid.NewGuid();
            }
            public Location(String name, String uniqueId, Double latitude, Double longitude)
            {
                this.Name = name;
                this.UniqueId = uniqueId;
                this.Latitude = latitude;
                this.Longitude = longitude;
            }
            public override string ToString()
            {
                string s = "Name: " + this.Name + ",  " +
                            "UniqueId: " + this.UniqueId + ",  " +
                            "Coordinate: @" + this.Latitude + ", " + this.Longitude;
                return s;
            }
            public int CompareTo(Location other)
            {

                // Alphabetic sort if distance is equal. [A to Z]
                if (this.Distance == other.Distance)
                {
                    return this.Name.CompareTo(other.Name);
                }
                // Default to distance sort. [low to high]
                return this.Distance.CompareTo(other.Distance);
            }

            public double GetDistanceTo(Location other)
            {
                if (double.IsNaN(this.Latitude) || double.IsNaN(this.Longitude) || double.IsNaN(other.Latitude) || double.IsNaN(other.Longitude))
                {
                    throw new ArgumentException("Argument_LatitudeOrLongitudeIsNotANumber");
                }
                else
                {
                    double latitude = this.Latitude * 0.0174532925199433;
                    double longitude = this.Longitude * 0.0174532925199433;
                    double num = other.Latitude * 0.0174532925199433;
                    double longitude1 = other.Longitude * 0.0174532925199433;
                    double num1 = longitude1 - longitude;
                    double num2 = num - latitude;
                    double num3 = Math.Pow(Math.Sin(num2 / 2), 2) + Math.Cos(latitude) * Math.Cos(num) * Math.Pow(Math.Sin(num1 / 2), 2);
                    double num4 = 2 * Math.Atan2(Math.Sqrt(num3), Math.Sqrt(1 - num3));
                    double num5 = 6376500 * num4;
                    return num5;
                }
            }
            
        }
        public class LocationDataSource
        {
            public Location CurrentLocation;

            private SortedSet<Location> _placesNearBy = new SortedSet<Location>();
            public SortedSet<Location> PlacesNearBy
            {
                get { return this._placesNearBy; }
            }
            public async Task GetPlacesAsync()
            {
                await GetCurrentLocation();
                string response = await ReadJsonFromGoogleAsync(this.CurrentLocation);

                JsonObject jsonResult = JsonObject.Parse(response);
                JsonArray places = jsonResult["results"].GetArray();

                foreach (JsonValue place in places) 
                {
                    JsonObject placeObject = place.GetObject();

                    JsonObject placeLocation = placeObject["geometry"].GetObject()["location"].GetObject();
                    double latitude = placeLocation["lat"].GetNumber();
                    //Debug.WriteLine(latitude);
                    double longitude = placeLocation["lng"].GetNumber();
                    //Debug.WriteLine(longitude);
                    string name = placeObject["name"].GetString();
                    //Debug.WriteLine(name);
                    string uniqueId = placeObject["place_id"].GetString();
                    //Debug.WriteLine(uniqueId);

                    Location location = new Location(name, uniqueId, latitude, longitude);

                    this._placesNearBy.Add(location);
                }
            }
            public async Task GetCurrentLocation()
            {
                //Current Location
                var locator = new Geolocator();
                locator.DesiredAccuracyInMeters = 50;

                // MUST ENABLE THE LOCATION CAPABILITY!!!
                var position = await locator.GetGeopositionAsync();

                double latitude = position.Coordinate.Latitude;
                double longitude = position.Coordinate.Longitude;

                this.CurrentLocation = new Location(latitude, longitude);
            }
            public async Task<string> ReadJsonFromGoogleAsync(Location location)
            {
                return await ReadJsonFromGoogleAsync(location.Latitude, location.Longitude);
            }
            public async Task<string> ReadJsonFromGoogleAsync(double latitude, double longitude)
            {
                string jsonURL = "https://maps.googleapis.com/maps/api/place/nearbysearch/json";
                string location = "location=" + latitude + "%2C" + longitude;
                string radius = "radius=10000";
                string APIKey = "key=AIzaSyBf2rgSAk4ZHmNn_rylKZbUx4wsQ1aGrhI";

                HttpClient client = new HttpClient();
                var resource = await client.GetAsync(new Uri(jsonURL + "?" + location + "&" + radius + "&" + APIKey));
                var response = await resource.Content.ReadAsStringAsync();

                return response;
            }

            /** static **/
            public static LocationDataSource _locationDataSource = new LocationDataSource();
            public async static Task<LocationDataSource> getInstance()
            {
                //if (_locationDataSource._placesNearBy.Count == 0)
                await _locationDataSource.GetPlacesAsync();
                foreach (Location location in _locationDataSource.PlacesNearBy)
                {
                    Debug.WriteLine(location);
                }
                return _locationDataSource;
            }
        }
    }
    
}
