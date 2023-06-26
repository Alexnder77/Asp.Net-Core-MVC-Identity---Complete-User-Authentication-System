using System.Linq;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotBlocket2.Models;
//using Umbraco.Core.Models;

namespace NotBlocket2.Controllers {


    public class NotBlocketController : Controller {


        [HttpGet]
        public IActionResult Start() {

            List<string> locationNames = new List<string>();
            List<int> ProfileCountPerLocation = new List<int>();
            string errormsg = string.Empty;

            Graphmethods gm = new Graphmethods();

            gm.GetLocationsWithDataSet(out locationNames, out ProfileCountPerLocation, out errormsg);

            ViewBag.LocationNames = locationNames;
            ViewBag.ProfileCountPerLocation = ProfileCountPerLocation;
            return View();
        }

        [HttpGet]
        public IActionResult SearchResults(string search, string sort) {

            //Dealing with the null-values
            if (sort == null) { sort = "Name"; }
            if (search != null) {

                ViewBag.search = search;
                ViewBag.sort = sort;

                //Get the Ad list
                List<Ad> Adlist = new List<Ad>();
                AdMethods pm = new AdMethods();
                string error = "";
                Adlist = pm.GetAdsWithDataSet2(sort, search, out error);
                ViewBag.error = error;
                return View(Adlist);
            }
            return RedirectToAction("Start");
        }


        [HttpGet]
        public IActionResult CreateAd() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAd(Ad ad) {
            AdMethods am = new AdMethods();
            string errorMessage = string.Empty;
            int rowsAffected = 0;

            if (!ModelState.IsValid) {
                ViewBag.error = "Invalid model state.";
                ViewBag.antal = rowsAffected;
                return View();
            }

            //ad.ImagePath = await am.SaveImageAsync(file);

            rowsAffected = await am.InsertAdAsync(ad);

            if (rowsAffected == 1) {
                return RedirectToAction("AdsView");
            }
            else {
                ViewBag.error = "The ad was not inserted into the database.";
                ViewBag.antal = rowsAffected;
                return View();
            }

        }

            [HttpGet]
        public IActionResult DeleteAd(int id) {
            // Remove the row with the specified ID from the database
            AdMethods am = new AdMethods();
            string error = "";
            am.DeleteAd(id, out error);
            ViewBag.error = error;

            return RedirectToAction("AdsView");
        }

        [HttpGet]
        public IActionResult EditAd(int Id) {
            string error = "";

            AdMethods pm = new AdMethods();
            Ad ad = pm.GetAdById(Id, out error);
            ViewBag.error = error;
            if (ad != null) {
                return View(ad);
            }
            else {
                return View(ad);
            }
        }


        [HttpPost]
        public IActionResult EditAd(Ad ad) {
            string error = "";
            if (ModelState.IsValid) {
                // updt databse and go back to list
                AdMethods pm = new AdMethods();
                

                pm.UpdateAd(ad, out error);
                ViewBag.error = error;
                return RedirectToAction("MyAdsView");
                //return View(ad);
            }

            var errorMessage = "Validation errors: ";
            foreach (var errorr in ModelState.Values) {
                if (errorr.Errors.Count > 0) {
                    errorMessage += errorr.Errors[0].ErrorMessage + "; ";
                }
            }
            ViewBag.error = error + errorMessage;
            return View(ad);
        }



        [HttpGet]
        public ActionResult ProfilesView() {
            List<Profile> Profilelist = new List<Profile>();
            ProfileMethods pm = new ProfileMethods();
            string error = "";
            Profilelist = pm.GetPersonWithDataSet(out error);
            ViewBag.error = error;
            return View(Profilelist);
        }

		
		public IActionResult AdsView() {
            ProfileMethods pm = new ProfileMethods();
            LocationMethods lm = new LocationMethods();
            AdMethods am = new AdMethods();


			string selectedValue = null;
		    if (Request.Method == "POST") {
				selectedValue = Request.Form["Category"];
			}
			ViewBag.location = selectedValue;
            
			ViewModelProfileAdsLocation myModel = new ViewModelProfileAdsLocation
            {
                ProfileList = pm.GetPersonWithDataSet(out string errormsg),
                LocationList = lm.GetLocationsWithDataSet(out string errormsg2),

                //ad list for the categories dropdown
                AdList = am.GetAdsWithDataSet(out string errormsg3),

                //seperate ad list for the view
				FilterdAdList = am.GetAdsWithDataSet(out string errormsg4, selectedValue)

			};
            ViewBag.error = "1: " + errormsg + "2: " + errormsg2 + "3: " + errormsg3 + "4: " + errormsg4;


			return View(myModel);
        }


        public IActionResult MyAdsView() {
            string userId = User.Identity.Name;

            ViewBag.userId = userId;
           
            AdMethods am = new AdMethods();


            ViewModelProfileAdsLocation myModel = new ViewModelProfileAdsLocation {
                AdList = am.GetAdsWithDataSetForUser(userId, out string adErrorMsg), // Filter ads by user ID
               
            };

            ViewBag.error = $" 3: {adErrorMsg}";

            return View(myModel);

        }


		// Method for individual ad pages 
		// when an ad is pressed in the list view 
		// go to an individual site for that ad 
		public IActionResult AdPage(int adId) {
			AdMethods am = new AdMethods();
			Ad ad = am.GetAdById(adId, out string adErrorMsg);

			if (ad != null) {
				return View(ad);
			}

			ViewBag.error = adErrorMsg;
			return View("Error");
		}




	}
}
