using GordonBeemingCom.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace GordonBeemingCom.Controllers;

[ApiController]
[AllowAnonymous]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("api/[controller]")]
public sealed class DeployController(ApplicationDbContext applicationDbContext,ILogger<DeployController> logger,IConfiguration configuration) : ControllerBase
{
    [HttpPost("database")]
    public Task<IActionResult> RunDatabaseMigration([FromBody]string databaseName)
    {
        return databaseName switch
        {
            "Application" => MigrateDatabase(applicationDbContext),
            _ => Task.FromResult<IActionResult>(BadRequest())
        };
    }

    [HttpPost("version")]
    public IActionResult GetVersion()
    {
        return Ok(configuration["COMMIT_HASH"]);
    }

    private async Task<IActionResult> MigrateDatabase(DbContext context)
    {
        try
        {
            await context.Database.MigrateAsync();
            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"An error occurred while migrating the database '{context.Database.GetDbConnection().Database}'.");
            return Conflict();
        }
    }
}
