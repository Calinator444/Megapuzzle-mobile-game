using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;


//I used this code sample for some pointers for this page (like the idea of getting the Puzzle table as a list)

//https://docs.microsoft.com/en-us/samples/xamarin/xamarin-forms-samples/getstarted-notes-database/


using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Major
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PuzzlePicker : ContentPage
    {
        string player;
        public PuzzlePicker(string currentPlayer)
        {
            InitializeComponent();
            player = currentPlayer;
            
            
            
            
        }
        
        protected override void OnAppearing()
        {
            
            //We set up the itemsource by getting the Puzzle table from the main database
            Console.WriteLine("OnAppearing was called");
            base.OnAppearing();
            Carousel.ItemsSource = App.MainDatabase.getPuzzles();
            noPuzz.IsVisible = App.MainDatabase.getPuzzles().Count < 1;
            
            
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            //puzzle set to null?
            var deletable = (Puzzle)Carousel.CurrentItem;
            App.MainDatabase.deletePuzzle(deletable);
            //Puzzle thing = (Puzzle)sender.par;
            //sender.Parent
            //DisplayAlert($"{thing.ID}","Ok","kid");
            //Navigation.PopAsync();
        }

        private void Play_Clicked(object sender, EventArgs e)
        {
            var puzzle = (Puzzle)Carousel.CurrentItem;

            SKBitmap myBit;

            //=============================================
            // Reference A1: externally sourced algorithm
            // Purpose: converting a file path to an SKBIT object
            // Date: 30 Oct 2020
            // Source: Xamarin forum post
            // Author: user Kevinvadenhoek
            // url: https://forums.xamarin.com/discussion/102129/creating-skimage-from-resources-within-forms
            // Adaptation required: adapted the method 
            //=============================================


            //We need to get the image as a stream object
            //which we then decode into an SKBitmap
            
            using (Stream fileStream = File.OpenRead(puzzle.link))
            {
                myBit = SKBitmap.Decode(fileStream);
            }
            //=============================================
            // End reference A1
            //=============================================

            //we just parse multiplayer as "true" here because if the player is accessing the puzzle picker 
            //they're definitely playing multiplayer
            Navigation.PushAsync(new MainGame(myBit, puzzle.xPieces, 
                puzzle.yPieces, puzzle.hardMode, puzzle.shuffInterval,puzzle.ID,player,true));
        }

    }
}