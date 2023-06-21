namespace NotBlocket2.Models {
	public class ViewModelProfileAdsLocation {
		public IEnumerable<Profile> ProfileList { get; set; }
		public IEnumerable<Location> LocationList { get; set; }
		public IEnumerable<Ad> AdList { get; set; }
		public IEnumerable<Ad> FilterdAdList { get; set; }
	}
}
