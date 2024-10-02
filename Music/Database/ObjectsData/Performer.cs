namespace Database
{
    public class Performer
    {
        public int IdPerformer{get;set;} //IPK
        public string Name {get;set;}
        public Type type{get;set;}

        //factoryyyyy
        Performer(string name, Type type)
        {
            Name = name;
            this.type = type;
        }

        public class Type
        {
            public int IdType{get;set;}
            public string? Description{get;set;}

            public enum Artist
            {
                Person = 0,
                Group = 1,
                Unknown = 2
            }

        }

    }
}