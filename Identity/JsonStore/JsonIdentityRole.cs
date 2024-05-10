using System;

namespace GSK.ArtWork.Web.Auth.CustomProvider
{
    public class JsonIdentityRole
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
    }
}
