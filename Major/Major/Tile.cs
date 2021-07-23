using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Major
{

    //Were using inheritance here 
    //The "Tile" class is a specialized ContentView Class with some new attributes
    public class Tile:ContentView
    {
        public Tile(int row, int col, ImageSource src, int width, int height)
        {
            Row = row;
            Col = col;
            TileWidth = width;
            TileHeight = height;
            Image myImage = new Image { Source = src };
            Content = myImage;

        }
        
        public int Row
        {
            get;
            set;
        }
        public int Col
        { 
            get;
            set;
        }
        //we need a method of memorizing the tile's original position
        public int id
        {
            get;
            set;
        }
        public int TileWidth
        {

            get;
            set;
        }
        public int TileHeight
        {
            get;
            set;
        }

    }
}
