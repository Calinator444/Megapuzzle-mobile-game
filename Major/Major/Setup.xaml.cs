using Plugin.Media;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Major
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Setup : ContentPage
    {
        
        public SKBitmap preview;
        readonly SKBitmap origin;
        SKBitmap saveMap;
        private bool multiplayer;
        string player;
       
        public Setup(SKBitmap myBit, bool multiplayer, string player)
        {
            InitializeComponent();
            this.player = player;
            this.multiplayer = multiplayer;
            preview = myBit;
            origin = imageDuplicate(preview);

            upload.IsVisible = multiplayer;
            uploadLabel.IsVisible = multiplayer;

            //halves the size of the image if it's too large,
            //drawing those preview lines repeatedly on a large image 
            if (preview.Width > 1000)
                   
                preview = resizedBit(myBit, myBit.Width / 2, myBit.Height / 2);

            updatePreview();
        }

        private void yCuts_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            updatePreview();
        }

        private SKBitmap resizedBit(SKBitmap bit, int newX, int newY)
        {
            SKRect canvasSize = new SKRect(0, 0, newX, newY);
            SKBitmap newBitmap = new SKBitmap(newX, newY);
            using (SKCanvas myCan = new SKCanvas(newBitmap))
            {
                myCan.DrawBitmap(bit, canvasSize);
            }
            return newBitmap;
        }

        //we create a duplicate by creating an empty bit map and drawing the image provided into it
        public SKBitmap imageDuplicate(SKBitmap dupSource)
        {
            SKBitmap duplicate = new SKBitmap(dupSource.Width, dupSource.Height);
            using (SKCanvas dupe = new SKCanvas(duplicate))
            {
                dupe.DrawBitmap(dupSource, new SKRect(0, 0, dupSource.Width, dupSource.Height));
            }
            return duplicate;
        }

        //creates a new preview image which shows how the image will look when it's cut
        public void updatePreview()
        {
            SKBitmap duplicate = imageDuplicate(preview);
            SKPaint myPaint = new SKPaint();
            
            myPaint.Style = SKPaintStyle.Stroke;
            myPaint.StrokeWidth = 5;
            myPaint.Color = SKColors.Blue;
            int ycuts = (int)yCuts.Value;
            int xcuts = (int)xCuts.Value;
            int tileHeight = duplicate.Height / ycuts;
            int tileWidth = duplicate.Width / xcuts;



            using (SKCanvas myCanvas = new SKCanvas(duplicate))
            {
                for (int x = 0; x <= xcuts; x++)
                {
                    for (int y = 0; y <= ycuts; y++)
                    {
                        SKRect myRect = new SKRect(x * tileWidth, y * tileHeight, tileWidth, tileHeight);
                        myCanvas.DrawRect(myRect, myPaint);
                    }
                }


            }
            imgPreview.Source = (SKBitmapImageSource)duplicate;
            saveMap = duplicate;


            //portrait images that are too long can break the game
            if(duplicate.Height > duplicate.Width)
                imgPreview.HeightRequest = 250;
            
            Console.WriteLine(string.Format($"xCuts:{xcuts} yCuts:{ycuts}, tileHeight:{tileHeight} tileWidth:{tileWidth}"));
            Console.WriteLine(string.Format($"ImageWidth:{duplicate.Width} ImageHeight:{duplicate.Height}"));

        }



        protected override void OnAppearing()
        {
            base.OnAppearing();

            //we need to build a list of items for the dropdown picker 

            numPicker.Items.Add("0.5");
            numPicker.Items.Add("1");
            numPicker.Items.Add("2");
            numPicker.Items.Add("3");
        }
        //=============================================
        // Reference A4: externally sourced algorithm
        // Purpose: Saving SKBitmaps to the users phone so that they can be reloaded later
        // Date: 30 Oct 2020
        // Source: Xamarin forms forums
        // Author: Mattelibow
        // url: https://forums.xamarin.com/discussion/75958/using-skiasharp-how-to-save-a-skbitmap
        // Adaptation required: adjusted the format so the image is saved as a jpeg
        // also broke the method of saving the image off into a function so it could be reused
        //=============================================
        public void saveFile(string dir, SKBitmap bitMap)
        {
            using (var image = SKImage.FromBitmap(bitMap))
            using (var data = image.Encode(SKEncodedImageFormat.Jpeg, 100))
            //Im using the autoincrement here to name the files automatically
            using (var stream = File.OpenWrite(dir))
            {
                data.SaveTo(stream);

            }
        }
        //=============================================
        // End reference A4
        //=============================================

        private void Button_Clicked(object sender, EventArgs e)
        {
            //throw new NotImplementedException();

            if (upload.IsChecked)

            {

                Console.WriteLine("Upload was checked");

                //solution from here
                
                //CrossMedia.Current.p
                Puzzle newPuzzle = new Puzzle();

                //casting here to remove the mantissa from xCuts and yCuts
                //we need to do this because the slider value is stored as a double
                newPuzzle.xPieces = (int) xCuts.Value;
                newPuzzle.yPieces = (int)yCuts.Value;
                newPuzzle.author = player;
                newPuzzle.hardMode = HardMode.IsChecked;
                newPuzzle.shuffInterval = Convert.ToDouble(numPicker.SelectedItem);



                //Puzzle.ID isn't autoincremented until the puzzle is inserted into the database
                //for what ever reason I can access the new ID without pulling it from the database
                App.MainDatabase.insterPuzzle(newPuzzle);



                
                string thumbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"thumb{newPuzzle.ID}.jpg");
                string imgPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"Image{newPuzzle.ID}.jpg");
                //DisplayAlert(filePath, "New path", "ok");

                //if we're going to replay the puzzle after reloading the app we need to save the source image
                //to device storage
                saveFile(thumbPath, saveMap);
                saveFile(imgPath, origin);

                App.MainDatabase.assignUrl(newPuzzle.ID, thumbPath, imgPath);
                Navigation.PushAsync(new MainGame(origin, (int)xCuts.Value, (int)yCuts.Value, HardMode.IsChecked, 
                    Convert.ToDouble(numPicker.SelectedItem), newPuzzle.ID,player,multiplayer));
            }
            //if the player chooses not to upload their puzzle the game acts as if the player is playing singleplayer
            else
                Navigation.PushAsync(new MainGame(origin, (int)xCuts.Value, (int)yCuts.Value, HardMode.IsChecked,
                    Convert.ToDouble(numPicker.SelectedItem), 0, player,false));



        }

        private void HardMode_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            //I dont want players trying to enable hard mode without a shuffle interval selected
            numPicker.SelectedIndex = 0;
        }
    }
}