namespace LogicPOS.Application.Features.Finance.Saft;

public interface ISaftPtValidator
{
    SaftValidationReport Validate(ReadOnlyMemory<byte> xml);
}
