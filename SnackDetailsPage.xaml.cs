using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace mband
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SnackDetailsPage : ContentPage, INotifyPropertyChanged

    {
        static SearchIndexClient indexClient;
        public ObservableCollection<Specific> SPECIFIC { get; private set; }


        public SnackDetailsPage (string Address)
		{
            InitializeComponent();
            SPECIFIC = new ObservableCollection<Specific>();
            indexClient = new SearchIndexClient(Constants.SearchServiceName, Constants.Index, new SearchCredentials(Constants.QueryApiKey));
            BindingContext = this;

            SPECIFIC.Clear();
            SearchParameters sp = new SearchParameters()
            {
                SearchMode = SearchMode.All,
                
            };
            sp.Filter = "Address eq '" + Address + "'";

            var response = indexClient.Documents.Search<Specific>("*", sp);
            SearchResult<Specific> result = response.Results[0];           
          
            //         foreach (SearchResult<Specific> result in response.Results)            
            //       {
            //SPECIFIC.Add(new Specific
            //{
            var aa = new Specific { 
                    Mamaname = result.Document.Mamaname,
                    Snackname = result.Document.Snackname,
                    Address = result.Document.Address,
            //                   ImageUrl = result.Document.ImageUrl
                    Tell = result.Document.Tell,
                    Area = result.Document.Area,
                    Wifi = result.Document.Wifi,
                    Karaoke = result.Document.Karaoke,
                    Costume = result.Document.Costume,
                                                   //                    GeoLocation = result.Document.GeoLocation,
                    Counter = result.Document.Counter,
                    Box = result.Document.Box,
                    Birthplace = result.Document.Birthplace,
                    Dialect = result.Document.Dialect,
                    Age = result.Document.Age,
                    //Height = result.Document.Height,
                    Shape = result.Document.Shape,
                    Time = result.Document.Time,
                    Dayoff = result.Document.Dayoff,
                    Labour = result.Document.Labour,
                    Menu = result.Document.Menu,
                    HP = result.Document.HP,
                    PaymentSystem = result.Document.PaymentSystem,
                    PaymentMethod = result.Document.PaymentMethod,
                    Description = result.Document.Description,
                    Cost = result.Document.Cost,
                    Introduction = result.Document.Introduction,
                    Coupon1 = result.Document.Coupon1,
                    Coupon2 = result.Document.Coupon2,
                    Coupon3 = result.Document.Coupon3,
                    Reservation = result.Document.Reservation,            
            //    });
            };
            //      var obj = JsonConvert.DeserializeObject<Specific>(aa);

            mamaname.Text = aa.Mamaname;
            snackname.Text = aa.Snackname;
            address.Text = aa.Address;
            introduction.Text = aa.Introduction;
            paymentsystem.Text = aa.PaymentSystem;
            paymentmethod.Text = aa.PaymentMethod;
            number.Text = aa.Tell.ToString();
            url.Text = aa.Reservation;

            var images = new List<string>
            {
                "https://pbs.twimg.com/profile_images/493938489067446272/_w64wQ6H.jpeg",
                "https://pbs.twimg.com/profile_images/493938489067446272/_w64wQ6H.jpeg",
                "https://pbs.twimg.com/profile_images/493938489067446272/_w64wQ6H.jpeg",
                "https://pbs.twimg.com/profile_images/493938489067446272/_w64wQ6H.jpeg",
            };
            


            var tapGesture1 = new TapGestureRecognizer();
            tapGesture1.Tapped += (s, e) => OnCall(number.Text);
            // attache the gesture to your label
            number.GestureRecognizers.Add(tapGesture1);

            var tapGesture2 = new TapGestureRecognizer();
            tapGesture2.Tapped += (s, e) => OnWebBrowse();
            // attache the gesture to your label
            url.GestureRecognizers.Add(tapGesture2);

        }
     

        private async void OnCall(object sender)
        {
            if (await this.DisplayAlert(
                    number.Text,
                    snackname.Text + "に電話をかけますか?",
                    "はい",
                    "いいえ"))
            {
                var dialer = DependencyService.Get<IDialer>();
                if (dialer != null)
                {
                  
                    dialer.Dial(number.Text);
                }

            }
        }


        private void OnWebBrowse()
        {
            Uri uri = new Uri(url.Text);
            DependencyService.Get<IWebBrowserService>().Open(uri);
        }

        public interface IWebBrowserService
        {
            void Open(Uri uri);
        }


        async void OnFavClicked(object sender, EventArgs e)
        {
            SearchParameters sp = new SearchParameters()
            {
                SearchMode = SearchMode.All,

            };
            sp.Filter = "Address eq '" + address.Text + "'";

            var response = indexClient.Documents.Search<favorite>("*", sp);
            SearchResult<favorite> result = response.Results[0];

            var bb = new favorite
            {
                ID = result.Document.ID,
                Mamaname = result.Document.Mamaname,
                Snackname = result.Document.Snackname,
                Address = result.Document.Address,
            };

                //var fav = (favorite)result.Document;
                //var favo = (favorite)SPECIFIC;
            await App.Database.SaveItemAsync(bb);
            



            Fav.Text = "お気に入り登録済";
            Fav.TextColor = Color.LightGray;
            Fav.BackgroundColor = Color.LightYellow;

            // await Navigation.PopAsync();
            //この後に登録しましたのポップ

        }


    }

   
}
