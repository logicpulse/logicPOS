using System;

namespace logicpos.plugin.contracts
{
    public interface IPlugin
    {
        string Name { get; }
        Type BaseType { get; }
        Type Interface { get ; }
        void Do();
    }
}
