using LogicPOS.ApiServer.DTOs;
using LogicPOS.ApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogicPOS.ApiServer.Controllers;

[AllowAnonymous]
[Route("receipts")]
[Route("api/receipts")]
public sealed class ReceiptsController : ApiControllerBase
{
    // O Copilot irá sugerir a injeção do ReceiptsService e os endpoints aqui dentro
}
