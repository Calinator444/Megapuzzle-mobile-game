using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Media;
using SkiaSharp;
using Plugin.Media.Abstractions;

namespace Major
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreatePuzzle : ContentPage
    {
        private bool bMultiplayer;
        string player;
        public CreatePuzzle(bool isMultiplr, string currentPlayer)
        {
            InitializeComponent();
            //App.mul
            bMultiplayer = isMultiplr;
            player = currentPlayer;
        }


        
        private async void PickPhoto(object sender, EventArgs e)
        {
            try
            {
                //exits the loop if the phone doesn't support photo picker
                //if (!CrossMedia.Current.IsPickPhotoSupported)
                //    return;
                await CrossMedia.Current.Initialize();
                var myPhoto = await CrossMedia.Current.PickPhotoAsync();
                var myStream = myPhoto.GetStream();
                SKBitmap newbit = SKBitmap.Decode(myStream);
                await Navigation.PushAsync(new Setup(newbit, bMultiplayer,player));
             
            }
            //if you're gonna crash do it gracefully
            catch
            {
                await DisplayAlert("Error", "Failed to retrieve image", "ok");
            }
        }

        //=============================================
        // Reference A5: externally sourced algorithm
        // Purpose: Initializing the media plugin 
        // Date: 15 sept 2020
        // Source: Github
        // Author: James Montenmango
        // url: https://github.com/jamesmontemagno/MediaPlugin
        // Adaptation required: The image created is converted to an SKBitmap so that it can be drawn on and modified
        // a try/catch loop was also added to prevent the program from crashing if the camera was unavailable or didn't return a photo
        //=============================================
        private async void TakePhoto(object sender, EventArgs e)
        {
            try
            {
                await CrossMedia.Current.Initialize();
                var myFile = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    Directory = "Sample",
                    Name = "test.jpg"
                });
                var fileStream = myFile.GetStream();
                SKBitmap mybit = SKBitmap.Decode(fileStream);
                await Navigation.PushAsync(new Setup(mybit, bMultiplayer, player));
            }
            catch
            {
                await DisplayAlert("Error", "Could not access camera", "ok");
            }
            //=============================================
            // End reference A5
            //=============================================
        }
    }
}