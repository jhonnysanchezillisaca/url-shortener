using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Models;

namespace UrlShortener.Controllers
{
    [Route("api/shorten")]
    [ApiController]
    public class ShortenedUrlController : ControllerBase
    {
        private readonly UrlShortenerContext _context;

        public ShortenedUrlController(UrlShortenerContext context)
        {
            _context = context;
        }

        // GET: Catch all remaining routes
        [Route("/{**url}")]
        [HttpGet(Order = int.MaxValue)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult CatchAll(string url)
        {
            var shortenedUrl = _context.ShortenedUrls.Find(url);

            if (shortenedUrl == null)
            {
                return NotFound();
            }

            return RedirectPermanent(shortenedUrl.Url);
        }

        // POST: api/shorten
        [HttpPost]
        public async Task<ActionResult<ShortenedUrl>> PostShortenedUrlEntry(string url)
        {
            if (url == null || !Uri.TryCreate(url, UriKind.Absolute, out Uri _))
            {
                return BadRequest("Invalid url");
            }

            var shortenedUrl = new ShortenedUrl(GenerateId(), url);

            _context.ShortenedUrls.Add(shortenedUrl);
            await _context.SaveChangesAsync();

            return Ok(shortenedUrl.Id);
        }

        private string GenerateId()
        {
            var id = GenerateRandomString(10);

            if (ExistsId(id))
            {
                GenerateId();
            }

            return id;
        }

        private bool ExistsId(string id)
        {
            return _context.ShortenedUrls.Any(e => e.Id == id);
        }

        public static string GenerateRandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[new Random().Next(s.Length)]).ToArray());
        }
    }
}
