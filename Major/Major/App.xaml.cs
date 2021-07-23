using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Major
{
    public partial class App : Application
    {
        static Database database;
        public bool bMultiplayer
        {
            get;set;
        }

        public static Database MainDatabase
        {
            
            get
            {
                if (database == null)
                    database = new Database(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mydb.db3"));
                return database;
            }
        }
        public App()
        {
            InitializeComponent();
             
            MainPage = new NavigationPage(new LoginScreen());
            //_camera 
        }
        
        protected override void OnStart()
        {
            
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
