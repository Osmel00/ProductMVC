using ProductBook.Models.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;



namespace ProductBook.Models.ViewModels
{
    public class ProductVM
    {
        public required Product Product { get; set; }
        [ValidateNever]
        public required IEnumerable<SelectListItem> CategoryList { get; set;}
    }
}
