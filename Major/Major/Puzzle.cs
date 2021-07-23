using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Major
{
    public class Puzzle
    {
        [PrimaryKey, AutoIncrement]
        public int ID {get;set;}
        public int xPieces { get; set; }
        public int yPieces { get; set; }
        public TimeSpan recordTime { get; set; }
        public string recordHolder { get; set; }
        public string author { get; set; }
        public string link { get; set; }
        public string thumbnail { get; set; }

        public bool hardMode { get; set; }
        public double shuffInterval { get; set; }
        
    }
}
