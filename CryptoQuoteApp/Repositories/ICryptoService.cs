using System;
using CryptoQuoteApp.Models;

namespace CryptoQuoteApp.Repositories
{
	public interface ICryptoService
	{
        Task<CryptoQuote> GetCryptoQuoteAsync(string cryptoCode);

    }
}

