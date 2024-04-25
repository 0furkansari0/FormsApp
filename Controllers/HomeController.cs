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
    public async Task<IActionResult> Create(Product model, IFormFile imageFile)
    {
           
        var extention ="";
        if(imageFile != null)
        {
            var allowExtentions = new []{".jpg",".jpeg",".png"};
            extention = Path.GetExtension(imageFile.FileName);

            if(!allowExtentions.Contains(extention))
            {
                ModelState.AddModelError("","Geçerli bir resim seçiniz.");
            }
        }

        if(ModelState.IsValid)
        {
            if(imageFile != null)
            {
                var randomFileName = string.Format($"{Guid.NewGuid().ToString()}{extention}");
                var path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/img",randomFileName);
                using(var stream = new FileStream(path,FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                model.Image = randomFileName;
                model.ProductId = Repository.Products.Count + 1;
                Repository.CreateProduct(model);
                return RedirectToAction("Index");
            }                      
        }
       
        ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
        return View(model);

    }

    public IActionResult Edit(int? id)
    {
        if(id==null)
        {
            return NotFound();
        }
        var entity = Repository.Products.FirstOrDefault(t=>t.ProductId == id);

        if(entity==null)
        {
            return NotFound();
        }
        ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
        return View(entity);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Product model, IFormFile? imageFile)
    {
        if(id != model.ProductId)
        {
            return NotFound();
        }

        if(ModelState.IsValid)
        {
            if(imageFile != null)
            {
               var extention = Path.GetExtension(imageFile.FileName);
                var randomFileName = string.Format($"{Guid.NewGuid().ToString()}{extention}");
                var path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/img",randomFileName);

                using(var stream = new FileStream(path,FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                model.Image = randomFileName;
            }
            Repository.EditProduct(model);
            return RedirectToAction("Index");
        }
        ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
        return View(model);
    }

    public IActionResult Delete(int? id)
    {
        if(id == null)
        {
            return NotFound();
        }

        var entitiy = Repository.Products.FirstOrDefault(t=>t.ProductId == id);
        if(entitiy == null)
        {
            return NotFound();
        }

        return View("DeleteConfirm", entitiy);

    }

    [HttpPost]
    public IActionResult Delete(int? id, int ProductId)
    {
        if(id != ProductId)
        {
            return NotFound();
        }

        var entitiy = Repository.Products.FirstOrDefault(t=>t.ProductId == ProductId);
        if(entitiy == null)
        {
            return NotFound();
        }

        Repository.DeleteProduct(entitiy);
        return RedirectToAction("Index");

    }

    public IActionResult EditProducts(List<Product> Products)
    {
        foreach(var product in Products)
        {
            Repository.EditIsActive(product);
        }
        return RedirectToAction("Index");
    }

}
