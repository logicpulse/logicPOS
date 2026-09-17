using LogicPOS.ApiServer.DTOs;
using LogicPOS.ApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogicPOS.ApiServer.Controllers;

[AllowAnonymous]
[Route("holidays")]
[Route("api/holidays")]
public sealed class HolidaysController : ApiControllerBase
{
    // O Copilot irá sugerir a injeção do HolidaysService e os endpoints aqui dentro
}
