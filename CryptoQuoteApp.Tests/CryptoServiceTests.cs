using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CryptoQuoteApp.Models;
using CryptoQuoteApp.Repositories;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

namespace CryptoQuoteApp.Tests
{
    public class CryptoServiceTests
    {
        [Fact]
        public async Task GetCryptoQuoteAsync_ValidCryptoCode_ReturnsCorrectQuote()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();
            var coinMarketCapResponse = @"
            {
                ""data"": [
                    {
                        ""symbol"": ""BTC"",
                        ""quote"": {
                            ""USDT"": {
                                ""price"": 50000.00
                            }
                        }
                    }
                ]
            }";
            var exchangeRatesResponse = new ExchangeRatesResponse
            {
                Rates = new Dictionary<string, decimal>
                {
                    { "EUR", 0.85m },
                    { "BRL", 5.5m },
                    { "GBP", 0.75m },
                    { "AUD", 1.3m }
                }
            };

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(coinMarketCapResponse)
                })
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object);
            var cryptoService = new CryptoService(httpClient);

            // Act
            var result = await cryptoService.GetCryptoQuoteAsync("BTC");

            // Assert
            result.Should().NotBeNull();
            result.Code.Should().Be("BTC");
            result.USD.Should().Be(50000.00m);
            result.EUR.Should().Be(50000.00m * 0.85m);
            result.BRL.Should().Be(50000.00m * 5.5m);
            result.GBP.Should().Be(50000.00m * 0.75m);
            result.AUD.Should().Be(50000.00m * 1.3m);
        }

        [Fact]
        public async Task GetCryptoQuoteAsync_InvalidCryptoCode_ReturnsNull()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();
            var coinMarketCapResponse = @"
            {
                ""data"": [
                    {
                        ""symbol"": ""BTC"",
                        ""quote"": {
                            ""USDT"": {
                                ""price"": 50000.00
                            }
                        }
                    }
                ]
            }";

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(coinMarketCapResponse)
                });

            var httpClient = new HttpClient(handlerMock.Object);
            var cryptoService = new CryptoService(httpClient);

            // Act
            var result = await cryptoService.GetCryptoQuoteAsync("XYZ");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetCryptoQuoteAsync_ApiError_ReturnsNull()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("API error"));

            var httpClient = new HttpClient(handlerMock.Object);
            var cryptoService = new CryptoService(httpClient);

            // Act
            var result = await cryptoService.GetCryptoQuoteAsync("BTC");

            // Assert
            result.Should().BeNull();
        }
    }
}