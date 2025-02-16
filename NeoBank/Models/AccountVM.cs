using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NeoBank.Models
{
    public class AccountVM
    {
        public Account Account { get; set; }
        //[ValidateNever]
        //public IEnumerable<SelectListItem> CategoryList { get; set; }
    }
}
