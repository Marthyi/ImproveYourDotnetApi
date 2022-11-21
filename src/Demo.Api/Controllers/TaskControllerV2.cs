using Demo.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/tasks")]
public class TaskControllerV2 : TaskControllerV1
{
    static ActivitySource s_source = new ActivitySource("Sample");

    [HttpGet("")]
    public async Task<IActionResult> GetTasks()
    {
        using Activity? activity = s_source.StartActivity("Get Tasks");

        IReadOnlyList<BusinessTask>? tasks = await FakeDB.Instance.GetAllAsync();

        return new OkObjectResult(tasks);
    }
}