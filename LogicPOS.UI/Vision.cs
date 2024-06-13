using Gtk;
using logicpos.Classes.Gui.Gtk.Widgets;
using LogicPOS.Settings;
using System.Configuration;

namespace logicpos
{
    public static class Vision
    {
        public static void RunTest()
        {
            GeneralSettings.Settings = ConfigurationManager.AppSettings;
            Program.SetCulture();

            Gtk.Application.Init();

            var win = new Window("Vision");
            MoneyPad moneyPad = new MoneyPad(win);
            moneyPad.Show();
            win.Show();

            

            Gtk.Application.Run();
        }


    }
}
