using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FormsApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FormsApp.Controllers;

public class HomeController : Controller
{
    public HomeController()
    {
        
    }

    [HttpGet]
    public IActionResult Index(string searchString, string category)
    {
        var products = Repository.Products;

        if(!string.IsNullOrEmpty(searchString))
        {
            ViewBag.SearchString=searchString;
            products = products.Where(t=>t.Name.ToLower().Contains(searchString.ToLower())).ToList();
        }

        if(!string.IsNullOrEmpty(category) && category != "0")
        {
            products = products.Where(t=>t.CategoryId == int.Parse(category)).ToList();
        }
        
        var model = new ProductViewModel{
            Products=products,
            Categories=Repository.Categories,
            SelectedCategory = category
        };

        //ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name",category);

        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
        return View();
    }

    [HttpPost]
    public IActionResult Create(Product model)
    {
        if(ModelState.IsValid)
        {
            model.ProductId = Repository.Products.Count + 1;
            Repository.CreateProduct(model);
            return RedirectToAction("Index");
        }
       
        ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
        return View(model);

    }

}
