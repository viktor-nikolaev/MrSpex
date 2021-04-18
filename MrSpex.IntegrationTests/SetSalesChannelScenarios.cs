using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MrSpex.AppServices;
using MrSpex.Data;
using MrSpex.Domain;
using MrSpex.WebApp;
using Xunit;

namespace MrSpex.IntegrationTests
{
    public sealed class SetSalesChannelScenarios
    {
        private readonly HttpClient _client;
        private readonly AvailabilityDbContext _db;

        public SetSalesChannelScenarios()
        {
            WebApplicationFactory<Startup> factory = new();
            _client = factory.CreateClient();

            var serviceScope = factory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            _db = serviceScope.ServiceProvider.GetRequiredService<AvailabilityDbContext>();

            _db.Database.ExecuteSqlRaw("TRUNCATE TABLE \"SalesChannels\";");
        }

        [Theory]
        [InlineData("DE", new[] {"Loc1", "Loc2"})]
        public async Task Creating_sales_channel_with_one_location_when_there_is_none(string salesChannelId,
            string[] locations)
        {
            var message = await PostSalesChannel(salesChannelId, locations);

            // assert
            message.StatusCode.Should().Be(StatusCodes.Status200OK);

            var fromDb = _db.SalesChannels.FirstOrDefault(x => x.SalesChannelId == salesChannelId);

            fromDb.Should().NotBeNull();

            fromDb!.Locations.Should().BeEquivalentTo(locations);
        }

        [Theory]
        [InlineData("DE", new[] {"Loc1", "Loc2"})]
        public async Task Overwriting_existing_sales_channel_adding_a_location(string salesChannelId,
            string[] locations)
        {
            // arrange
            await PostSalesChannel(salesChannelId, new[] {"Loc1"});

            var message = await PostSalesChannel(salesChannelId, locations);

            // assert
            message.StatusCode.Should().Be(StatusCodes.Status200OK);

            var fromDb = _db.SalesChannels.FirstOrDefault(x => x.SalesChannelId == salesChannelId);

            fromDb.Should().NotBeNull();

            fromDb!.Locations.Should().BeEquivalentTo(locations);
        }

        [Theory]
        [InlineData("DE", new[] {"Loc1"})]
        public async Task Overwriting_existing_sales_channel_removing_a_location(string salesChannelId,
            string[] locations)
        {
            // arrange
            await PostSalesChannel(salesChannelId, new[] {"Loc1", "Loc2"});

            var message = await PostSalesChannel(salesChannelId, locations);

            // assert
            message.StatusCode.Should().Be(StatusCodes.Status200OK);

            var fromDb = _db.SalesChannels.FirstOrDefault(x => x.SalesChannelId == salesChannelId);

            fromDb.Should().NotBeNull();

            fromDb!.Locations.Should().BeEquivalentTo(locations);
        }
        
        [Theory]
        [InlineData("DE", null)]
        public async Task  No_locations_causes_Bad_Request(string salesChannelId, string[] locations)
        {
            var message = await PostSalesChannel(salesChannelId, locations);
            message.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }
        private async Task<HttpResponseMessage> PostSalesChannel(string salesChannelId, string[] locations)
        {
            var command = new SetSalesChannel.Command(salesChannelId, locations);
            return await _client.PostAsJsonAsync(Urls.SetSalesChannel, command);
        }
    }
}