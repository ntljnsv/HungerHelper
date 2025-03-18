using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Recipes.Models;
using Recipes.Models.Recipes;
using Recipes.Models.RecipeUtil;

namespace Recipes.Controllers
{
    public class RecipesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Recipes
        public ActionResult Index()
        {
            //var recipes = db.Recipes.Include(r => r.Author);
            var recipes = db.Recipes.AsQueryable();
            var result = recipes.Select(r => new RecipeBasicsDTO()
            {
                Id = r.Id,
                Title = r.Title,
                LikeCount = r.LikeCount,
                AuthorName = r.Author == null ? "NO AUTHOR YET" : r.Author.UserName,
                AuhtorId = r.Author == null ? "" : r.Author.Id,
                NumServings = r.NumServings,
                TimeNeeded = r.TimeNeeded,
                FirstPhotoPath = r.Contents.OfType<RecipePhoto>().FirstOrDefault().Path
                
            }).ToList();
            ViewBag.Categories = new List<string> { "Breakfast", "Lunch", "Dinner", "Snack", "Dessert" };
            return View(result);
        }

        // GET: Recipes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipe recipe = db.Recipes.Find(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }

            string userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            ViewBag.UserId = User.Identity.IsAuthenticated ? userId: "";
            ViewBag.UserProfilePicture = User.Identity.IsAuthenticated ? user.PicturePath : "";



