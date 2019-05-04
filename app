using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly:XamlCompilation(XamlCompilationOptions.Compile)]
namespace mband
{
	public class App : Application
	{
        static FavoriteDatabase database;

        public App ()
		{
            // The root page of your application
            MainPage = new NavigationPage(new TabbedPage1())
            {
//               Children = {
//            
//                    new Search(),
//                    new Location(),
//                    new TodoList()
//                }
            };


        }

        //　DBを格納するためのローカル ファイル パスを返す
        public static FavoriteDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new FavoriteDatabase(
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mbandSQLite.db3"));
                }
                return database;
            }
        }

        public int ResumeAtTodoId { get; set; }

        protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
