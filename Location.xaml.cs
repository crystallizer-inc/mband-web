using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Spatial;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Maps;
using Plugin.Permissions;
using Plugin.Geolocator;
using Plugin.Permissions.Abstractions;
using System.Collections.ObjectModel;
using Microsoft.Azure.Search.Models;
using Microsoft.Azure.Search;
using MapKit;
using System.Text.RegularExpressions;
using System.Diagnostics;

using Android.Gms.Maps;

namespace mband
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Location : ContentPage
	{
        private Map map;
        private SearchBar searchBar;
        private readonly Geocoder geocoder = new Geocoder();
        private FeatureSearch featureSearch = new FeatureSearch();
        public ObservableCollection<Models> Modelss { get; private set; }
        static SearchIndexClient indexClient;
        private Button pinbutton;

        public event EventHandler Clicked;


        public Location()
        {
            InitializeComponent();
            Modelss = new ObservableCollection<Models>();
            BindingContext = this;
            indexClient = new SearchIndexClient(Constants.SearchServiceName, Constants.Index, new SearchCredentials(Constants.QueryApiKey));


            // 銀座駅をスタート地点に地図表示
            var map = new Map(MapSpan.FromCenterAndRadius(
                new Position(35.671728, 139.764443), Distance.FromMiles(0.3)))
            {
                IsShowingUser = true,
                HeightRequest = 100,
                WidthRequest = 960,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

           


            Task ret = CheckLocationPermissionStatusAsync(map);
            ret.Wait();




            pinbutton = new Button
            {
                Text = "現在地付近のお店を表示する",
                HeightRequest = 35,
                FontSize = 10,
                BackgroundColor = Color.LightGray,
                BorderColor = Color.Gray,
                BorderWidth = 7,
                BorderRadius = 20,
            };

            pinbutton.Clicked += async (object sender, EventArgs e) =>
            {
                await Button_Clicked();
                pinbutton.Text = "更新";
            };

            var stack = new StackLayout { Spacing = 0 };
            stack.Children.Add(pinbutton);
            stack.Children.Add(map);
            Content = stack;

        }

        private async Task Button_Clicked()
        {
            var locator = CrossGeolocator.Current;
            var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(10));
            var Lat = position.Latitude.ToString();
            var Lon = position.Longitude.ToString();

            // 現在地をスタート地点に地図表示
            var map = new Map(MapSpan.FromCenterAndRadius(
                new Position(position.Latitude, position.Longitude), Distance.FromMiles(0.3)))
            {
                IsShowingUser = true,
                HeightRequest = 100,
                WidthRequest = 960,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            
            var stack = new StackLayout { Spacing = 0 };
            stack.Children.Add(pinbutton);
            stack.Children.Add(map);
            Content = stack;

            // Define user location and distance might be navigated as kilometer?
            await GeoSearchAsync(lat: Lat, lon: Lon, distance: 100);
//            Regex re = new Regex(@"[^0-9]");
            for (int i = 0; i < 15; i++)
            {
                var pinn = new Pin
                {
                    Type = PinType.SearchResult,
                    Position = new Position(Modelss[i].Geolocation.Latitude, Modelss[i].Geolocation.Longitude),
                    //Position = new Position(35.681298, 139.766247),
                    Label = Modelss[i].Snackname,
                    Address = Modelss[i].Address,
                   
                };
               
                Device.BeginInvokeOnMainThread(() =>
                {
                // 同期（UIスレッド）
                map.Pins.Add(pinn);
                });


                pinn.Clicked += async (x, y) => {
                   // string id = re.Replace(pinn.Id.ToString(), "");
                    var page = new SnackDetailsPage(pinn.Address);
                    await Navigation.PushAsync(page);
                };



            }

           


        }

        //private void GeoSearch(string lat, string lon, int distance)
        async Task GeoSearchAsync(string lat, string lon, int distance)
        {
            //            var response = featureSearch.GeoSearch(lat, lon, distance);

            SearchParameters sp = new SearchParameters()
            {
                SearchMode = SearchMode.All,
                Select = new List<string>() { "ID", "Mamaname", "Snackname", "Address","Geolocation" },
                Top = 15,
                Filter = "geo.distance(Geolocation, geography'POINT(" + lon + " " + lat + ")') le " + distance.ToString(),
                //ScoringProfile = "Scoring-name",
                OrderBy = new List<string>() { "geo.distance(Geolocation, geography'POINT(" + lon + " " + lat + ")') asc"},
            };
            var response = await indexClient.Documents.SearchAsync<Models>("*", sp);
            //            var exchange = new JsonResult();
            //          var models = exchange.Deserialize(response.ToString());
            //        var latitude = models.Geolocation.Latitude;
            foreach (SearchResult<Models> result in response.Results)
            {
                Modelss.Add(new Models
                {
                    ID = result.Document.ID,
                    Mamaname = result.Document.Mamaname,
                    Snackname = result.Document.Snackname,
                    Address = result.Document.Address,
                    Geolocation = result.Document.Geolocation,
                });

               
            }

        }

        // iOS pin tapped method??
       // void OnDidSelectAnnotationView(object sender, MKAnnotationViewEventArgs e)
        //{
        //}

  
        

        //Android pin tapped event
 //       async void OnInfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
   //     {           
     //      var page = new SnackDetailsPage(Id.ToString());
       //    await Navigation.PushAsync(page);           
        //}


            private async Task CheckLocationPermissionStatusAsync(Map map)
        {
            // LocationのPermission状態を取得
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
            if (status != PermissionStatus.Granted)
            {
                // 許可されていなければユーザーに許可してもらうためにPermissionのリクエストを行う。
                status = (await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location))[Permission.Location];
            }
            if (status == PermissionStatus.Granted)
            {
                // 許可されたらマップ上の現在地を表示する。
                map.IsShowingUser = true;
            }
        }





        private async void OnSearchButtonPressed(object sender, EventArgs e)
        {
            // 検索バーから入力された地名から緯度・経度を取得する
            var positions = await geocoder.GetPositionsForAddressAsync(searchBar.Text);
            // 座標は複数とれる可能性があるが、今回は先頭の座標を利用する
            var position = positions.FirstOrDefault();
            // 座標が一つ以上とれていた場合のみ以下を処理する
            if (position != null)
            {
                // 該当地点へ移動する
                map.MoveToRegion(MapSpan.FromCenterAndRadius(                   
                        position,
                        Distance.FromMiles(0.2)));
                // 座標から住所を逆引きする
                var addresses = await geocoder.GetAddressesForPositionAsync(position);
                // 住所は複数とれる可能性があるが、今回は先頭の住所を利用する
                var address = addresses.FirstOrDefault();
                // 住所が一つ以上とれていた場合、以下を処理する
                if (address != null)
                {
                    // 以前設定したピンがあればピンを除去する
                    map.Pins.Clear();
                    // 新たにピンを作成し地図へ登録する
                    var pin = new Pin
                    {
                        Type = PinType.Place,               // ピンの形状                   
                        Position = position,                // ピンを登録する座標
                        Label = searchBar.Text,             // ピンのラベル。検索条件を設定
                        Address = address.Replace("\n", "") // ピンの住所。取得した住所の先頭が「日本\n～」となるので改行を除去する
                    };
                    map.Pins.Add(pin);
                }
            }
        }

      
    }
}
