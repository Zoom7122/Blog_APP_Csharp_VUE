using BlogAPP_BLL.Intarface;
using Microsoft.AspNetCore.Mvc;

namespace BlagAPP_MVC.Controllers
{
    public class EntaranceController : Controller
    {
        private readonly ILoginService _loginService;

        public EntaranceController(ILoginService loginService) 
        {
            _loginService = loginService;
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}
