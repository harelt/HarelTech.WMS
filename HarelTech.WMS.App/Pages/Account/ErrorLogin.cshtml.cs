using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HarelTech.WMS.App.Pages
{
    public class ErrorLoginModel : PageModel
    {
        [BindProperty]
        public string Error { get; set; }
        public void OnGet(string error)
        {
            Error = error;
        }
    }
}