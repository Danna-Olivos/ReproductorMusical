namespace Database
{
    public abstract class Performer
    {
        public int IdPerformer{get;set;}
        public string? Name {get;set;}
        public Type? type{get;set;}

        public class Type
        {
            public int IdType{get;set;}
            public string? Description{get;set;}

        }

    }
}