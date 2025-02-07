using CryptoQuoteApp.Repositories;
using Microsoft.AspNetCore.Mvc;
namespace CryptoQuoteApp.Controllers
{
    [Route("api/crypto")]
    public class CryptoController : ControllerBase
    {
        private readonly ICryptoService _cryptoService;

        public CryptoController(ICryptoService cryptoService)
        {
            _cryptoService = cryptoService;
        }

        [HttpGet("{cryptoCode}")]
        public async Task<IActionResult> GetQuote(string cryptoCode)
        {
            try
            {
                var quote = await _cryptoService.GetCryptoQuoteAsync(cryptoCode);
                return Ok(quote);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}