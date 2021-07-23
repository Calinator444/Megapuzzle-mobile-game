

//This program was based heavily on the following 2 code samples
//skiasharp demo files https://docs.microsoft.com/en-us/samples/xamarin/xamarin-forms-samples/skiasharpforms-demos/
//"Xamagon Xuzzle" sample from the following textbook https://www.programmersought.com/article/48011024102/
/*
    
    The original sample was a simple sliding block puzzle, but I didn't want to copy this exactly
    Instead the user solves the puzzle by clicking consecutive puzzle pieces, which results in those
    two pieces being swapped around in the puzzle.


 
 */
using Xamarin.Essentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Markup;
using Xamarin.Forms.Xaml;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Major
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainGame : ContentPage
    {
        Stopwatch myStop;
        Tile myTile;
        double aspectRatio;
        
        //each new puzzle created will be pushed to an array
        //this way the tiles can be referred to by their location within the array
        Tile[,] tileArray;
        int[,] winArray;
        //stores the X,Y coords of the highlighted tile and a third int used as a bool which determines whether a tile is highlighted
        int[] highlighted;
        //Tile[,] winnerArray;

        bool playing;
        int xPieces, yPieces;
        SKBitmap mainImage;
        bool hardEnabled;
        //TimeSpan shufflePeriod;
        double dElapsedSecs;
        int iElapsedSecs;
        Random rand;
        bool multiplayer;
        int puzzleId;
        string player;

        public MainGame(SKBitmap skbit, int xCuts, int yCuts, bool hardMode, double hardInterval, int puzzleId, string player, bool multiPlay)
        {
            InitializeComponent();
            if (skbit.Height > skbit.Width)
                absoluteLayout.HorizontalOptions = LayoutOptions.CenterAndExpand;
            NavigationPage.SetHasBackButton(this, false);
            hardEnabled = hardMode;
            this.puzzleId = puzzleId;
            this.player = player;
            //we use a double here otherwise the program will convert 0.5 to zero
            dElapsedSecs = hardInterval * 60;
            iElapsedSecs = (int) dElapsedSecs;
            rand = new Random();
            multiplayer = multiPlay;
            mainImage = skbit;
            yPieces = yCuts;
            xPieces = xCuts;
            myStop = new Stopwatch();

            //who can say where the road goes
            //where the day flows
            //only time
            
            myStop = new Stopwatch(); 
            playing = false;
            highlighted = new int[3];
            tileArray = new Tile[xCuts + 1, yCuts + 1];
            winArray = new int[xCuts + 1, yCuts + 1];
            fillFrame(mainImage);

            //start the accelerometer when the main game is instantiated
            if (!Accelerometer.IsMonitoring)
                Accelerometer.Start(SensorSpeed.UI);
            Accelerometer.ShakeDetected += Accelerometer_ShakeDetected;
           // Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;

        }

        private void Accelerometer_ShakeDetected(object sender, EventArgs e)
        {
            shuffle();
        }


        //a little confusing here, but we have a timer that updates the form every second and a stop watch which
        //keps track of how much time has elapsed
        public void startTimer()
        {            
            myStop.Start();
            Device.StartTimer(new TimeSpan(0,0,1), () =>//updates the form every second
            {

                Device.BeginInvokeOnMainThread(() =>
                {
                    StartGame.Text = String.Format("{0:D2}:{1:D2}", myStop.Elapsed.Minutes , myStop.Elapsed.Seconds);
                    int getSecs = (int)myStop.Elapsed.TotalSeconds;
                    if (getSecs % dElapsedSecs == 0 && hardEnabled)
                        shuffle();
                    else Console.WriteLine($"{getSecs} % {dElapsedSecs} = {getSecs % dElapsedSecs}");
                });
                if (playing)
                    return true;
                else
                    return false;
                    
            });
        }
        //=============================================
        // Reference A2: externally sourced code
        // Purpose: Create the puzzle within an absolute layout object
        // Date: 08 Oct 2020
        // Source: Microsoft Documentation
        // Author: unknown
        // url: https://docs.microsoft.com/en-us/samples/xamarin/xamarin-forms-samples/skiasharpforms-demos/
        // Adaptation required: adapted the code so that it creates my Tile objects with Row attributes that are useful later
        // code now creates tiles with variable aspect ratios, not just 1:1
        //=============================================
        public void fillFrame(SKBitmap mybit)
        {
            //absoluteLayout.HeightRequest = 60;
            int incId = 0;
            for (int row = 0; row < yPieces; row++)
            {
                for (int col = 0; col < xPieces; col++)
                {
                    int custWidth = mybit.Width / xPieces;
                    int custHeight = mybit.Height / yPieces;

                    SKBitmap halfbit = new SKBitmap(custWidth, custHeight, false);
                    int xStart = custWidth * col;
                    int yStart = custHeight * row;
                    aspectRatio = (double)custWidth / custHeight;
                    int xEnd = xStart + custWidth;
                    int yEnd = yStart + custHeight;
                    SKRect src = new SKRect(xStart, yStart, xEnd, yEnd);
                    SKRect dest = new SKRect(0, 0, custWidth, custHeight);

                    using (SKCanvas croppedMap = new SKCanvas(halfbit))
                    {
                        croppedMap.DrawBitmap(mybit, src, dest);
                    }

                    //}
                    myTile = new Tile(row, col, (SKBitmapImageSource)halfbit, custWidth, custHeight);
                    myTile.id = incId;
                    incId += 1;
                    //Console.WriteLine($"My file: {()halfbit}");

                    int tilePad = 2;
                    //myTile.WidthRequest = custWidth;
                    myTile.Padding = new Thickness(tilePad, tilePad, tilePad, tilePad);
                    myTile.BackgroundColor = Color.Black;


                    TapGestureRecognizer tapGesture = new TapGestureRecognizer
                    {
                        Command = new Command(OnTapped),
                        CommandParameter = myTile,
                    };

                    myTile.GestureRecognizers.Add(tapGesture);
                    myTile.Row = row;
                    myTile.Col = col;

                    tileArray[col, row] = myTile;
                    tileArray[col, row].WidthRequest = absoluteLayout.Width / xPieces;
                    absoluteLayout.Children.Add(tileArray[col, row]);
                    
                  
                }
            }
            //=============================================
            // End reference A2
            //=============================================
            winArray = generateWinner();
            

        }
        private void shuffle()
        {

            //To prevent bugs the program removes the program un-highlights the selected
            //tile before shuffling


            //a little vibration that lets the user know the screen has been shuffled
            Vibration.Vibrate(500);
            
            tileArray[highlighted[0], highlighted[1]].BackgroundColor = Color.Black;
            highlighted[2] = 0;

            for (int i = 0; i < 7; i++)
            {
                int xTile1 = rand.Next(xPieces);
                int yTile1 = rand.Next(yPieces);
                int xTile2 = rand.Next(xPieces);
                int yTile2 = rand.Next(yPieces);
                while (xTile1 == xTile2 && yTile1 == yTile2)
                {
                    Console.WriteLine("shuffle attempted to move 2 tiles of the same coordinates");
                    xTile1 = rand.Next(xPieces);
                    yTile1 = rand.Next(yPieces);
                }
                Tile Tile1 = tileArray[xTile1, yTile1];
                double Width = tileArray[xTile1, yTile1].Width;//Tile1.Width;
                double Height = tileArray[xTile1, yTile1].Height;
                Tile Tile2 = tileArray[xTile2, yTile2];
                tileArray[xTile1, yTile1].LayoutTo(new Rectangle(Tile2.Col * Width, Tile2.Row * Height, Width, Height));
                tileArray[xTile2, yTile2].LayoutTo(new Rectangle(Tile1.Col * Width, Tile1.Row * Height, Width, Height));



                //I hate to do it this way but I cant cast either of these as ref paremeters so I cant use a function for this

                //swap the "Row" attributes of each tile
                int Temp = Tile1.Row;
                Tile1.Row = Tile2.Row;
                Tile2.Row = Temp;

                
                
                //swap the "Col" attributes of each tile
                Temp = Tile1.Col;
                Tile1.Col = Tile2.Col;
                Tile2.Col = Temp;


                //swap the tile positions n the array
                Tile temp = tileArray[xTile1, yTile1];

                //We can't use the Tile variables because they refer to the wrong memory location
                tileArray[xTile1, yTile1] = tileArray[xTile2, yTile2];
                tileArray[xTile2, yTile2] = temp;

            }
        }

        //checks the id of each puzzle piece in the main array to see if the puzzle is in the correct order
        private bool checkWin()
        {
            bool bWon = true;
            for (int row = 0; row < yPieces;row++)
            {
                for (int col = 0; col < xPieces;col++)
                {

                    if (winArray[col, row] != tileArray[col, row].id)
                    {
                        Console.WriteLine($"unmatching tiles at ROW:{col} COL:{row}");
                        bWon = false;
                    }
                }
            }
            return bWon;
        }


        //an array containing the id for all of the puzzle pieces in their correct order
        int[,] generateWinner()
        {
            int[,] winner = new int[xPieces + 1, yPieces + 1];
            for (int row = 0; row < yPieces;row++)
            {

                for (int col = 0;col < xPieces;col++)
                {
                    winner[col, row] = tileArray[col, row].id;
                }
            }
            return winner;
        }
       
        async void OnTapped(object sender)
        {
            
            //cast the sending object as a tile so we can get it's attributes
            Tile thisTile = (Tile)sender;

            if (highlighted[2] > 0)
            {
                int[] prev = new int[2];
                tileArray[highlighted[0], highlighted[1]].BackgroundColor = Color.Black;


                //we haven't swapped the items in the array yet, we've only moved their position in the puzzle
                tileArray[highlighted[0], highlighted[1]].LayoutTo(new Rectangle(thisTile.Col * thisTile.Width, thisTile.Row * thisTile.Height, thisTile.Width, thisTile.Height));

                //this task is awaited so that the program doesn't check for a win before the tiles are switched
                await thisTile.LayoutTo(new Rectangle(highlighted[0] * thisTile.Width, highlighted[1] * thisTile.Height, thisTile.Width, thisTile.Height)); 
                highlighted[2] = 0;

                

                tileArray[highlighted[0], highlighted[1]].Row = thisTile.Row;
                tileArray[highlighted[0], highlighted[1]].Col = thisTile.Col;

                int prevCol = thisTile.Col;
                int prevRow = thisTile.Row;
                thisTile.Col = highlighted[0];
                thisTile.Row = highlighted[1];

                Tile temp = tileArray[highlighted[0], highlighted[1]];
                
                tileArray[highlighted[0], highlighted[1]] = tileArray[prevCol, prevRow];
                tileArray[prevCol, prevRow] = temp;

                if (playing && checkWin())
                {
                    
                    TimeSpan winTime = new TimeSpan(0,myStop.Elapsed.Minutes,myStop.Elapsed.Seconds);
                    string winnerMessage = "Well done! You finished";
                    Console.WriteLine($"{ winTime}");


                    //the database will return a string depending on whether they completed the puzzle 
                    if (multiplayer)
                        winnerMessage = App.MainDatabase.checkNewRecord(puzzleId, player, winTime);
                 
                    myStop.Stop();
                    Console.WriteLine($"{ winTime}");
                    playing = false;
                   
                    Navigation.PushAsync(new WinnerPage(winTime, winnerMessage));
           
                }
                
            }
            else
            {
                highlighted[0] = thisTile.Col;
                highlighted[1] = thisTile.Row;
                thisTile.BackgroundColor = Color.Blue;
                highlighted[2] = 1;
                
            }
        }

            private async void Start_playing(object sender, EventArgs e)
            {
                shuffle();
                playing = true;
                startTimer();
                StartGame.IsEnabled = false;
            }


        //using the back button doesn't actually quit the game 
        //so I needed to remove it and create a "quit" button that handles all that
        private void quit_Clicked(object sender, EventArgs e)
        {
            playing = false;
            Navigation.PopToRootAsync();
        }



        //adding each of the tiles will resize the absoluteLayout
        //using this event handler we can finish off by placing each tile in its correct location
        private void absoluteLayout_SizeChanged(object sender, EventArgs e)
        {
            //if (mainImage.Height > mainImage.Width)
            //{
                Console.WriteLine("Width is greater than height"); 
                //absoluteLayout.HeightRequest = 10;
            //}
            foreach (Tile t in absoluteLayout.Children)
            {

                //modded
                int tileSize, tileHeight;
                if (mainImage.Height > mainImage.Width)
                {
                    tileHeight = (int)500 / yPieces;
                    tileSize = (int)(tileHeight * aspectRatio);
                }
                else
                {
                    tileSize = (int)absoluteLayout.Width / xPieces;
                    tileHeight = (int)(tileSize / aspectRatio);
                }
                Console.WriteLine($"AspectRatio:{aspectRatio}");
                
                int xStart = t.Col * tileSize;
                int yStart = t.Row * tileHeight;
                
                Console.WriteLine($"MOVING TILE TO X:{xStart} Y:{yStart} WIDTH:{tileSize} HEIGHT:{tileHeight}");
                AbsoluteLayout.SetLayoutBounds(t, new Rectangle(xStart, yStart, tileSize,tileHeight));
            }
        }
    }
    }
