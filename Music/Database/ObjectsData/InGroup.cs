namespace Database
{
    public class InGroup 
    {
        public int IdPerson{get;set;}
        public int IdGroup{get;set;}

        public InGroup(int id_person, int id_group)
        {
            IdPerson = id_person;
            IdGroup = id_group;
        }
    }
}