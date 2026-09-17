using LogicPOS.ApiServer.DTOs;
using LogicPOS.ApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogicPOS.ApiServer.Controllers;

[AllowAnonymous]
[Route("places")]
[Route("api/places")]
public sealed class PlacesController : ApiControllerBase
{
    // O Copilot irá sugerir a injeção do PlacesService e os endpoints aqui dentro
}
