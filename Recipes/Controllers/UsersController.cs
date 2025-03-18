using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PagedList;
using Recipes.Models;
using Recipes.Models.ProfileUtil;
using Recipes.Models.Recipes;
using Recipes.Models.RecipeUtil;

namespace Recipes.Controllers
{
    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult UserProfile(string id, int page = 1, int pageSize = 2, string sortOrder = "date")
        {
            ApplicationUser applicationUser;
            if (id == null)
            {
                id = User.Identity.GetUserId();
            }
            applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            var model = new ProfileVM()
            {
                Id = applicationUser.Id,
                Name = applicationUser.UserName,
                Email = applicationUser.Email,
                Bio = applicationUser.Bio,
                PicturePath = applicationUser.PicturePath
            };
            if(applicationUser is Author author)
            {
                var recipes = author.Recipes.AsQueryable();  
                var sorted = sortedRecipes(sortOrder, recipes, page, pageSize);
                model.Recipes = sorted;
            }
            if(applicationUser is User user)
            {
                var likes = user.Likes.AsQueryable();
                var sorted = sortedLikedRecipes(sortOrder, likes, page, pageSize);
                model.LikedRecipes = sorted;
            }
            ViewBag.sortOrder = sortOrder;
            ViewBag.userId = User.Identity.IsAuthenticated ? User.Identity.GetUserId() : "";
            return View(model);
        }

        private IPagedList<RecipeBasicsDTO> sortedRecipes(string sortOrder, IQueryable<Recipe> recipes, int page, int pageSize)
        {
            if (sortOrder == "date") recipes = recipes.OrderBy(r => r.CreatedAt);
            if (sortOrder == "likes") recipes = recipes.OrderByDescending(r => r.LikeCount);
            if (sortOrder == "title") recipes = recipes.OrderByDescending(r => r.Title);
            var sorted = createPagedList(recipes, page, pageSize);

            return sorted;
        }

        private IPagedList<RecipeBasicsDTO> sortedLikedRecipes(string sortOrder, IQueryable<Like> likes, int page, int pageSize)
        {
            if (sortOrder == "date") likes = likes.OrderByDescending(l => l.DateLiked);
            if (sortOrder == "likes") likes = likes.OrderByDescending(l => l.Recipe.LikeCount) ;
            if (sortOrder == "title") likes = likes.OrderBy(l => l.Recipe.Title);
            if (sortOrder == "author") likes =  likes.OrderBy(l => l.Recipe.Author.UserName);

            var recipes = likes.Select(l => l.Recipe).AsQueryable();

            return createPagedList(recipes, page, pageSize);
        }

        private IPagedList<RecipeBasicsDTO> createPagedList(IQueryable<Recipe> recipes, int page, int pageSize)
        {
            var sorted = recipes.Select(r => new RecipeBasicsDTO
            {
                Id = r.Id,
                Title = r.Title,
                LikeCount = r.LikeCount,
                AuhtorId = r.Author != null ? r.Author.Id : "",
                AuthorName = r.Author != null ?  r.Author.UserName : "no author",
                NumServings = r.NumServings,
                TimeNeeded = r.TimeNeeded,
                FirstPhotoPath = r.Contents != null && r.Contents.OfType<RecipePhoto>().Any()
                    ? r.Contents.OfType<RecipePhoto>().First().Path
                    : null
            }).ToPagedList(page, pageSize);

            return sorted;
        }

        // GET: Users/EditProfile/5
        [Authorize]
        public ActionResult EditProfile()
        {
            var id = User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            var model = new EditProfileVM()
            {
                Name = applicationUser.UserName,
                Bio = applicationUser.Bio,
                PicturePath = applicationUser.PicturePath
            };
            return View(model);
        }

        // POST: Users/EditProfile/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(EditProfileVM model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                var id = User.Identity.GetUserId();
                var user = db.Users.Find(id);
                if(user == null)
                {
                    return HttpNotFound();
                }

                user.UserName = model.Name;
                user.Bio = model.Bio;

                if(file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Uploads"), fileName);
                    file.SaveAs(path);
                    user.PicturePath = "~/Uploads/" + fileName;
                }
                db.SaveChanges();
                return RedirectToAction("UserProfile");
            }
            return View(model);
        }


        public ActionResult GetRecipes(string id, string sortOrder, int page = 1, int pageSize = 2)
        {
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }

            var model = new ProfileVM();
            model.Id = applicationUser.Id;
            if (applicationUser is Author author)
            {
                var recipes = author.Recipes.AsQueryable();
                var sorted = sortedRecipes(sortOrder, recipes, page, pageSize);
                model.Recipes = sorted;
            }

            if (applicationUser is User user)
            {
                var likes = user.Likes.AsQueryable();
                var sorted = sortedLikedRecipes(sortOrder, likes, page, pageSize);
                model.LikedRecipes = sorted;
            }

            ViewBag.sortOrder = sortOrder;
            ViewBag.userId = User.Identity.GetUserId();

            return PartialView("_RecipePartial", model);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
