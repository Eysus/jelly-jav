using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JellyJav.Plugin.Entity
{
    public class Actress: EntityBase
    {
        public string Name { get; set; }
        public DateTime? Birthdate { get; set; }
        public string? Birthlace { get; set; }
        public string? Cover { get; set; }

        public Actress(
            string id,
            string name,
            DateTime? birthdate,
            string? birthlace,
            string? cover
        ) : base(id) {
            Name = name;
            Birthdate = birthdate;
            Birthlace = birthlace;
            Cover = cover;
        }
    }
}
