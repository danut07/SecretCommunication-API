using Microsoft.AspNetCore.Mvc;

namespace SecretCommunication_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageSteganography : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
