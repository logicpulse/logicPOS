using Gtk;
using LogicPOS.Settings;
using LogicPOS.UI;
using LogicPOS.UI.Buttons;
using System.Configuration;

namespace logicpos
{
    public static class Vision
    {
        public static void RunTest()
        {
            GeneralSettings.Settings = ConfigurationManager.AppSettings;
            Program.SetCulture();

            Application.Init();

            var win = new Window("Vision");

            VBox vBox = new VBox(false, 10);

            var button = new CustomButton(new ButtonSettings { Name=""});
            button.Label = "Hello, World!";
            button.ModifyText(StateType.Active, new Gdk.Color(23, 43, 23));

            var button2 = new CustomButton(new ButtonSettings { Name = "" });
            button2.Label = "Hello, World 2!";

            vBox.Add(button2);
            vBox.Add(button);
            vBox.ShowAll();

            win.Add(vBox);
          
            win.Show();


            Application.Run();
        }
    }
}
