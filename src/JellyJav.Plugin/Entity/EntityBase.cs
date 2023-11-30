namespace JellyJav.Plugin.Entity
{
    public abstract class EntityBase
    {
        public string Id { get; set; }

        public EntityBase(string id)
        {
            Id = id;
        }
    }
}
