using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarelTech.WMS.App.Models;
using HarelTech.WMS.Common.Models;
using HarelTech.WMS.RestClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public async Task<IActionResult> OnGet(string companies)
        {
            var cmps =  System.Text.Json.JsonSerializer.Deserialize<List<Company>>(companies);
            //cmps.Insert(0, new Common.Models.Company { dname = "", title = "Select" });

            ViewData["Companies"] = new SelectList(cmps, "dname", "title");
            var wrhs = await _wmsClient.GetUserWarhousesAsync(cmps[0].dname, Utilities.UserId(User.Claims));

            ViewData["Warhouses"] = new SelectList(wrhs.OrderByDescending(o => o.IsDefault), "warhs", "warhsdes");
            _cache.Set($"{Utilities.UserId(User.Claims)}_companies", companies, DateTime.Now.AddHours(8));
            return await Task.FromResult(Page());
        }

        public async Task<IActionResult> OnGetWarhouses(string company)
        {
            var wrhs = await _wmsClient.GetUserWarhousesAsync(company, Utilities.UserId(User.Claims));

            return new JsonResult(new { success = true, warhouses = wrhs.OrderByDescending(o => o.IsDefault) });
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _cache.Set($"{Utilities.UserId(User.Claims)}_company", Company);
            _cache.Set($"{Utilities.UserId(User.Claims)}_warhouseId", Warhouse);
            _cache.Set($"{Utilities.UserId(User.Claims)}_companyName", Request.Form["CompanyName"].ToString());
            _cache.Set($"{Utilities.UserId(User.Claims)}_warhouseName", Request.Form["WarhouseName"].ToString());
            return await Task.FromResult(RedirectToPage("../Index"));
        }

        public async Task<IActionResult> OnGetCompanies()
        {
            var companies = _cache.Get<string>($"{Utilities.UserId(User.Claims)}_companies");
            return await Task.FromResult(RedirectToAction("OnGet", new { companies }));
        }
    }
}