namespace LogicPOS.Persistence.Cloud;

public interface ICloudAccountCatalog
{
    CloudAccountSnapshot Find(string clientId);
}
