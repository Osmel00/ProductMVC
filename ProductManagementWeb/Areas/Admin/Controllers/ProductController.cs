using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProductBook.DataAccess.Repository.IRepository;
using ProductBook.Models.Models;
using ProductBook.Models.ViewModels;

namespace ProductBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> products = _unitOfWork.ProductRepository.GetAll(includeProperties: "Category").ToList();
            return View(products);
        }
        public IActionResult Upsert(int? id)//Update  + Insert
        {
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.CategoryRepository.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString(),
                }),
                Product = new Product()
            };
            if(id==null || id == 0)
            {
                //creATE
                return View(productVM);
            } else
            {
                //Update
                productVM.Product = _unitOfWork.ProductRepository.Get(p => p.Id == id);
                return View(productVM);
            }
           
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM,IFormFile? file)//Update  + Insert
        {
          if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                      //delete the old image
                        var oldImagePath = 
                          Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName),FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                }

                if (productVM.Product.Id == 0) 
                {
                    _unitOfWork.ProductRepository.Add(productVM.Product);
                }else
                {
                    _unitOfWork.ProductRepository.Update(productVM.Product);
                }
                
                _unitOfWork.Save();
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");
            }else
            {

                productVM.CategoryList = _unitOfWork.CategoryRepository.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString(),
                });
                return View(productVM);
            }
        }
        
      
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> products = _unitOfWork.ProductRepository.GetAll(includeProperties: "Category").ToList();
            return Json(new {data = products });
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
           var productsToBeDeleted = _unitOfWork.ProductRepository.Get(c => c.Id == id);
            if (productsToBeDeleted==null)
            {
                return Json(new { success = false,message = "Error while deleting" });
            }

            //delete the old image
            var oldImagePath =
              Path.Combine(_webHostEnvironment.WebRootPath, productsToBeDeleted.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.ProductRepository.Remove(productsToBeDeleted);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete successful " });
        }
        #endregion
    }
}

