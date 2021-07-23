using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Major
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateAccount : ContentPage
    {
        public CreateAccount()
        {
            InitializeComponent();
            
        }

        private void Entries_Clicked(object sender, EventArgs e)
        {
            App.MainDatabase.getUser();
        }


        private async void AddAccount_Clicked(object sender, EventArgs e)
        {
            Account newAccount = new Account();
            //remove the end spaces from the account details entered before attempting to query
            //the database
            newAccount.Username = Username.Text.Trim();
            newAccount.Password = Password.Text.Trim();

            //database insert method returns false when it fails to create an account
            bool accountAdded = App.MainDatabase.insertThing(newAccount);
            if (accountAdded)
            {
                await DisplayAlert("Success!", "Account creation was successful!", "Ok");
                await Navigation.PopAsync();
            }
            else
                await DisplayAlert("Error", "An account with that username already exists", "Ok");
        }

        private void DeleteAccounts_Clicked(object sender, EventArgs e)
        {
            App.MainDatabase.deleteAccounts(Username.Text);
        }
    }
}