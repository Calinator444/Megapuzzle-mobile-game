using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Major
{
    //=============================================
    // Reference A3: externally sourced code
    // Purpose: using classes as unique tupels in a table
    // Date: 29 Oct 2020
    // Source: Microsoft Docs
    // Author: Unknown
    // url: https://docs.microsoft.com/en-us/samples/xamarin/xamarin-forms-samples/getstarted-notes-database/
    // Adaptation required: Creating my own data objects with different data types
    //=============================================
    public class Account
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

    }

    //=============================================
    // End reference A3
    //=============================================
}
