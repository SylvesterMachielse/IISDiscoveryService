using IISDiscoveryService.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IISDiscoveryService.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class WebsitesController : Controller
    {
        private readonly IISWebsiteProvider _iisWebsiteProvider;

        public WebsitesController(IISWebsiteProvider iisWebsiteProvider)
        {
            _iisWebsiteProvider = iisWebsiteProvider;
        }

        [HttpGet]
        public ActionResult Get()
        {
            var websites = _iisWebsiteProvider.Provide();
            
            return Json(websites);
        }
    }
}
