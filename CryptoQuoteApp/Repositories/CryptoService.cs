using System;
using System.Net.Http;
using System.Threading.Tasks;
using CryptoQuoteApp.Models;
using Newtonsoft.Json.Linq;

namespace CryptoQuoteApp.Repositories
{
    public class CryptoService : ICryptoService
    {
        private readonly HttpClient _httpClient;
        private static readonly string API_KEY = "7705b00f-9133-4f09-888d-b5b1ee6ffcde";

        public CryptoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", API_KEY);
            _httpClient.DefaultRequestHeaders.Add("Accepts", "application/json");
        }

        public async Task<CryptoQuote> GetCryptoQuoteAsync(string cryptoCode)
        {
            try
            {
                decimal usdPrice = 0;

                // Call CoinMarketCap API using HttpClient
                var coinMarketCapResponse = await MakeAPICall();
                var joResponse2 = JObject.Parse(coinMarketCapResponse);
                var arrayType = (JArray)joResponse2["data"];
                foreach (var item in arrayType)
                {
                    var symbol = item["symbol"].ToString();
                    if (symbol == cryptoCode)
                    {
                        var joResponse3 = JObject.Parse(item["quote"].ToString());
                        var joResponse4 = JObject.Parse(joResponse3["USDT"].ToString());
                        usdPrice = Convert.ToDecimal(joResponse4["price"].ToString());
                    }
                }

                // Call ExchangeRatesAPI using HttpClient
                var exchangeRatesUrl = "https://api.exchangeratesapi.io/latest?access_key=c1ccc443b7f00f0eb81971619af10de9";
                var exchangeRatesResponse = await _httpClient.GetFromJsonAsync<ExchangeRatesResponse>(exchangeRatesUrl);

                if (exchangeRatesResponse == null)
                    throw new Exception("Exchange rates not found.");

                return new CryptoQuote
                {
                    Code = cryptoCode,
                    USD = usdPrice,
                    EUR = usdPrice * exchangeRatesResponse.Rates["EUR"],
                    BRL = usdPrice * exchangeRatesResponse.Rates["BRL"],
                    GBP = usdPrice * exchangeRatesResponse.Rates["GBP"],
                    AUD = usdPrice * exchangeRatesResponse.Rates["AUD"]
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task<string> MakeAPICall()
        {
            var url = "https://pro-api.coinmarketcap.com/v1/cryptocurrency/listings/latest?start=1&limit=200&convert=USDT";
            var response = await _httpClient.GetStringAsync(url);
            return response;
        }
    }
}