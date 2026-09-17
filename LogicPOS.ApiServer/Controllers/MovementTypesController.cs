using LogicPOS.ApiServer.DTOs;
using LogicPOS.ApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogicPOS.ApiServer.Controllers;

[AllowAnonymous]
[Route("movementtypes")]
[Route("api/movementtypes")]
public sealed class MovementTypesController : ApiControllerBase
{
    // O Copilot irá sugerir a injeção do MovementTypesService e os endpoints aqui dentro
}
