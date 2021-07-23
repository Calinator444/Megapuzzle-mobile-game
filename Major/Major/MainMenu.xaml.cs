using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Major
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainMenu : ContentPage
    {
        private bool multiplayer;
        string player;
        public MainMenu(bool bMultiplayer, string currentPlayer)
        {
            InitializeComponent();
            multiplayer = bMultiplayer;
            player = currentPlayer;            
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreatePuzzle(multiplayer,player));
        }

        private void PuzzleBrowser(object sender, EventArgs e)
        {
            if(!multiplayer)
            {
                DisplayAlert("Error", "You must be online to use this feature", "Ok");
            }

            else
                Navigation.PushAsync(new PuzzlePicker(player));
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            if (!multiplayer)
            {
                DisplayAlert("Error", "You must be online to use this feature", "Ok");
            }

            else
                Navigation.PushAsync(new myPuzzles(player));
        }
    }
}