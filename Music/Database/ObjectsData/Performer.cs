namespace Database
{
    public class Performer
    {
        public int IdPerformer{get;set;}
        public string Name {get;set;}
        public Type type{get;set;}

        //factoryyyyy
        Performer(int idPerformer, string name, Type type)
        {
            IdPerformer = idPerformer;
            Name = name;
            this.type = type;
        }

        public class Type
        {
            public int IdType{get;set;}
            public string? Description{get;set;}

        }

    }
}