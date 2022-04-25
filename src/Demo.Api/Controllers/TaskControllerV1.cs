using Demo.Api.Controllers;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/tasks")]
public class TaskControllerV1 : Controller
{

    public TaskControllerV1()
    {

    }

    [HttpGet("")]
    public async Task<IActionResult> GetTasks()
    {
        await Task.Delay(100);
        throw new NotImplementedException();
    }

    [HttpPost("")]
    /// <param name="id" example="123">The product id</param>
    public async Task<IActionResult> AddTask(BusinessTask task)
    {
        BusinessTask? tasks = await FakeDB.Instance.AddTask(task);

        return Ok(task);
    }

}