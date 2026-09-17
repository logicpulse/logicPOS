using ErrorOr;
using LogicPOS.ApiServer.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace LogicPOS.ApiServer.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult FromErrorOr<T>(ErrorOr<T> result)
    {
        if (!result.IsError)
        {
            return Ok(result.Value);
        }

        return CreateProblem(result.Errors);
    }

    protected IActionResult CreateProblem(IReadOnlyList<Error> errors)
    {
        var problem = ApiProblemDetails.FromErrors(errors, HttpContext);
        return StatusCode(problem.Status, problem);
    }
}
