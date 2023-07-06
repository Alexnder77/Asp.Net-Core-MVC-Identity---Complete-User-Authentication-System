using NotBlocket2.Models;

namespace NotBlocket2.Models {
    public class AdLists {
        public IEnumerable<Ad> AdList { get; set; }
        public IEnumerable<Ad> FilterdAdList { get; set; }
    }
}
