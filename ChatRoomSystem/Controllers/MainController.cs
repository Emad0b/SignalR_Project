using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ChatRoomSystem.Data;
using ChatRoomSystem.Models;

namespace ChatRoomSystem.Controllers
{
    public class MainController : Controller
    {
        private readonly MyProjectContext _context;

        public MainController(MyProjectContext context)
        {
            _context = context;
        }

        // GET: Main
        public IActionResult Index()
        {
            return View();
        }



        
    }
}
