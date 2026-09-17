using LogicPOS.ApiServer.DTOs;
using LogicPOS.ApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogicPOS.ApiServer.Controllers;

[AllowAnonymous]
[Route("tables")]
[Route("api/tables")]
public sealed class TablesController : ApiControllerBase
{
    // O Copilot irá sugerir a injeção do TablesService e os endpoints aqui dentro
}
