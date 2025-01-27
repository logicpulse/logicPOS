using LogicPOS.Api.Features.Users.GetUserNameById;
using LogicPOS.UI.Errors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LogicPOS.UI.Components.Users
{
    public static class UsersService
    {
        private static readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();

        public static string GetUserNameById(Guid id)
        {
            var getUserNameResult = _mediator.Send(new GetUserNameByIdQuery(id)).Result;

            if (getUserNameResult.IsError)
            {
                ErrorHandlingService.HandleApiError(getUserNameResult);
                return null;
            }

            return getUserNameResult.Value;
        }
    }
}
