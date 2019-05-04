using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace mband
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Search : ContentPage
    {
        private FeatureSearch featureSearch = new FeatureSearch();
        public ObservableCollection<Models> Modelss { get; private set; }
        public ICommand SearchCommand { get; private set; }
        public object JsonRequestBehavior { get; private set; }
        static SearchIndexClient indexClient;

        public Search()
        {
            InitializeComponent();
            BackgroundColor = Color.FromHex("82DAAA");

            Modelss = new ObservableCollection<Models>();
            indexClient = new SearchIndexClient(Constants.SearchServiceName, Constants.Index, new SearchCredentials(Constants.QueryApiKey));
           
            //BindingContext = new SearchViewModel2();
            BindingContext = this;
        }




        private async void OnClicked(object sender, EventArgs args)
        //private void OnClicked(object sender, EventArgs args)
        {
            string dialect = null;
            string age = null;
            string area = null;
            string costume = null;
            string wifi = null;
            string karaoke = null;

            if ((string)Dialectpicker.SelectedItem !=　"方言∇")
                dialect = (string)Dialectpicker.SelectedItem;
            if ((string)Agepicker.SelectedItem != "年齢∇")
                age = (string)Agepicker.SelectedItem;
            if ((string)Areapicker.SelectedItem != "地域∇")
                area = (string)Areapicker.SelectedItem;
            if ((string)Costumepicker.SelectedItem != "衣装∇")
                costume = (string)Costumepicker.SelectedItem;
            if (Wifiswitch.IsToggled == true)
                wifi = "あり";
            if (Karaokeswitch.IsToggled == true)
                karaoke = "あり";

            var locator = CrossGeolocator.Current;
            var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(10));
            double lat = position.Latitude;
            double lon = position.Longitude;

            await SearchViewModel2Async(lat:lat, lon:lon, q:freeword.Text, Dialect:dialect, Age:age, Area:area, Costume:costume, Wifi:wifi, Karaoke:karaoke);
            //         await SearchViewModel2Async(q: searchBar.Text, countyFacet: Dialect);

        }


        void OnListViewItemTapped(object sender, ItemTappedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }

        async void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var model = ((ListView)sender).SelectedItem as Models;
            if (model != null)
            {
                var page = new SnackDetailsPage(model.Address);
    //            page.BindingContext = model;
                await Navigation.PushAsync(page);
            }
        }


        async Task SearchViewModel2Async(double lat , double lon , string q = "", string Dialect = "", string Age = "", string Area = "", string Costume = "", string Wifi = "", string Karaoke = "")
//        async Task SearchViewModel2Async(string q = "", string countyFacet = "")
        //internal class SearchViewModel2Async(string q = "", string countyFacet = "")
        {
            if (string.IsNullOrWhiteSpace(q))
                q = "*";

             Modelss.Clear();
            

 //           DocumentSearchResult<Models> response = featureSearch.Search(q, countyFacet, countyFacet2);
            
            SearchParameters sp = new SearchParameters()
            {
                SearchMode = SearchMode.All,
                //Select row
                Select = new List<string>() { "ID", "Mamaname", "Snackname", "Address", "Geolocation" },
                OrderBy = new List<string>() { "geo.distance(Geolocation, geography'POINT(" + lon + " " + lat + ")') asc" },
            };
            // Filter each row
            string filter = null;

            if (Area != null)
            {
                filter = "Area eq '" + Area + "'";
            }
            else
            {
                filter = "Filter eq 1";            
            }

            if (Age != null)
            {
                filter += " and Age eq '" + Age + "'";
            }            
            if (Dialect != null)
            {
                filter += " and Dialect eq '" + Dialect + "'";
            }
            if (Costume != null)
            {
                filter += " and Costume eq '" + Costume + "'";
            }
            if (Wifi != null)
            {
                filter += " and Wifi eq '" + Wifi + "'";
            }
            if (Karaoke != null)
            {
                filter += " and Karaoke eq '" + Karaoke + "'";
            }

            if (filter != null)
            {
                sp.Filter = filter;
            }

           

            var response = await indexClient.Documents.SearchAsync<Models>(q, sp);
            foreach (SearchResult<Models> result in response.Results)
            {

                //Calculate distance
                double Lat = result.Document.Geolocation.Latitude;
                double Lon = result.Document.Geolocation.Longitude;
                double sLat1 = Math.Sin(Radians(lat));
                double sLat2 = Math.Sin(Radians(Lat));
                double cLat1 = Math.Cos(Radians(lat));
                double cLat2 = Math.Cos(Radians(Lat));
                double cLon = Math.Cos(Radians(lon) - Radians(Lon));
                double cosD = sLat1 * sLat2 + cLat1 * cLat2 * cLon;
                double d = Math.Acos(cosD);
                double dist = R * d;

                Modelss.Add(new Models
                {
                    ID = result.Document.ID,
                    Mamaname = result.Document.Mamaname,
                    Snackname = result.Document.Snackname,
                    Address = result.Document.Address,
                    Distance = dist.ToString(),

                    
                    //                    ImageUrl = result.Document.ImageUrl
//                    Tell = result.Document.Tell,
  //                  Area = result.Document.Area,
    //                Wifi = result.Document.Wifi,
      //              Karaoke = result.Document.Karaoke,
        //            Costume = result.Document.Costume,

                    //                    GeoLocation = result.Document.GeoLocation,
//                    Counter = result.Document.Counter,
  //                  Box = result.Document.Box,
    //                Birthplace = result.Document.Birthplace,
      //              Dialect = result.Document.Dialect,
        //            Age = result.Document.Age,
                    //Height = result.Document.Height,
//                    Shape = result.Document.Shape,
  //                  Time = result.Document.Time,
    //                Dayoff = result.Document.Dayoff,
      //              Labour = result.Document.Labour,
        //            Menu = result.Document.Menu,
          //          HP = result.Document.HP,
            //        PaymentSystem = result.Document.PaymentSystem,
              //      PaymentMethod = result.Document.PaymentMethod,
                //    Description = result.Document.Description,
                    //Description = result.Highlights.Description,
//                    Cost = result.Document.Cost,
  //                  Introduction = result.Document.Introduction,
    //                Coupon1 = result.Document.Coupon1,
      //              Coupon2 = result.Document.Coupon2,
        //            Coupon3 = result.Document.Coupon3,
          //          Reservation = result.Document.Reservation,            
                });
            }          
        }

        const double R = 6371; // km
        const double PIx = Math.PI;
        const double RADIO = 6378.16;

        private double Radians(double x)
        {
            return x * PIx / 180;
        }
    }
}
