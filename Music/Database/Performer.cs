namespace Database
{
    public abstract class Performer
    {
        public int IdPerformer{get;set;}
        public string? Name {get;set;}
        public Type? type{get;set;}

        public class Type
        {
            public int idType{get;set;}
            public string? description{get;set;}

        }

    }
}