            RecipeDetails model = new RecipeDetails()
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Steps = recipe.Steps,
                RecipeIngredients = recipe.RecipeIngredients,
                Contents = recipe.Contents,
                Comments = recipe.Comments,
                Likes = recipe.Likes,
                LikeCount = recipe.LikeCount,
                TimeNeeded = recipe.TimeNeeded,
                Category = recipe.Category,
                CreatedAt = recipe.CreatedAt,
                NumServings = recipe.NumServings,
                AuthorId = recipe.AuthorId,
                Author = recipe.Author,
            };
            model.UserGroups = db.RecipeGroups.Where(g => g.UserId == userId).ToList();
            return View(model);
        }


        [Authorize(Roles = "Author")]
        public ActionResult Create()
        {
            var model = new CreateRecipeVM();
            model.Id = 0;
            model.Categories = new List<string> { "Breakfast", "Lunch", "Dinner", "Snack", "Dessert" };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateRecipeVM model)
        {
            if (ModelState.IsValid)
            {
                var recipe = new Recipe() { 
                    AuthorId = User.Identity.GetUserId(),
                    CreatedAt = DateTime.UtcNow,
                    Title = model.Title,
                    NumServings = model.NumServings,
                    TimeNeeded = model.TimeNeeded,
                    Category = model.Category
                };
                var files = Request.Files;
                if(model.Contents != null)
                {
                    recipe.Contents = GetContents(files, model.Contents, recipe.Id);
                }
                if(model.Ingredients != null)
                {
                    recipe.RecipeIngredients = GetIngredients(model.Ingredients, recipe.Id);
                }
                if(model.Steps != null)
                {
                    recipe.Steps = GetSteps(model.Steps, recipe.Id);
                }
                db.Recipes.Add(recipe);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        private List<RecipeStep> GetSteps(List<RecipeStep> Steps, int recipeId)
        {
            List<RecipeStep> recipeSteps = new List<RecipeStep>();
            foreach(var step in Steps)
            {
                if (!string.IsNullOrWhiteSpace(step.Description)) {
                    step.RecipeId = recipeId;
                    recipeSteps.Add(step);
                }
            }
            return recipeSteps;
        }

        private List<RecipeIngredient> GetIngredients(List<RecipeIngredient> Ingredients, int recipeId)
        {
            List<RecipeIngredient> ingredients = new List<RecipeIngredient>();
            foreach (var recipeIngredient in Ingredients)
            {
                if (!string.IsNullOrWhiteSpace(recipeIngredient.Ingredient.Name))
                {
                    var ingName = recipeIngredient.Ingredient.Name.ToLower();
                    if (!db.Ingredients.Any(i => i.Name == ingName))
                    {
                        var i = new Ingredient() { Name = ingName };
                        db.Ingredients.Add(i);
                        db.SaveChanges();
                    }
                    if(recipeIngredient.Quantity == null) recipeIngredient.Quantity = "0";
                    if (string.IsNullOrWhiteSpace(recipeIngredient.Unit)) recipeIngredient.Unit = "none";
                    var ingredient = db.Ingredients.FirstOrDefault(i => i.Name == ingName);
                    recipeIngredient.Ingredient = null;
                    recipeIngredient.IngredientId = ingredient.Id;
                    recipeIngredient.RecipeId = recipeId;
                    ingredients.Add(recipeIngredient);
                }
            }
            db.SaveChanges();
            return ingredients;
        }

        private List<Content> GetContents(HttpFileCollectionBase files, ICollection<Content> Contents, int recipeId) 
        {
            var photoIdx = 0;
            List<Content> contentList = new List<Content>();
            foreach (var content in Contents)
            {
                if(content != null)
                {
                    if (content is RecipeText text && !string.IsNullOrWhiteSpace(text.Text))
                    {
                        content.RecipeId = recipeId;
                        contentList.Add(content);
                    }
                    else if (content.ContentType == "Photo")
                    {
                        var file = files[photoIdx++];
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Uploads/"), fileName);
                        file.SaveAs(path);

                        var photo = new RecipePhoto
                        {
                            Path = "/Uploads/" + fileName,
                            Order = content.Order,
                            RecipeId = recipeId
                        };
                        contentList.Add(photo);
                    }
                }
                
            }
            db.SaveChanges();
            return contentList;
        }

        [Authorize(Roles = "Author")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipe recipe = db.Recipes.Find(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            CreateRecipeVM model = new CreateRecipeVM()
            {
                Title = recipe.Title,
                TimeNeeded = recipe.TimeNeeded,
                NumServings = recipe.NumServings,
                Contents = recipe.Contents.ToList(),
                Ingredients = recipe.RecipeIngredients.ToList(),
                Steps = recipe.Steps.ToList(),
            };
            model.Categories = new List<string> { "Breakfast", "Lunch", "Dinner", "Snack", "Dessert" };
            model.Id = (int)id;
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CreateRecipeVM model)
        {
            if (ModelState.IsValid)
            {
                var recipe = db.Recipes.Find(model.Id);
                if(recipe == null)
                {
                    return HttpNotFound();
                }
                recipe.Title = model.Title;
                recipe.TimeNeeded = model.TimeNeeded;
                recipe.NumServings = model.NumServings;
                recipe.Category = model.Category;
                var files = Request.Files;
                if(files != null)
                {
                    UpdateContents(files, model.Contents, recipe);
                }
                if(model.Steps != null)
                {
                    UpdateSteps(model.Steps, recipe);
                }
                if(model.Ingredients != null)
                {
                    UpdateIngredients(model.Ingredients, recipe);
                }
                db.Entry(recipe).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ;
            return View(model);
        }

        private void UpdateContents(HttpFileCollectionBase files, List<Content> Contents, Recipe recipe)
        {
            int photoIdx = 0;
            if (Contents == null) return;
            for(int i=0; i<Contents.Count; i++)
            {
                var content = Contents[i];
                var existing = recipe.Contents.FirstOrDefault(c => c.Order == content.Order);
                if(content is RecipeText newText && !string.IsNullOrWhiteSpace(newText.Text))
                {
                    if(existing != null)
                    {
                        var text = existing as RecipeText;
                        text.Text = newText.Text;
                    }
                }
                if(content is RecipePhoto newPhoto)
                {
                    var file = files[photoIdx++];
                    if(file != null && file.ContentLength > 0)
                    {
                        if(existing != null)
                        {
                            var photo = existing as RecipePhoto;
                            if(System.IO.File.Exists(photo.Path))
                            {
                                System.IO.File.Delete(photo.Path);
                            }
                        }
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Uploads/"), fileName);
                        file.SaveAs(path);

                        if(existing != null)
                        {
                            var photo = existing as RecipePhoto;
                            photo.Path = path;
                            photo.Path = "/Uploads/" + fileName;
                        }
                    }
                }
            }
            db.SaveChanges();
        }


        private void UpdateSteps(List<RecipeStep> Steps, Recipe recipe)
        {
            for(int i=0; i<Steps.Count; i++)
            {
                var existing = recipe.Steps.FirstOrDefault(s => s.StepNumber == Steps[i].StepNumber);
                if(existing.Description != Steps[i].Description && !string.IsNullOrWhiteSpace(Steps[i].Description))
                {
                    existing.Description = Steps[i].Description;
                }
            }
            db.SaveChanges();
        }

        private void UpdateIngredients(List<RecipeIngredient> Ingredients, Recipe recipe)
        {
            var existingIngredients = recipe.RecipeIngredients.ToList();
            foreach(var newIngredient in Ingredients)
            {
                var name = newIngredient.Ingredient.Name.ToLower();
                newIngredient.Ingredient.Name = name;
                if (string.IsNullOrWhiteSpace(name)) continue;
                if (string.IsNullOrWhiteSpace(newIngredient.Quantity)) newIngredient.Quantity = "0";
                if (string.IsNullOrWhiteSpace(newIngredient.Unit)) newIngredient.Unit = "none";
                var existing = existingIngredients.FirstOrDefault(i => i.Ingredient.Name == name);
                if (existing != null)
                {
                    existing.Unit = newIngredient.Unit;
                    existing.Quantity = newIngredient.Quantity;
                }
                else
                {
                    if(!db.Ingredients.Any(i => i.Name == name))
                    {
                        var iDB = new Ingredient() { Name = name };
                        db.Ingredients.Add(iDB);
                        db.SaveChanges();
                    }
                    var ingDB = db.Ingredients.FirstOrDefault(i => i.Name == name);
                    var newRIngredient = new RecipeIngredient()
                    {
                        IngredientId = ingDB.Id,
                        RecipeId = recipe.Id,
                        Quantity = newIngredient.Quantity,
                        Unit = newIngredient.Unit
                    };
                    recipe.RecipeIngredients.Add(newRIngredient);
                }
            }
            foreach(var existing in existingIngredients)
            {
                if (!Ingredients.Any(i => i.Ingredient.Name.ToLower() == existing.Ingredient.Name))
                {
                    recipe.RecipeIngredients.Remove(existing);
                    db.Entry(existing).State = EntityState.Deleted;

                    if(!db.Ingredients.Any(i => existing.RecipeId == i.Id))
                    {
                        var ingredient = db.Ingredients.Find(existing.IngredientId);
                        db.Ingredients.Remove(ingredient);
                    }
                }
            }
            db.SaveChanges();
        }


        // GET: Recipes/Delete/5
        [Authorize(Roles = "Author")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipe recipe = db.Recipes.Find(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            return View(recipe);
        }

        // POST: Recipes/Delete/5
        [Authorize(Roles = "Author")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Recipe recipe = db.Recipes.Find(id);
            ICollection<Ingredient> ingredients = recipe.RecipeIngredients.Select(i => i.Ingredient).ToList();
            db.Recipes.Remove(recipe);
            db.SaveChanges();
            DeleteIngredients(ingredients);
            return RedirectToAction("Index");
        }

        private void DeleteIngredients(ICollection<Ingredient> ingredients)
        {
            foreach(var i in ingredients)
            {
                if(!db.RecipeIngredients.Any(ri => ri.IngredientId == i.Id))
                {
                    db.Ingredients.Remove(i);
                }
            }
            db.SaveChanges();
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
