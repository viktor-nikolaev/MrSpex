using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MrSpex.AppServices;
using MrSpex.Data;
using MrSpex.Domain;
using MrSpex.WebApp;
using Xunit;

namespace MrSpex.IntegrationTests
{
    public sealed class StockScenarios
    {
        private readonly HttpClient _client;
        private readonly AvailabilityDbContext _db;

        public StockScenarios()
        {
            WebApplicationFactory<Startup> factory = new();
            _client = factory.CreateClient();

            var serviceScope = factory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            _db = serviceScope.ServiceProvider.GetRequiredService<AvailabilityDbContext>();

            SeedDatabase();
        }

        [Theory]
        [InlineData("A10", "Loc1", 2)]
        public async Task Set_stock_when_not_exists(string sku, string location, int quantity)
        {
            var message = await PostStock(sku, location, quantity);

            // assert
            message.StatusCode.Should().Be(StatusCodes.Status200OK);

            var fromDb = _db.Stocks
                .FirstOrDefault(x => x.SKU == sku && x.Location == location && x.Quantity == quantity);

            fromDb.Should().NotBeNull();
        }

        [Theory]
        [InlineData("A10", "Loc1", 2)]
        public async Task Setting_stock_overrides_old_stock(string sku, string location, int quantity)
        {
            // arrange
            await PostStock(sku, location, 1);

            // act
            var message = await PostStock(sku, location, quantity);

            // assert
            message.StatusCode.Should().Be(StatusCodes.Status200OK);

            var fromDb = _db.Stocks
                .FirstOrDefault(x => x.SKU == sku && x.Location == location && x.Quantity == quantity);

            fromDb.Should().NotBeNull();
        }

        [Theory]
        [InlineData("A10", "Loc1", 2)]
        public async Task Setting_quantity_in_one_location_does_not_overwrite_another_location(string sku,
            string location, int quantity)
        {
            // arrange
            await PostStock(sku, location, 1);
            await PostStock(sku, "Loc2", 2);

            // act
            var message = await PostStock(sku, location, quantity);

            // assert
            message.StatusCode.Should().Be(StatusCodes.Status200OK);

            var fromLoc1 = _db.Stocks
                .FirstOrDefault(x => x.SKU == sku && x.Location == location && x.Quantity == quantity);

            var fromLoc2 = _db.Stocks
                .FirstOrDefault(x => x.SKU == sku && x.Location == "Loc2" && x.Quantity == 2);

            fromLoc1.Should().NotBeNull();
            fromLoc2.Should().NotBeNull();
        }

        [Fact]
        public async Task Setting_multiple_stock_in_multiple_locations()
        {
            // act
            var message = await PostMultipleStock(new[]
            {
                ("A10", "Loc1", 1),
                ("A10", "Loc2", 2),
                ("A20", "Loc2", 3)
            });

            // assert
            message.StatusCode.Should().Be(StatusCodes.Status200OK);

            var fromLoc1 = _db.Stocks
                .Where(x =>
                    x.SKU == "A10" && x.Location == "Loc1" && x.Quantity == 1 ||
                    x.SKU == "A10" && x.Location == "Loc2" && x.Quantity == 2 ||
                    x.SKU == "A20" && x.Location == "Loc2" && x.Quantity == 3
                )
                .ToList();

            fromLoc1.Should().HaveCount(3);
        }


        [Theory]
        [InlineData("A10", "XXnoexist", 2)]
        public async Task Setting_stock_in_nonexistant_location_in_any_sales_channel(string sku, string location,
            int quantity)
        {
            var message = await PostStock(sku, location, quantity);
            message.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }   
        
        [Theory]
        [InlineData("", "", -2)]
        public async Task Setting_stock_with_no_stock_set(string sku, string location, int quantity)
        {
            var message = await PostStock(sku, location, quantity);
            message.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        private async Task<HttpResponseMessage> PostStock(string sku, string location, int quantity)
        {
            var stockDto = new StockDto(location, sku, quantity);
            var command = new SetStock.Command(new[] {stockDto});

            return await _client.PostAsJsonAsync(Urls.SetStock, command);
        }

        private async Task<HttpResponseMessage> PostMultipleStock(
            IEnumerable<(string sku, string location, int quantity)> stock)
        {
            var stockDto = stock
                .Select(x => new StockDto(x.location, x.sku, x.quantity))
                .ToList();

            var command = new SetStock.Command(stockDto);

            return await _client.PostAsJsonAsync(Urls.SetStock, command);
        }

        private void SeedDatabase()
        {
            var salesChannel = new SalesChannel
            (
                "303f2cd8-0698-4ae9-adf8-cd22140903a9",
                new[] {"Loc1", "Loc2"}
            );

            if (!_db.SalesChannels.Any(x => x.SalesChannelId == salesChannel.SalesChannelId))
            {
                _db.SalesChannels.Add(salesChannel);
                _db.SaveChanges();
            }
        }
    }
}