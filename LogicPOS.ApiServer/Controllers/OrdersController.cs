using LogicPOS.ApiServer.DTOs;
using LogicPOS.ApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogicPOS.ApiServer.Controllers;

[AllowAnonymous]
[Route("orders")]
[Route("api/orders")]
public sealed class OrdersController : ApiControllerBase
{
    // O Copilot irá sugerir a injeção do OrdersService e os endpoints aqui dentro
}
