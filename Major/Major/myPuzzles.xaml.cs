using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Major
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class myPuzzles : ContentPage
    {
        string player;
        public myPuzzles(string player)
        {
            InitializeComponent();
            this.player = player;
            Carousel.ItemsSource = App.MainDatabase.getMyPuzzles(this.player);
            if (App.MainDatabase.getMyPuzzles(this.player).Count < 1)
                noPuzz.IsVisible = true;
        }

        
        private async void Button_Clicked(object sender, EventArgs e)
        {
            bool confirmDelete = await DisplayAlert("Confirm", "Are you sure you want to delete this item?", "yes", "no");

            if (confirmDelete)
            {
                var puzzle = (Puzzle)Carousel.CurrentItem;
                App.MainDatabase.deletePuzzle(puzzle);
                Navigation.PopAsync();
            }

        }

        
    }
}