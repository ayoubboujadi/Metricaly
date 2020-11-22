using FluentAssertions;
using Metricaly.Core.Interfaces;
using Metricaly.Infrastructure.Applications.Commands.CreateApplication;
using Metricaly.Infrastructure.Data;
using Metricaly.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IntegrationTests.Applications.Commands
{
    public class CreateApplicationTests
    {
        private readonly ApplicationDbContext applicationDbContext;
        private readonly ICreateApplicationService createApplicationService;
        private readonly ICurrentUserService currentUserService;

        private readonly string currentUserId = Guid.NewGuid().ToString();

        public CreateApplicationTests()
        {
            var dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
              .UseInMemoryDatabase(databaseName: "TestDb")
              .Options;

            applicationDbContext = new ApplicationDbContext(dbOptions);

            createApplicationService = new CreateApplicationService(applicationDbContext, new ApiKeyGenerator());


            var mock = new Mock<ICurrentUserService>();
            mock.Setup(x => x.GetCurrentUserId()).Returns(currentUserId);
            currentUserService = mock.Object;
        }


        [Test]
        public async Task ShouldCreateApplication()
        {
            var command = new CreateApplicationCommand()
            {
                ApplicationName = "MyApplication"
            };

            var guid = await new CreateApplicationCommandHandler(createApplicationService, currentUserService)
                .Handle(command, CancellationToken.None);

            var application = applicationDbContext.Applications.FirstOrDefault(x => x.Id == guid);

            application.Should().NotBeNull();
            application.Name.Should().Be("MyApplication");
            application.UserId.Should().Be(currentUserId);
        }

    }
}
