using Microsoft.AspNetCore.Mvc;
using RecipeBox.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;
using System;

namespace RecipeBox.Controllers
{
  public class IngredientsController : Controller
  {
    private readonly RecipeBoxContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public IngredientsController(UserManager<ApplicationUser> UserManager, RecipeBoxContext db)
    {
      _userManager = UserManager;
      _db = db;
    }

    public ActionResult Index()
    {
      List<Ingredient> model = _db.Ingredients.ToList();
      return View(model);
    }

    public ActionResult Create()
    {
      ViewBag.RecipeId = new SelectList(_db.Recipes, "RecipeId", "Name");
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(Ingredient ingredient, int RecipeId)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      
      _db.Ingredients.Add(ingredient);
      _db.SaveChanges();
      bool duplicate = _db.IngredientRecipes.Any(join => join.RecipeId == RecipeId && join.IngredientId == ingredient.IngredientId);
      if (RecipeId !=0 && !duplicate)
      {
        _db.IngredientRecipes.Add(new IngredientRecipe() { RecipeId = RecipeId, IngredientId = ingredient.IngredientId, User = currentUser});
      }
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public async Task<ActionResult> Details(int id)
    {

      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var thisIngredient = _db.Ingredients
        .Include(ingredient => ingredient.JoinEntities)
        .ThenInclude(join => join.Recipe)
        .FirstOrDefault(ingredient => ingredient.IngredientId == id);
      // var thisIngredientForUser = _db.IngredientRecipes.Where(entry => entry.User.Id == currentUser.Id);
      // var thisIngredientForUser = _db.IngredientRecipes.Where(thisIngredient.JoinEntities.Contains() == currentUser.Id);
      ViewBag.UsersIngredients = _db.IngredientRecipes.Where(entry => entry.User.Id == currentUser.Id && entry.IngredientId == id).ToList();
      return View(thisIngredient);
    }

// var userRecipes = _db.Recipes.Where(entry => entry.User.Id == currentUser.Id).ToList();

    public ActionResult Edit(int id)
    {
      var thisIngredient = _db.Ingredients.FirstOrDefault(ingredient => ingredient.IngredientId == id);
      ViewBag.RecipeId = new SelectList(_db.Recipes, "RecipeId", "Name");
      return View(thisIngredient);
    }

    [HttpPost]
    public async Task<ActionResult> Edit(Ingredient ingredient, int RecipeId)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);

      _db.Entry(ingredient).State = EntityState.Modified;
      _db.SaveChanges();
      bool duplicate = _db.IngredientRecipes.Any(join => join.RecipeId == RecipeId && join.IngredientId == ingredient.IngredientId);
      if (RecipeId !=0 && !duplicate)
      {
        _db.IngredientRecipes.Add(new IngredientRecipe() { RecipeId = RecipeId, IngredientId = ingredient.IngredientId, User = currentUser});
      }
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Delete(int id)
    {
      var thisIngredient = _db.Ingredients.FirstOrDefault(ingredient => ingredient.IngredientId == id);
      return View(thisIngredient);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisIngredient = _db.Ingredients.FirstOrDefault(ingredient => ingredient.IngredientId == id);
      _db.Ingredients.Remove(thisIngredient);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult> DeleteRecipe(int joinId)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      ViewBag.PageTitle = "Remove this Ingredient from the Recipe";
      var joinEntry = _db.IngredientRecipes.FirstOrDefault(entry => entry.IngredientRecipeId == joinId);
      Console.WriteLine(userId);
      Console.WriteLine(currentUser);
      Console.WriteLine(currentUser.Id);
      if (joinEntry.User.Id == userId)
      {
        _db.IngredientRecipes.Remove(joinEntry);
        _db.SaveChanges();
      }
      else
      {
        Console.WriteLine("You don't have authorization to delete this.");
      }
      return RedirectToAction("Index");

    }

  }
}