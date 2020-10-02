using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Metricaly.Core.Entities;
using Metricaly.Core.Interfaces;
using Metricaly.Infrastructure.Data;
using Metricaly.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Metricaly.Angular.Controllers
{
    public class ApplicationDto
    {

    }

    public class ApplicationCreateRequest
    {
        [Required]
        public string Name { get; set; }
    }


    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ApplicationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext applicationDbContext;
        private readonly ICreateApplicationService createApplicationService;

        public ApplicationController(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager, ICreateApplicationService createApplicationService)
        {
            this.applicationDbContext = applicationDbContext;
            this.userManager = userManager;
            this.createApplicationService = createApplicationService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Application>>> List()
        {
            var currentUserEmail = User.FindFirst(ClaimTypes.Email).Value;
            ApplicationUser user = await userManager.FindByEmailAsync(currentUserEmail);

            var applications = await applicationDbContext.Applications
                .Where(a => a.UserId == user.Id)
                .AsNoTracking()
                .ToListAsync();

            // TODO: Automap this
            return applications;
        }

        [HttpPost]
        public async Task Create([FromBody]ApplicationCreateRequest applicationCreateRequest)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            await createApplicationService.CreateAsync(applicationCreateRequest.Name, userId);
        }
    }
}
