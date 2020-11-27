using FluentAssertions;
using Metricaly.Infrastructure.Data;
using Metricaly.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationTests
{
    public class CreateApplicationServiceTests
    {
        private readonly ApplicationDbContext applicationDbContext;
        private readonly CreateApplicationService createApplicationService;

        private readonly string userId = Guid.NewGuid().ToString();
        private readonly string applicationName = "Test Application Name";

        public CreateApplicationServiceTests()
        {
            var dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                  .UseInMemoryDatabase(databaseName: "TestDb")
                  .Options;

            applicationDbContext = new ApplicationDbContext(dbOptions);
            createApplicationService = new CreateApplicationService(applicationDbContext, new ApiKeyGenerator());
        }

        [Test]
        public async Task ShouldCreateApplication()
        {
            var createApp = await createApplicationService.CreateAsync(applicationName, userId);

            var application = await applicationDbContext.Applications.FirstOrDefaultAsync(x => x.Id == createApp.Id);

            application.Id.Should().NotBeEmpty();
            application.Name.Should().Be(applicationName);
            application.UserId.Should().Be(userId);
            application.ApiKey.Should().NotBeNullOrWhiteSpace();
            application.Metrics.Should().HaveCount(0);
        }
    }
}