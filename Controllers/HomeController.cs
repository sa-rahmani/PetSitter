﻿using Microsoft.AspNetCore.Mvc;
using PetSitter.Data;
using PetSitter.Models;
using System.Diagnostics;

namespace PetSitter.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PetSitterContext _db;

        public HomeController(ILogger<HomeController> logger, PetSitterContext context)
        {
            _logger = logger;
            _db = context;
        }

        public IActionResult Index()
        {
            IQueryable<Pet> pets = from pts in _db.Pets
                                   select pts;
            return View(pets);
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