namespace Database
{
    public abstract class Performer
    {
        public int IdPerformer{get;set;}
        public string Name {get;set;}
        public required Type type{get;set;}

        //factoryyyyy
        protected Performer(int idPerformer, string name, Type type)
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