using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Devices.Geolocation;

namespace LazyTodo
{
    

    namespace LazyTodo
    {
        public class Location : IComparable<Location>
        {
            public string Name { get; set; }
            public string UniqueId { get; set; }
            public string Address { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public double Distance 
            { 
                get 
                {
                    return GetDistanceTo(LocationDataSource._locationDataSource.CurrentLocation);
                }
            }
            public Location()
            {
                this.Name = "";
                this.UniqueId = "";
                this.Address = "";
                this.Latitude = 0;
                this.Longitude = 0;
            }
            public Location(String name, String uniqueId)
            {
                this.Name = name;
                this.UniqueId = uniqueId;
                this.Address = "";
                this.Latitude = 0;
                this.Longitude = 0;
            }
            public Location(Double latitude, Double longitude)
            {
                this.Name = "@ " + latitude + ", " + longitude;
                this.Address = "";
                this.Latitude = latitude;
                this.Longitude = longitude;
                this.UniqueId = "" + Guid.NewGuid();
            }
            public Location(String name, String uniqueId, String address, Double latitude, Double longitude)
            {
                this.Name = name;
                this.UniqueId = uniqueId;
                this.Address = address;
                this.Latitude = latitude;
                this.Longitude = longitude;
            }
            public override string ToString()
            {
                string s = "Name: " + this.Name + ",  " +
                            "Address: " + this.Address + ", " +
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
            public async Task<List<Location>> GetPredictionAsync(string input)
            {
                string response = await ReadPredictionJsonAsync(this.CurrentLocation, input);
                Debug.WriteLine(response);

                JsonObject jsonResult = JsonObject.Parse(response);
                JsonArray places = jsonResult["predictions"].GetArray();

                List<Location> predictions = new List<Location>();

                foreach (JsonValue place in places)
                {
                    JsonObject placeObject = place.GetObject();
                    string description = placeObject["description"].GetString();
                    //Debug.WriteLine(name);
                    string uniqueId = placeObject["place_id"].GetString();
                    //Debug.WriteLine(uniqueId);

                    Location location = new Location(description, uniqueId);

                    predictions.Add(location);
                }

                return predictions;

            }
            public async Task<string> ReadPredictionJsonAsync(Location location, string input)
            {
                return await ReadPredictionJsonAsync(location.Latitude, location.Longitude, input);
            }
            public async Task<string> ReadPredictionJsonAsync(double latitude, double longitude, string inputString)
            {
                string jsonURL = "https://maps.googleapis.com/maps/api/place/autocomplete/json";
                string APIKey = "key=AIzaSyBf2rgSAk4ZHmNn_rylKZbUx4wsQ1aGrhI";
                string location = "location=" + latitude + "%2C" + longitude;
                string radius = "radius=10000";
                string input = "input=" + inputString;

                HttpClient client = new HttpClient();
                var resource = await client.GetAsync(new Uri(jsonURL + "?" + APIKey + "&" + input + "&" + location + "&" + radius));
                var response = await resource.Content.ReadAsStringAsync();

                return response;
            }
            public async Task GetNearByPlacesAsync()
            {
                await GetCurrentLocation();
                string response = await ReadNearByJsonAsync(this.CurrentLocation);

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

                    Location location = new Location(name, uniqueId, "", latitude, longitude);

                    this._placesNearBy.Add(location);
                }
            }
            public async Task<string> ReadNearByJsonAsync(Location location)
            {
                return await ReadNearByJsonAsync(location.Latitude, location.Longitude);
            }
            public async Task<string> ReadNearByJsonAsync(double latitude, double longitude)
            {
                string jsonURL = "https://maps.googleapis.com/maps/api/place/nearbysearch/json";
                string APIKey = "key=AIzaSyBf2rgSAk4ZHmNn_rylKZbUx4wsQ1aGrhI";
                string location = "location=" + latitude + "%2C" + longitude;
                string radius = "radius=10000";

                HttpClient client = new HttpClient();
                var resource = await client.GetAsync(new Uri(jsonURL + "?" + APIKey + "&" + location + "&" + radius));
                var response = await resource.Content.ReadAsStringAsync();

                return response;
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
            

            /** static **/
            public static LocationDataSource _locationDataSource = new LocationDataSource();
            public async static Task<LocationDataSource> getInstance(bool forceUpdate)
            {
                await _locationDataSource.GetCurrentLocation();
                //if (forceUpdate) await _locationDataSource.GetNearByPlacesAsync();
                foreach (Location location in _locationDataSource.PlacesNearBy)
                {
                    Debug.WriteLine(location);
                }
                return _locationDataSource;
            }
            public static async Task<Location> GetLocationFromId(string id)
            {
                string jsonURL = "https://maps.googleapis.com/maps/api/place/details/json";
                string APIKey = "key=AIzaSyBf2rgSAk4ZHmNn_rylKZbUx4wsQ1aGrhI";
                string placeId = "placeid=" + id;

                HttpClient cliend = new HttpClient();
                var resource = await cliend.GetAsync(new Uri(jsonURL + "?" + APIKey + "&" + placeId));
                var response = await resource.Content.ReadAsStringAsync();

                JsonObject jsonResult = JsonObject.Parse(response);
                JsonObject placeDetail = jsonResult["result"].GetObject();

                string name = placeDetail["name"].GetString();
                Debug.WriteLine(name);
                JsonObject placeLocation = placeDetail["geometry"].GetObject()["location"].GetObject();
                double latitude = placeLocation["lat"].GetNumber();
                double longitude = placeLocation["lng"].GetNumber();

                string address = placeDetail.GetNamedString("formatted_address", "");

                Location location = new Location(name, id, address, latitude, longitude);
                return location;

            }
        }
    }
    
}
