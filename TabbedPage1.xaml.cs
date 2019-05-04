using mband.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace mband
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabbedPage1 : TabbedPage
    { 


        public TabbedPage1 ()
        {

            var Search = new Search()
            {
                Title = "検索",
                Icon = "icon.png"
            };

            var Location = new Location()
            {
               Title = "地図",
                Icon = "icon.png"
            };

            var favorite = new Favorite()
            {
                Title = "お気に入り",
                Icon = "icon.png"
            };

            InitializeComponent();
            Children.Add(Search);
            Children.Add(Location);
            Children.Add(favorite);

        }
    }
}
