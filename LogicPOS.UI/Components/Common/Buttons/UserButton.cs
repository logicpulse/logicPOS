using LogicPOS.UI.Extensions;
using System.Drawing;

namespace LogicPOS.UI.Buttons
{
    public class UserButton : TextButton
    {
        public UserButton(ButtonSettings settings)
            : base(settings)
        {
            if (ButtonSettings.Logged)
            {
                ChangeFont(ButtonSettings.Font, Color.Green);

                if (ButtonSettings.BackgroundColor != Color.Transparent)
                {
                    ButtonSettings.BackgroundColor = settings.BackgroundColor.Lighten(0.50F);
                }
            }
            else
            {
                ChangeFont(ButtonSettings.Font, Color.Black);

                if (ButtonSettings.BackgroundColor != Color.Transparent)
                {
                    ButtonSettings.BackgroundColor = settings.BackgroundColor.Darken(0.50F);
                }
            }
        }


    }
}
