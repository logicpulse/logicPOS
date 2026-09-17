using LogicPOS.ApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogicPOS.ApiServer.Controllers;

[AllowAnonymous]
[Route("users")]
[Route("api/users")]
public sealed class UsersController : ApiControllerBase
{
    private readonly UsersService _usersService;

    public UsersController(UsersService usersService)
    {
        _usersService = usersService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _usersService.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:guid}/name")]
    public async Task<IActionResult> GetName(Guid id, CancellationToken cancellationToken)
    {
        var name = await _usersService.GetNameAsync(id, cancellationToken);
        return name is null ? NotFound() : Ok(name);
    }

    [HttpGet("{id:guid}/permissions")]
    public async Task<IActionResult> GetPermissions(Guid id, CancellationToken cancellationToken)
    {
        return Ok(await _usersService.GetPermissionsAsync(id, cancellationToken));
    }
}
