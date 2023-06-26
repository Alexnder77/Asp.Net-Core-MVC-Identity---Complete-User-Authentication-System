using Microsoft.AspNetCore.Identity;

namespace AuthSystem.Models {
    public class AppUser : IdentityUser {
        public List<Item> Items { get; set; }
    }
    public class Item {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Power { get; set; }
        public string Charges { get; set; }
        public AppUser AppUser { get; set; }
    }
}
