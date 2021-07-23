using Plugin.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Major
{
    public partial class LoginScreen : ContentPage
    {
        public LoginScreen()
        {
            InitializeComponent();
            
             
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CrossMedia.Current.Initialize();
        }


        private async void Create_Account(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreateAccount());
        }


        private void Login_Clicked(object sender, EventArgs e)
        {

            //We use an inclusive or operator here so that the query does not go through unless both
            //the username and password have been entered
            
            if (Username.Text == null | Password.Text == null)
                DisplayAlert("Error", "Please enter your username and password", "Ok");

            else
            {
                //We use the Trim() feature here in case the user accidentally leaves empty spaces at the start or end of the details they entered
                Account loginAttempt = App.MainDatabase.returnUser(Username.Text.Trim());
                if (loginAttempt != null)
                {
                    //Console.WriteLine("User was found");
                    if (loginAttempt.Password == Password.Text.Trim())
                    {
                        Navigation.PushAsync(new MainMenu(true, Username.Text.Trim()));
                        Console.WriteLine("Password matches");
                        //App.bMultiplayer = true;
                    }
                    else
                        DisplayAlert("Error", "The password you entered was incorrect", "Ok");
                }
                else
                    DisplayAlert("Error", "An account with that username does not exist", "Ok");
            }
            

        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            //passes an empty string if singleplayer is enabled
            await Navigation.PushAsync(new MainMenu(false,""));
        }
    }
}
