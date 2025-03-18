using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.AspNet.Identity;
using Recipes.Models;
using Recipes.Models.Recipes;
using Recipes.Models.RecipeUtil;

namespace Recipes.Controllers
{
    public class RecipesAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/RecipesAPI
        public IQueryable<Recipe> GetRecipes()
        {
            return db.Recipes;
        }

        // GET: api/RecipesAPI/5
        [ResponseType(typeof(Recipe))]
        public IHttpActionResult GetRecipe(int id)
        {
            Recipe recipe = db.Recipes.Find(id);
            if (recipe == null)
            {
                return NotFound();
            }

            return Ok(recipe);
        }

        [HttpGet]
        [Route("api/RecipesAPI/all-recipes")]
        public IHttpActionResult GetAllRecipes()
        {
            var recipes = db.Recipes.AsQueryable();
            var result = recipes.Select(r => new RecipeBasicsDTO()
            {
                Id = r.Id,
                Title = r.Title,
                LikeCount = r.LikeCount,
                AuthorName = r.Author == null ? "NO AUTHOR YET" : r.Author.UserName,
                AuhtorId = r.Author == null ? "" : r.AuthorId,
                NumServings = r.NumServings,
                TimeNeeded = r.TimeNeeded,
                FirstPhotoPath = r.Contents.OfType<RecipePhoto>().FirstOrDefault().Path
            }).ToList();
            return Ok(result);
        }


        [HttpPost]
        [Route("api/RecipesAPI/search-filter")]
        public IHttpActionResult SearchRecipes([FromBody] SearchFilter model)
        {
            if (model == null || 
                (string.IsNullOrWhiteSpace(model.Query)
                && (model.IngredientIds == null || !model.IngredientIds.Any())
                && string.IsNullOrWhiteSpace(model.Category)))
            {
                return BadRequest("Search query and ingredients cannot both be empty.");
            }

            var words = model.Query?.ToLower().Split(' ').Where(q => !string.IsNullOrWhiteSpace(q)).ToList() ?? new List<string>();

            var recipes = db.Recipes.AsQueryable();


            if(words.Any())
            {
                recipes = recipes.Where(r => words.All(w =>
                    r.Author.UserName.ToLower().Contains(w.ToLower()) ||
                    r.Title.ToLower().Contains(w.ToLower()) ||
                    r.RecipeIngredients.Any(i => i.Ingredient.Name.ToLower().Contains(w.ToLower()))));
            }
            
            if(model.IngredientIds != null && model.IngredientIds.Any()) 
            {
                recipes = recipes.Where(r => r.RecipeIngredients.Any(ri => model.IngredientIds.Contains(ri.IngredientId)));
            }
            if(!string.IsNullOrEmpty(model.Category))
            {
                recipes = recipes.Where(r => r.Category == model.Category);
            }

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

            return Ok(result);
        }

        [HttpPost]
        [Route("api/RecipesAPI/like")]
        public IHttpActionResult Like([FromBody] int id)
        {
            var userId = User.Identity.GetUserId();
            var recipe = db.Recipes.Find(id);
            if (userId == null || recipe == null)
            {
                return NotFound();
            }
            var like = db.Likes.FirstOrDefault(l => l.RecipeId == id && l.UserId == userId);

            if (like == null)
            {
                like = new Like() {
                    RecipeId = id,
                    UserId = userId,
                    DateLiked = DateTime.UtcNow
                };
                db.Likes.Add(like);
                recipe.LikeCount += 1;
            }
            else
            {
                db.Likes.Remove(like);
                recipe.LikeCount -= 1;
            }
            db.SaveChanges();
            return Json(new { success = true, likeCount = recipe.LikeCount });
        }

        [HttpPost]
        [Route("api/RecipesAPI/comment/{id}")]
        public IHttpActionResult Comment(int id, [FromBody] string commentContent)
        {
            var userId = User.Identity.GetUserId();
            var recipe = db.Recipes.Find(id);
            if(userId == null || recipe == null)
            {
                return NotFound();
            }
            var user = db.Users.Find(userId);
            var comment = new Comment()
            {
                Content = commentContent,
                UserId = userId,
                RecipeId = id,
                CreatedAt = DateTime.UtcNow,
            };
            
            db.Comments.Add(comment);
            db.SaveChanges();
            var c = db.Comments.Find(comment.Id);
            var result = new
            {
                Content = c.Content,
                UserPhoto = user.PicturePath,
                CreatedAt = c.CreatedAt,
                UserName = user.UserName,
                UserId = userId
            };   
            return Ok(result);
        }




        // PUT: api/RecipesAPI/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutRecipe(int id, Recipe recipe)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != recipe.Id)
            {
                return BadRequest();
            }

            db.Entry(recipe).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecipeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/RecipesAPI
        [ResponseType(typeof(Recipe))]
        public IHttpActionResult PostRecipe(Recipe recipe)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Recipes.Add(recipe);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = recipe.Id }, recipe);
        }

        // DELETE: api/RecipesAPI/5
        [HttpDelete]
        [Route("api/RecipesAPI/{id}")]
        public IHttpActionResult DeleteRecipe(int id)
        {
            Recipe recipe = db.Recipes.Find(id);
            if (recipe == null)
            {
                return NotFound();
            }

            db.Recipes.Remove(recipe);
            db.SaveChanges();

            return Ok(new { Message = "Recipe deleted successfully" });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RecipeExists(int id)
        {
            return db.Recipes.Count(e => e.Id == id) > 0;
        }
    }
}