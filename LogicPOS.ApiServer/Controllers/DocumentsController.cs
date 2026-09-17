using LogicPOS.ApiServer.DTOs;
using LogicPOS.ApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogicPOS.ApiServer.Controllers;

[AllowAnonymous]
[Route("documents")]
[Route("api/documents")]
public sealed class DocumentsController : ApiControllerBase
{
    // O Copilot irá sugerir a injeção do DocumentsService e os endpoints aqui dentro
}
