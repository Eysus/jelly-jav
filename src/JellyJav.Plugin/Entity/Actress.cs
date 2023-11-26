namespace JellyJav.Plugin.Entity
{
    public class Actress : EntityBase
    {
        public string Name { get; set; }
        public DateTime? Birthdate { get; set; }
        public string? Birthplace { get; set; }
        public string? Cover { get; set; }

        public Actress(
            string id,
            string name,
            DateTime? birthdate,
            string? birthplace,
            string? cover
        ) : base(id)
        {
            Name = name;
            Birthdate = birthdate;
            Birthplace = birthplace;
            Cover = cover;
        }

        public override bool Equals(object? obj)
        {
            return obj is Actress actress &&
                   Name == actress.Name &&
                   Birthdate == actress.Birthdate &&
                   Birthplace == actress.Birthplace &&
                   Cover == actress.Cover;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Birthdate, Birthplace, Cover);
        }
    }
}
