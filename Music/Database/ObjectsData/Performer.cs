namespace Database
{
    public class Performer
    {
        public int IdPerformer{get;set;} //IPK
        public string Name {get;set;}
        public Type.ArtistType Type{get;set;}

        //factoryyyyy
        public Performer(string name, Type.ArtistType type)
        {
            Name = name;
            Type = type;
        }

        public Performer(int id_performer, string name, Type.ArtistType type)
        {
            IdPerformer = id_performer;
            Name = name;
            Type = type;
        }

    }
}