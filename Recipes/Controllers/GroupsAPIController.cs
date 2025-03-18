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
    public class GroupsAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/GroupsAPI
        public IQueryable<RecipeGroup> GetRecipeGroups()
        {
            return db.RecipeGroups;
        }

        // GET: api/GroupsAPI/5
        [ResponseType(typeof(RecipeGroup))]
        public IHttpActionResult GetRecipeGroup(int id)
        {
            RecipeGroup recipeGroup = db.RecipeGroups.Find(id);
            if (recipeGroup == null)
            {
                return NotFound();
            }

            return Ok(recipeGroup);
        }

        [HttpGet]
        [Route("api/GroupsAPI/user-groups/{id}")]
        public IHttpActionResult GetUserRecipeGroups(string id)
        {
            if (id == null)
            {
                id = User.Identity.GetUserId();
            }
            var groups = db.RecipeGroups.Where(g => g.UserId == id).ToList();

            var result = groups.Select(g => new {
                g.Name,
                g.Id,
                Count = g.Recipes.Count(),
            }).ToList();
            return Ok(result);
        }



        [HttpPost]
        [Route("api/GroupsAPI/add-recipe")]
        public IHttpActionResult AddRecipeToGroup([FromBody] RecipeToGroupDTO dto)
        {
            var recipe = db.Recipes.FirstOrDefault(r => r.Id == dto.recipeId);
            if(recipe == null)
            {
                return NotFound();
            }
            var group = db.RecipeGroups.FirstOrDefault(g => g.Id == dto.groupId);
            if(group == null)
            {
                return NotFound();
            }

            group.Recipes.Add(recipe);
            db.SaveChanges();
            return Ok(new { Message = "Recipe added to group successfully" });
        }

        [HttpPost]
        [Route("api/GroupsAPI/remove-recipe")]
        public IHttpActionResult RemoveRecipeFromGroup([FromBody] RecipeToGroupDTO dto)
        {
            var recipe = db.Recipes.FirstOrDefault(r => r.Id == dto.recipeId);
            if (recipe == null)
            {
                return NotFound();
            }
            var group = db.RecipeGroups.FirstOrDefault(g => g.Id == dto.groupId);
            if (group == null)
            {
                return NotFound();
            }

            group.Recipes.Remove(recipe);
            db.SaveChanges();
            return Ok(new { Message = "Recipe removed from group successfully" });
        }




        // PUT: api/GroupsAPI/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutRecipeGroup(int id, RecipeGroup recipeGroup)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != recipeGroup.Id)
            {
                return BadRequest();
            }

            db.Entry(recipeGroup).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecipeGroupExists(id))
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

        // POST: api/GroupsAPI
        [HttpPost]
        [Route("api/GroupsAPI")]
        public IHttpActionResult PostRecipeGroup([FromBody] string Name)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.Identity.GetUserId();
            var recipeGroup = new RecipeGroup() { Name = Name};
            recipeGroup.UserId = userId;

            db.RecipeGroups.Add(recipeGroup);
            db.SaveChanges();

            return Ok(new { Id = recipeGroup.Id, Name = recipeGroup.Name, Count = 0 });
        }

        // DELETE: api/GroupsAPI/5
        [ResponseType(typeof(RecipeGroup))]
        [HttpDelete]
        [Route("api/GroupsAPI/delete-group/{id}")]
        public IHttpActionResult DeleteRecipeGroup(int id)
        {
            RecipeGroup recipeGroup = db.RecipeGroups.Find(id);
            if (recipeGroup == null)
            {
                return NotFound();
            }

            db.RecipeGroups.Remove(recipeGroup);
            db.SaveChanges();

            return Ok(new { Message = "Recipe group deleted successfully" });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RecipeGroupExists(int id)
        {
            return db.RecipeGroups.Count(e => e.Id == id) > 0;
        }
    }
}