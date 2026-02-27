#nullable enable
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Plugin.Maui.SwipeCardView.Tests;

class MockViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public MockViewModel(string? text = null)
    {
        _text = text;
    }

    string? _text;
    public virtual string? Text
    {
        get { return _text; }
        set
        {
            if (_text == value)
                return;

            _text = value;
            OnPropertyChanged();
        }
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
