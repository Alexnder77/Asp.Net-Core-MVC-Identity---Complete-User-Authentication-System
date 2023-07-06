using Microsoft.AspNetCore.Mvc;
using NotBlocket2.Models;
using Microsoft.AspNetCore.Http;


namespace NotBlocket2.Controllers {


    public class NotBlocketController : Controller {


        [HttpGet]
        public IActionResult Start() {
            
            /*
            List<string> locationNames = new List<string>();
            List<int> ProfileCountPerLocation = new List<int>();
            string errormsg = string.Empty;

            Graphmethods gm = new Graphmethods();

            gm.GetLocationsWithDataSet(out locationNames, out ProfileCountPerLocation, out errormsg);

            ViewBag.LocationNames = locationNames;
            ViewBag.ProfileCountPerLocation = ProfileCountPerLocation;

            */
            return View();
        }

        [HttpGet]
        public IActionResult SearchResults(string search, string sort) {
            //Get the Ad list
            List<Ad> Adlist = new List<Ad>();
            AdMethods pm = new AdMethods();
            string error = "";

            //Dealing with the null-values
            if (sort == null) { 
                sort = "Name";
                HttpContext.Session.SetString("Sort", sort);
            }

            if (search != null) {
                // Store search and sort values in session
                HttpContext.Session.SetString("Search", search);
                HttpContext.Session.SetString("Sort", sort);

                Adlist = pm.GetAdsWithDataSet2(sort, search, out error);
                ViewBag.error = error;

                return View(Adlist);
            }

            // search is empty
            Adlist = pm.GetAdsWithDataSet(out error);

            return View(Adlist);
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

            return RedirectToAction("MyAdsView");
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

		
		public IActionResult AdsView() {
            AdMethods am = new AdMethods();

			string selectedValue = null;
		    if (Request.Method == "POST") {
				selectedValue = Request.Form["Category"];
			}
			ViewBag.location = selectedValue;
            
			AdLists myModel = new AdLists {
                //ad list for the categories dropdown
                AdList = am.GetAdsWithDataSet(out string errormsg3),

                //seperate ad list for the view
				FilterdAdList = am.GetAdsWithDataSet(out string errormsg4,selectedValue)
			};
            ViewBag.error = "3: " + errormsg3 + "4: " + errormsg4;


			return View(myModel);
        }


        public IActionResult MyAdsView() {
            string userId = User.Identity.Name;

            ViewBag.userId = userId;
           
            AdMethods am = new AdMethods();

            AdLists myModel = new AdLists {
                //ad list for the categories dropdown
                AdList = am.GetAdsWithDataSet(out string errormsg3),
            };

            ViewBag.error = $" 3: {errormsg3}";

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
