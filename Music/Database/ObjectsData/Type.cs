namespace Database
{
    public class Type
    {
        public int IdType{get;set;}
        public string? Description{get;set;}

        public enum ArtistType
        {
            Person = 0,
            Group = 1,
            Unknown = 2
        }

    }
}