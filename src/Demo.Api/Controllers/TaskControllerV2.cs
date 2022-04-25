using Demo.Api.Controllers;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/tasks")]
public class TaskController : Controller
{


    [HttpGet("")]
    public async Task<IActionResult> GetTasks()
    {
        IReadOnlyList<BusinessTask>? tasks = await FakeDB.Instance.GetAllAsync();

        return new OkObjectResult(tasks);
    }
}