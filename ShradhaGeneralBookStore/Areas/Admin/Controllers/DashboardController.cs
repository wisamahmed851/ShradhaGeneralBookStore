﻿using Microsoft.AspNetCore.Mvc;
using ShradhaGeneralBookStore.Filters;

namespace ShradhaGeneralBookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();

        }
    }
}
