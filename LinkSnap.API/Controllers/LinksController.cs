using LinkSnap.Application.DTOs;
using LinkSnap.Application.Interfaces;
using LinkSnap.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LinkSnap.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LinksController : ControllerBase
    {
        private readonly IShortUrlService _shortUrlService;
        private readonly IAnalyticsService _analyticsService;

        public LinksController(IShortUrlService shortUrlService, IAnalyticsService analyticsService)
        {
            _shortUrlService = shortUrlService;
            _analyticsService = analyticsService;
        }

        [HttpPost("shorten")]
        public async Task<IActionResult> Shorten([FromBody] ShortenRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _shortUrlService.CreateShortUrlAsync(request, userId);
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetMyLinks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var links = await _shortUrlService.GetUserLinksAsync(userId);
            return Ok(links);
        }

        [Authorize]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetLink(Guid id)
        {
            var link = await _shortUrlService.GetLinkByIdAsync(id);
            if (link == null)
                return NotFound();
            return Ok(link);
        }

        [Authorize]
        [HttpGet("{id:guid}/analytics")]
        public async Task<IActionResult> GetAnalytics(Guid id)
        {
            var analytics = await _analyticsService.GetLinkAnalyticsAsync(id);
            return Ok(analytics);
        }

        [Authorize]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateLink(Guid id, [FromBody] UpdateLinkDto request)
        {
            var updated = await _shortUrlService.UpdateLinkAsync(id, request);
            return Ok(updated);
        }

        [Authorize]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteLink(Guid id)
        {
            await _shortUrlService.DeleteLinkAsync(id);
            return NoContent();
        }
    }
}
