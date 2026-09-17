using LogicPOS.ApiServer.DTOs;
using LogicPOS.ApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogicPOS.ApiServer.Controllers;

[AllowAnonymous]
[Route("articles")]
[Route("api/articles")]
public sealed class StocksController : ApiControllerBase
{
    // O Copilot irá sugerir a injeção do StocksService e os endpoints aqui dentro
}
