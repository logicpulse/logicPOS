using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogicPOS.Api.Features.Company.GetCompanyInformations
{
    public class GetCompanyInformationsQuery: IRequest<ErrorOr<CompanyInformations>>
    {
    }
}
