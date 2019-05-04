using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace mband.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Favorite : ContentPage
	{
    
        public Favorite ()
		{
			InitializeComponent ();           
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Reset the 'resume' id, since we just want to re-start here
            ((App)App.Current).ResumeAtTodoId = -1;
            favorites.ItemsSource = await App.Database.GetItemsAsync();
        }


        void OnListViewItemTapped(object sender, ItemTappedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }

        async void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var fav = ((ListView)sender).SelectedItem as favorite;
            if (fav != null)
            {
                var page = new SnackDetailsPage(fav.Address);
                //            page.BindingContext = model;
                await Navigation.PushAsync(page);
            }
        }

    }
}
