namespace LogicPOS.Core.Finance.Postal;

public enum PostalSearchTrigger
{
    /// <summary>Lost focus or equivalent: only a complete Portuguese code may call the API.</summary>
    AutoComplete,

    /// <summary>User asked for search or picker: complete code or a four-digit prefix.</summary>
    Explicit
}