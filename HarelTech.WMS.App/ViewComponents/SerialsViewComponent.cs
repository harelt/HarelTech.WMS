﻿using HarelTech.WMS.Common.Models;
using HarelTech.WMS.RestClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HarelTech.WMS.App.ViewComponents
{
    public class SerialsViewComponent: ViewComponent
    {
        private readonly IWmsClient _wmsClient;
        private readonly IMemoryCache _cache;

        public SerialsViewComponent(IWmsClient wmsClient, IMemoryCache cache)
        {
            _wmsClient = wmsClient;
            _cache = cache;
        }

        public async Task<IViewComponentResult> InvokeAsync(string company, long partId, long lot,
            string locName, long iTaskLot)
        {
            //var key = $"{partId}_{locName}_serials";
            //if (_cache.TryGetValue(key, out List<SerialModel> serials))
            //    return View(serials);

            var serials = await _wmsClient.GetSerials(new SerialsRequest { 
                Company = company,
                LocName = locName,
                PartId = partId,
                SerialId = lot
            });
            if(iTaskLot > 0)
            {
                var selected = await _wmsClient.GetSelectedSerials(company, iTaskLot);
                if(selected != null && selected.Count > 0)
                {
                    foreach (var item in selected)
                    {
                        serials.FirstOrDefault(w => w.Serial == item.Serial).AlradySelected = true;
                    }
                }
            }
            if (serials != null && serials.Count > 0)
            {
                serials = serials.OrderBy(o => o.AlradySelected).ToList();
            //    _cache.Set(key, serials, DateTime.Now.AddMinutes(5));
                return View(serials);
            }

            return View(new List<SerialModel>());
        }
    }
}
