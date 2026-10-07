using System.ComponentModel;
using System.Runtime.CompilerServices;
using LogicPOS.Core.Authentication;

namespace LogicPOS.App.Views;

public sealed class UserSlot : INotifyPropertyChanged
{
    public UserSlot(UserOption? user)
    {
        User = user;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public UserOption? User { get; }

    public bool HasUser => User is not null;

    public string Name => User?.Name ?? string.Empty;

    private bool _isSelected;

    public bool IsSelected
    {
        get => _isSelected;
        set => SetField(ref _isSelected, value);
    }

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
