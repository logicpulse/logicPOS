using LogicPOS.Plugin.Abstractions;
using System;
using System.Collections.Generic;

namespace LogicPOS.Plugin.Utils
{
    public class PluginContainer : Dictionary<string, IPlugin>
    {
        public ICollection<IPlugin> Plugins { get; set; }

        public PluginContainer(string directory)
        {
            LoadPluginsFromDirectory(directory);

            if (Plugins != null)
            {
                foreach (IPlugin plugin in Plugins)
                {
                    RegisterPlugin(plugin);
                }
            }
        }

        private void LoadPluginsFromDirectory(string directory)
        {
            Plugins = PluginLoader<IPlugin>.LoadPlugins(directory);
        }

        private void RegisterPlugin(IPlugin plugin)
        {
            this.Add(plugin.GetType().ToString(), plugin);
        }

        public IPlugin GetPlugin(string pluginType)
        {
            return this.ContainsKey(pluginType) ? this[pluginType] : null;
        }

        public T GetFirstPluginOfType<T>()
        {
            foreach (var item in this)
            {
                if (item.Value is T plugin)
                {
                    return plugin;
                }
            }

            return default;
        }

        public Dictionary<string, IPlugin> GetPlugins(Type interfaceType)
        {
            Dictionary<string, IPlugin> result = new Dictionary<string, IPlugin>();

            foreach (var item in this)
            {
                Type[] interfaces = item.Value.GetType().GetInterfaces();

                foreach (var currentInterface in interfaces)
                {
                    if (currentInterface == interfaceType)
                    {
                        result.Add(item.Value.GetType().ToString(), (item.Value as IPlugin));
                    }
                }
            }

            return result;
        }
    }
}
