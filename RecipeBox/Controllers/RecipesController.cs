using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using RecipeBox.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;
using System;


namespace RecipeBox.Controllers
{
  [Authorize]
  public class RecipesController : Controller
  {
    private readonly RecipeBoxContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public RecipesController(UserManager<ApplicationUser> UserManager, RecipeBoxContext db)
    {
      _userManager = UserManager;
      _db = db;
    }
  

    public async Task<ActionResult> Index()
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var userRecipes = _db.Recipes.Where(entry => entry.User.Id == currentUser.Id).OrderByDescending(recipe => recipe.Rating).ThenBy(recipe => recipe.Name).ToList();
      return View(userRecipes);
    }
    
    public ActionResult Create()
    {
      ViewBag.CategoryId = new SelectList(_db.Categories, "CategoryId", "Name");
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(Recipe recipe, int CategoryId)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      recipe.User = currentUser;
      _db.Recipes.Add(recipe);
      _db.SaveChanges();
      if (CategoryId !=0)
      {
        _db.CategoryRecipes.Add(new CategoryRecipe() { CategoryId = CategoryId, RecipeId = recipe.RecipeId});
      }
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Details(int id)
    {
      var thisRecipe = _db.Recipes
        .Include(recipe => recipe.JoinCategoryRecipes)
        .ThenInclude(join => join.Category)
        .Include(recipe => recipe.JoinIngredientRecipes)
        .ThenInclude(join => join.Ingredient)
        .FirstOrDefault(recipe => recipe.RecipeId == id);
      return View(thisRecipe);
    }

    public ActionResult Edit(int id)
    {
      var thisRecipe = _db.Recipes.FirstOrDefault(recipe => recipe.RecipeId == id);
      ViewBag.CategoryId = new SelectList(_db.Categories, "CategoryId", "Name");
      return View(thisRecipe);
    }

    [HttpPost]
    public ActionResult Edit(Recipe recipe, int CategoryId)
    {
      bool duplicate = _db.CategoryRecipes.Any(join => join.CategoryId == CategoryId && join.RecipeId == recipe.RecipeId);
      if (CategoryId !=0 && !duplicate)
      {
        _db.CategoryRecipes.Add(new CategoryRecipe() {CategoryId = CategoryId, RecipeId = recipe.RecipeId});
      }
      _db.Entry(recipe).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult AddCategory(int id)
    {
      var thisRecipe = _db.Recipes
      .FirstOrDefault(recipe => recipe.RecipeId == id);
      ViewBag.CategoryId = new SelectList(_db.Categories,"CategoryId", "Name");
      return View(thisRecipe);
    }    

    [HttpPost]
    public ActionResult AddCategory(Recipe recipe, int CategoryId)
    {
      bool duplicate = _db.CategoryRecipes.Any(join => join.CategoryId == CategoryId && join.RecipeId == recipe.RecipeId);
      if (CategoryId !=0 && !duplicate)
      {
        _db.CategoryRecipes.Add(new CategoryRecipe() { CategoryId = CategoryId, RecipeId = recipe.RecipeId });
        _db.SaveChanges();
      }
      return RedirectToAction("Index");
    }    

    public async Task<ActionResult> AddIngredient(int id)
    {

      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      var thisRecipe = _db.Recipes
      .Include(recipe => recipe.JoinIngredientRecipes)
      .FirstOrDefault(recipe => recipe.RecipeId == id);
      
      ViewBag.IngredientId = new SelectList(_db.Ingredients,"IngredientId", "Name");
      return View(thisRecipe);
    }    

    [HttpPost]
    public async Task<ActionResult> AddIngredient(Recipe recipe, int IngredientId, string Quantity)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      bool duplicate = _db.IngredientRecipes.Any(join => join.IngredientId == IngredientId && join.RecipeId == recipe.RecipeId);

      if (IngredientId != 0)
      {      
        if (duplicate)
        {
          ViewBag.SuccessMessage = "This Ingredient has already been added";
          ViewBag.IngredientId = new SelectList(_db.Ingredients,"IngredientId", "Name");
          return View();
        }
        else
        {
          ViewBag.SuccessMessage = "Not Duplicate";
          _db.IngredientRecipes.Add(new IngredientRecipe() { IngredientId = IngredientId, RecipeId = recipe.RecipeId, Quantity = Quantity, User = currentUser });
          _db.SaveChanges();
        }
      }
      return RedirectToAction("Index");
    }    

    public ActionResult Delete(int id)
    {
      var thisRecipe = _db.Recipes.FirstOrDefault(recipe => recipe.RecipeId == id);
      return View(thisRecipe);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisRecipe = _db.Recipes.FirstOrDefault(recipe => recipe.RecipeId == id);
      _db.Recipes.Remove(thisRecipe);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult DeleteCategory(int joinId)
    {
      var joinEntry = _db.CategoryRecipes.FirstOrDefault(entry => entry.CategoryRecipeId == joinId);
      _db.CategoryRecipes.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
    [HttpPost]
    public ActionResult DeleteIngredient(int joinId)
    {
      var joinEntry = _db.IngredientRecipes.FirstOrDefault(entry => entry.IngredientRecipeId == joinId);
      _db.IngredientRecipes.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}