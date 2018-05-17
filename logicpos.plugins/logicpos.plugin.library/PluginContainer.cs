using logicpos.plugin.contracts;
using System;
using System.Collections.Generic;

namespace logicpos.plugin.library
{
    public class PluginContainer : Dictionary<string, IPlugin>
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ICollection<IPlugin> _plugins;
        public ICollection<IPlugin> Plugins { get => _plugins; set => _plugins = value; }

        public PluginContainer(string directory)
        {
            // Get Plugins
            _plugins = GenericPluginLoader<IPlugin>.LoadPlugins(directory);

            // Proptect if dont have Plugins in PluginDirectory
            if (_plugins != null)
            {
                // Add Plugins to PluginContainer
                foreach (IPlugin item in _plugins)
                {
                    // Register Pluggin
                    this.Add(item.GetType().ToString(), item);
                }
            }
        }

        // Get Pluggin Use With:
        // GlobalFramework.PluginSoftwareVendor = (GlobalFramework.PluginContainer.GetPlugin("logicpulse.licencemanager.plugin.LogicpulseLicenceManagerPlugin") as ISoftwareVendor);
        public IPlugin GetPlugin(string plugginType)
        {
            IPlugin result = null;

            if (this.ContainsKey(plugginType))
            {
                return this[plugginType];
            }

            return result;
        }

        // Get First Pluggin Use with : 
        // ILicenceManager licenceManager = (GlobalFramework.PluginContainer.GetFirstPluginOfType<ILicenceManager>());
        public T GetFirstPluginOfType<T>()
        {
            List<T> founded = new List<T>();
            
            foreach (var item in this)
            {
                //_log.Debug(String.Format("GetFirstPluginOfType: {0}", item));

                if (item.Value is T) {
                    // Add To founded plugins
                    founded.Add((T) item.Value);
                };
            }

            // Detect more than one plugin of sme time
            if (founded.Count > 1)
            {
                _log.Debug(String.Format("Warning more than on plugin of type [{0}] founded, please delete one! Used first founded [{1}]", typeof(T), founded[0]));
            }

            // Always return first Founded
            return (founded.Count > 0) ? founded[0] : default(T);
        }

        // Get Pluggins of Interface Type
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
