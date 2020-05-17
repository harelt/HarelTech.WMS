using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarelTech.WMS.RestClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;

namespace HarelTech.WMS.App
{
    public class CompanyModel : PageModel
    {
        private readonly IWmsClient _wmsClient;
        private readonly IMemoryCache _cache;
        [BindProperty]
        public string Company { get; set; }
        [BindProperty]
        public long Warhouse { get; set; }
        public CompanyModel(IWmsClient wmsClient, IMemoryCache cache)
        {
            _wmsClient = wmsClient;
            _cache = cache;
        }
        public void OnGet()
        {
            
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _cache.Set("15_company", Company, DateTime.Now.AddHours(8));
            _cache.Set("15_warhouseId", Warhouse, DateTime.Now.AddHours(8));
            return await Task.FromResult(RedirectToPage("../Index"));
        }
    }
}