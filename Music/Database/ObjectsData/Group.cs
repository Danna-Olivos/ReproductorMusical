namespace Database
{
    public class Group
    {
        public int IdGroup{get;set;} //IPK
        public string GroupName {get;set;}
        public string StartDate{get;set;}
        public string EndDate{get;set;}

        public Group(string name, string start_date, string end_date)
        {
            GroupName = name;
            StartDate = start_date;
            EndDate = end_date;
        }

        public Group(int id_group, string name, string start_date, string end_date)
        {
            IdGroup = id_group;
            GroupName = name;
            StartDate = start_date;
            EndDate = end_date;
        }
        
    }
}