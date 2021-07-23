using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Xamarin.Forms;

namespace Major
{


    //this class acts as the Data Access Layer between the database and the main program
    public class Database
    {
        private SQLiteConnection myConnection;
        public Database(string path)
        {
            //creates the database if none exists
            myConnection = new SQLiteConnection(path);
            myConnection.CreateTable<Account>();
            myConnection.CreateTable<Puzzle>();
        }
        public void getUser()
        {
            var myTable = myConnection.Table<Account>();
            foreach (var s in myTable)
                Console.WriteLine($"User {s.Username} was found with ID{s.ID} and Password {s.Password}");
        }
        public bool insertThing(Account givenAcc)
        {
            //first we make sure that no entries with that name already exist
            Account myAccount = myConnection.Table<Account>().Where(i => i.Username == givenAcc.Username).FirstOrDefault();
            if (myAccount == null)
            {
                myConnection.Insert(givenAcc);
                return true;
            }
            else
                return false;
        }


        public string checkNewRecord(int pzzleId, string newRecordHolder, TimeSpan newRecord)
        {
            Puzzle currentRec = myConnection.Table<Puzzle>().Where(i => i.ID == pzzleId).FirstOrDefault();
            if (currentRec.recordHolder != null)
            {
                if (currentRec.recordTime > newRecord && newRecordHolder != currentRec.recordHolder)
                {
                    currentRec.recordTime = newRecord;
                    currentRec.recordHolder = newRecordHolder;
                    myConnection.Update(currentRec);
                    return "Congraturlations! You've set the new record for solving this puzzle";
                }
                else
                {
                    currentRec.recordTime = newRecord;
                    myConnection.Update(currentRec);
                    return "Congratulations! You beat your previous record";
                }
            }
            else
            {
                currentRec.recordTime = newRecord;
                currentRec.recordHolder = newRecordHolder;
                myConnection.Update(currentRec);
                return "This puzzle has not been completed until now";
            }
            
        }
        public void insterPuzzle(Puzzle puzzle)
        {
            myConnection.Insert(puzzle);
            
            //myConnection.Update(puzzle)
        }

        public void assignUrl(int id, string thumbPath, string mainPath)
        {
            Puzzle puzz = myConnection.Table<Puzzle>().Where(i => i.ID == id).FirstOrDefault();
            puzz.thumbnail = thumbPath;
            puzz.link = mainPath;

            //as far as I know, as long as the primary key of the object being inserted and the
            //object within the database match, the object within the database will be overwritten
            myConnection.Update(puzz);
        }
        
        public void deleteAccounts(String matching)
        {
            Account myAccount = myConnection.Table<Account>().Where(i => i.Username == matching).FirstOrDefault();
            myConnection.Delete(myAccount);
        }

        public void deletePuzzle(Puzzle puzzle)
        {
        
            myConnection.Delete(puzzle);
        }
        public Account returnUser(String username)
        {
            Account usr = myConnection.Table<Account>().Where(i => i.Username == username).FirstOrDefault();
            return usr;
        }
        public List<Puzzle> getPuzzles()
        {
            return myConnection.Table<Puzzle>().ToList();
        }
        public List<Puzzle> getMyPuzzles(string user)
        {
            return myConnection.Table<Puzzle>().Where(i => i.author == user).ToList();
        }

    }
}
