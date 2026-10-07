using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Enums;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities;

//[Table("cfg_configurationpreferenceparameter")]
public class PreferenceParameter : Entity.WithCode.AndOrder
{
    public string Token { get; set; } = null!;
    public string? Value { get; set; }
    public string? ValueTip { get; set; }
    public bool Required { get; set; }
    public string RegEx { get; set; } = null!;
    public string ResourceString { get; set; } = null!;
    public string? ResourceStringInfo { get; set; }
    public int FormType { get; set; }
    public int? FormPageNo { get; set; }
    public PreferenceParameterInputType InputType { get; set; }

    public async Task<Result> UpdateAsync(UpdatePreferenceParameterDto dto,
                                          IPreferenceParameterRepository repository,
                                          CancellationToken cancellationToken = default)
    {
        var updater = new PreferenceParameterUpdater(this, dto, repository);
        return await updater.UpdateAsync(cancellationToken);
    }

    public static async Task<Result<PreferenceParameter>> CreateAsync(CreatePreferenceParameterDto dto,
                                                                      IPreferenceParameterRepository repository,
                                                                      CancellationToken cancellationToken = default)
    {
        var creator = new PrefereneParameterCreator(dto, repository);
        return await creator.CreateAsync(cancellationToken);
    }
}
