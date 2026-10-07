using System;

namespace LogicPOS.Application.Features.Finance.At;

public interface IAtSoapCredentialsBuilder
{
    public Task<AtSoapCredentials> BuildCredentialsAsync();
}
