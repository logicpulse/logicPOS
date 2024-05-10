using System;

namespace LogicPOS.Plugin.Abstractions
{
    public interface IPlugin
    {
        string Name { get; }
        Type BaseType { get; }
        Type Interface { get; }
        void Do();
    }
}
