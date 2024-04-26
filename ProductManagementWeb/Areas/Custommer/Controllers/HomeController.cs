using Microsoft.AspNetCore.Mvc;
using ProductBook.DataAccess.Repository;
using ProductBook.DataAccess.Repository.IRepository;
using ProductBook.Models;
using ProductBook.Models.Models;
using System.Diagnostics;

namespace ProductBookWeb.Areas.Custommer.Controllers
{
    [Area("Custommer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(IUnitOfWork unitOfWork, ILogger<HomeController> logger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.ProductRepository.GetAll(includeProperties: "Category").ToList();
            return View(productList);
        }

        public IActionResult Details(int productId)
        {
            Product product = _unitOfWork.ProductRepository.Get(x => x.Id == productId, includeProperties: "Category");
            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
