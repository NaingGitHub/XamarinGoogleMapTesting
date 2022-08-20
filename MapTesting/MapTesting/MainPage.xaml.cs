using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace MapTesting
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            bool isFirstPin = true;
            var polyline = new Polyline();
            double lastLat = 0;
            double lastLon = 0;
            map.MyLocationEnabled = true;
            map.UiSettings.MyLocationButtonEnabled = true;
            map.MapLongClicked += Map_MapLongClicked;

            map.MapClicked += (sender, e) =>
            {
                var lat = e.Point.Latitude.ToString("0.000");
                var lng = e.Point.Longitude.ToString("0.000");
                Pin pin = new Pin()
                {
                    Type = PinType.Place,
                    Label = "PIN 1",
                    Address = $"{lat}/{lng}",
                    Position = new Position(e.Point.Latitude, e.Point.Longitude),
                    Tag = "PIN1",
                    IsDraggable = true
                };

                map.Pins.Add(pin);
                if (isFirstPin)
                {
                    isFirstPin = false;
                }
                else
                {
                    var oldPosition = new Position(lastLat, lastLon);
                    var newPosition = new Position(e.Point.Latitude, e.Point.Longitude);
                    polyline.Positions.Clear();
                    polyline.Positions.Add(oldPosition);
                    polyline.Positions.Add(newPosition);
                    polyline.StrokeColor = Color.Red;
                    polyline.StrokeWidth = 3f;
                    map.Polylines.Add(polyline);

                    Location sourceCoordinates = new Location(lastLat, lastLon);
                    Location destinationCoordinates = new Location(e.Point.Latitude, e.Point.Longitude);
                    double distance = Location.CalculateDistance(sourceCoordinates, destinationCoordinates, DistanceUnits.Kilometers);
                    this.DisplayAlert("Difference Between 2 Points", $"{distance} Kilometers", "CLOSE");
                }
                lastLat = e.Point.Latitude;
                lastLon = e.Point.Longitude;
            };

            map.PinDragEnd += (sender, e) =>
            {
                var lat = e.Pin.Position.Latitude.ToString("0.000");
                var lng = e.Pin.Position.Longitude.ToString("0.000");
                this.DisplayAlert("New Position", $"{lat}/{lng}", "CLOSE");
            };
        }

        private void Map_MapLongClicked(object sender, MapLongClickedEventArgs e)
        {
            var lat = e.Point.Latitude.ToString("0.000");
            var lng = e.Point.Longitude.ToString("0.000");
            this.DisplayAlert("MapLongClicked", $"{lat}/{lng}", "CLOSE");
        }

        private void map_MyLocationButtonClicked(object sender, MyLocationButtonClickedEventArgs e)
        {
            
        }

        private async void SearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            var geocoder = new Xamarin.Forms.GoogleMaps.Geocoder();
            var positions = await geocoder.GetPositionsForAddressAsync(searchbar.Text);
            if (positions.Count() > 0)
            {
                var pos = positions.First();
                map.MoveToRegion(MapSpan.FromCenterAndRadius(pos, Distance.FromMeters(5000)));
                var reg = map.Region;
            }
            else
            {
                await this.DisplayAlert("Not found", "Geocoder returns no results", "Close");
            }
        }

        private void map_MeasureInvalidated(object sender, EventArgs e)
        {

        }
    }
}
