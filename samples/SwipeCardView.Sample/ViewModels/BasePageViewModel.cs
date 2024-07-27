using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SwipeCardView.Sample.ViewModels;

public abstract class BasePageViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected void RaisePropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}