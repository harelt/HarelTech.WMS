using HarelTech.WMS.RestClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HarelTech.WMS.App.ViewComponents
{
    public class BinsViewComponent: ViewComponent
    {
        private readonly IWmsClient _wmsClient;
        private readonly IMemoryCache _cache;

        public BinsViewComponent(IWmsClient wmsClient, IMemoryCache cache)
        {
            _wmsClient = wmsClient;
            _cache = cache;
        }

        public async Task<IViewComponentResult> InvokeAsync(string company, long warhouseId)
        {
            var key = $"{warhouseId}_bins";
            if (_cache.TryGetValue(key, out List<string> bins))
                return View(bins);

            bins = await _wmsClient.GetBins(company, warhouseId);
            if (bins != null && bins.Count > 0)
            {
                _cache.Set(key, bins, DateTime.Now.AddHours(2));
                return View(bins);
            }

            return View();
        }

    }
}
