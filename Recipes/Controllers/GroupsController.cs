using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Recipes.Models;
using Recipes.Models.Recipes;
using Recipes.Models.RecipeUtil;

namespace Recipes.Controllers
{
    public class GroupsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Groups
        public ActionResult Index()
        {
            var recipeGroups = db.RecipeGroups.Include(r => r.User);
            return View(recipeGroups.ToList());
        }


        public ActionResult UserGroups(string id)
        {
            if (id == null)
            {
                id =  User.Identity.GetUserId();
            }
            var user = db.Users.FirstOrDefault(u => u.Id == id);
            if(user == null)
            {
                return HttpNotFound();
            }
            var groups = db.RecipeGroups.Where(g => g.UserId == id).ToList();
            var model = groups.Select(g => new RecipeGroupDTO {
                Id = g.Id,
                Name = g.Name,
                UserId = g.UserId,
                User = g.User,
                Count = g.Recipes.Count()
            }).ToList();
            ViewBag.UserName = user.UserName;
            ViewBag.UserId = user.Id;
            ViewBag.CurrentUser = User.Identity.GetUserId();
            return View(model);
        }

        // GET: Groups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RecipeGroup recipeGroup = db.RecipeGroups.Find(id);
            if (recipeGroup == null)
            {
                return HttpNotFound();
            }

            var recipes = recipeGroup.Recipes.Select(r => new RecipeBasicsDTO
            {
                Id = r.Id,
                Title = r.Title,
                LikeCount = r.LikeCount,
                AuthorName = r.Author != null ? r.Author.UserName : "",
                NumServings = r.NumServings,
                TimeNeeded = r.TimeNeeded,
                FirstPhotoPath = r.Contents != null && r.Contents.OfType<RecipePhoto>().Any()
                    ? r.Contents.OfType<RecipePhoto>().First().Path
                    : null,
                AuhtorId = r.Author != null ? r.Author.Id : ""
            }).ToList();

            var model = new RecipeGroupDTO
            {
                Id = recipeGroup.Id,
                Name = recipeGroup.Name,
                UserId = recipeGroup.UserId,
                User = recipeGroup.User,
                Recipes = recipes
            };
            ViewBag.userId = User.Identity.IsAuthenticated ? User.Identity.GetUserId() : "";
            return View(model);
        }

        // GET: Groups/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.ApplicationUsers, "Id", "Bio");
            return View();
        }

        // POST: Groups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,UserId")] RecipeGroup recipeGroup)
        {
            if (ModelState.IsValid)
            {
                db.RecipeGroups.Add(recipeGroup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.ApplicationUsers, "Id", "Bio", recipeGroup.UserId);
            return View(recipeGroup);
        }

        // GET: Groups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RecipeGroup recipeGroup = db.RecipeGroups.Find(id);
            if (recipeGroup == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.ApplicationUsers, "Id", "Bio", recipeGroup.UserId);
            return View(recipeGroup);
        }

        // POST: Groups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,UserId")] RecipeGroup recipeGroup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(recipeGroup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.ApplicationUsers, "Id", "Bio", recipeGroup.UserId);
            return View(recipeGroup);
        }

        // GET: Groups/Delete/5
        [HttpDelete]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RecipeGroup recipeGroup = db.RecipeGroups.Find(id);
            if(recipeGroup == null)
            {
                return HttpNotFound();
            }
            db.RecipeGroups.Remove(recipeGroup);
            db.SaveChanges();
            return RedirectToAction("UserProfile", "Users");
        }

        // POST: Groups/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    RecipeGroup recipeGroup = db.RecipeGroups.Find(id);
        //    db.RecipeGroups.Remove(recipeGroup);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
