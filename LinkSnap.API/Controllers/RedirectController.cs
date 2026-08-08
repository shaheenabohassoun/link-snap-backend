using Microsoft.AspNetCore.Mvc;
using LinkSnap.Application.Services;

namespace LinkSnap.API.Controllers
{
    [ApiController]
    [Route("{shortCode}")]
    public class RedirectController : ControllerBase
    {
        private readonly IShortUrlService _shortUrlService;

        public RedirectController(IShortUrlService shortUrlService)
        {
            _shortUrlService = shortUrlService;
        }

        [HttpGet]
        public async Task<IActionResult> RedirectToOriginal(string shortCode)
        {
            var originalUrl = await _shortUrlService.ResolveShortCodeAsync(
                shortCode,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers.UserAgent.ToString(),
                Request.Headers.Referer.ToString());

            return RedirectPermanent(originalUrl);
        }
    }
}
