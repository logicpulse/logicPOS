using LogicPOS.UI.Extensions;
using System.Drawing;

namespace LogicPOS.UI.Buttons
{
    public class UserButton : TextButton
    {
        public UserButton(ButtonSettings settings)
            : base(settings)
        {
            if (_settings.Logged)
            {
                ChangeFont(_settings.Font, Color.Green);

                if (_settings.BackgroundColor != Color.Transparent)
                {
                    _settings.BackgroundColor = settings.BackgroundColor.Lighten(0.50F);
                }
            }
            else
            {
                ChangeFont(_settings.Font, Color.Black);

                if (_settings.BackgroundColor != Color.Transparent)
                {
                    _settings.BackgroundColor = settings.BackgroundColor.Darken(0.50F);
                }
            }
        }


    }
}
