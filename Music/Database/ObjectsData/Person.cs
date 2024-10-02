using System.Data.Common;

namespace Database
{
    public class Person
    {

        public int IdPerson{get;set;} //IPK
        public string StageName{get;set;}
        public string BirthDate{get;set;}
        public string DeathDate{get;set;} 

        Person(string stage_name, string birth_date, string death_date)
        {
            StageName = stage_name;
            BirthDate = birth_date;
            DeathDate = death_date;
        }
    }
}