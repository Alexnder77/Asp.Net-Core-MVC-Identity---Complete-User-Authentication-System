using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotBlocket2.Models;

namespace AuthSystem.Controllers {
    public class AdController : Controller {
        public IActionResult Index() {
            return View();
        }

        [HttpGet]
        public IActionResult CreateAd() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAd(Ad ad, IFormFile file) {
            AdMethods am = new AdMethods();
            int i = 0;
            string error = "";

            if (!ModelState.IsValid) {
                ViewBag.error = "error: " + error;
                ViewBag.antal = i;
                return View();
            }


            // Set the user's profile ID on the newly created ad
            //ad.Profile_Id = UserManager.GetUserName(User);

            //ad.Profile_Id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            error = error + User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (file != null) {
                ad.ImagePath = await AdMethods.SaveFileAsync(file);
            }

            am.InsertAd(ad, out error);
            ViewBag.error = error;
            ViewBag.antal = i;
            return RedirectToAction("AdsView");
        }


    }
}
