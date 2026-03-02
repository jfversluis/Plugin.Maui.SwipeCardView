using Plugin.Maui.SwipeCardView.Core;
using SwipeCardView.Sample.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SwipeCardView.Sample.ViewModels;

public class TinderPageViewModel : BasePageViewModel
{
    private ObservableCollection<Profile> _profiles = new ObservableCollection<Profile>();

    private uint _threshold;

    public TinderPageViewModel()
    {
        InitializeProfiles();

        Threshold = (uint)(DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density / 3);

        SwipedCommand = new Command<SwipedCardEventArgs>(OnSwipedCommand);
        DraggingCommand = new Command<DraggingCardEventArgs>(OnDraggingCommand);

        ClearItemsCommand = new Command(OnClearItemsCommand);
        AddItemsCommand = new Command(OnAddItemsCommand);
    }

    public ObservableCollection<Profile> Profiles
    {
        get => _profiles;
        set
        {
            _profiles = value;
            RaisePropertyChanged();
        }
    }

    public uint Threshold
    {
        get => _threshold;
        set
        {
            _threshold = value;
            RaisePropertyChanged();
        }
    }

    public ICommand SwipedCommand { get; }

    public ICommand DraggingCommand { get; }

    public ICommand ClearItemsCommand { get; }

    public ICommand AddItemsCommand { get; }

    private void OnSwipedCommand(SwipedCardEventArgs eventArgs)
    {
    }

    private void OnDraggingCommand(DraggingCardEventArgs eventArgs)
    {
        switch (eventArgs.Position)
        {
            case DraggingCardPosition.Start:
                return;

            case DraggingCardPosition.UnderThreshold:
                break;

            case DraggingCardPosition.OverThreshold:
                break;

            case DraggingCardPosition.FinishedUnderThreshold:
                return;

            case DraggingCardPosition.FinishedOverThreshold:
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnClearItemsCommand()
    {
        Profiles.Clear();
    }

    private void OnAddItemsCommand()
    {
    }

    private void InitializeProfiles()
    {
        // Photos are from https://unsplash.com/. Name and Age values are fictional.

        Profiles.Add(new Profile { ProfileId = 1, Name = "Laura", Age = 24, Photo = "p705193.jpg", School = "UCLA", Distance = "3 miles away" });
        Profiles.Add(new Profile { ProfileId = 2, Name = "Sophia", Age = 21, Photo = "p597956.jpg", School = "Stanford University", Distance = "5 miles away" });
        Profiles.Add(new Profile { ProfileId = 3, Name = "Anne", Age = 19, Photo = "p497489.jpg", School = "MIT", Distance = "10 miles away" });
        Profiles.Add(new Profile { ProfileId = 4, Name = "Yvonne", Age = 27, Photo = "p467499.jpg", School = "NYU", Distance = "2 miles away" });
        Profiles.Add(new Profile { ProfileId = 5, Name = "Abby", Age = 25, Photo = "p589739.jpg", School = "Columbia University", Distance = "8 miles away" });
        Profiles.Add(new Profile { ProfileId = 6, Name = "Andressa", Age = 28, Photo = "p453095.jpg", School = "UC Berkeley", Distance = "12 miles away" });
        Profiles.Add(new Profile { ProfileId = 7, Name = "June", Age = 29, Photo = "p503001.jpg", School = "Yale University", Distance = "6 miles away" });
        Profiles.Add(new Profile { ProfileId = 8, Name = "Kim", Age = 22, Photo = "p627958.jpg", School = "Princeton", Distance = "4 miles away" });
        Profiles.Add(new Profile { ProfileId = 9, Name = "Denesha", Age = 26, Photo = "p474893.jpg", School = "Harvard University", Distance = "15 miles away" });
        Profiles.Add(new Profile { ProfileId = 10, Name = "Sasha", Age = 23, Photo = "p458914.jpg", School = "USC", Distance = "7 miles away" });

        Profiles.Add(new Profile { ProfileId = 11, Name = "Austin", Age = 28, Photo = "p378674.jpg", School = "Caltech", Distance = "9 miles away" });
        Profiles.Add(new Profile { ProfileId = 12, Name = "James", Age = 32, Photo = "p398931.jpg", School = "Duke University", Distance = "11 miles away" });
        Profiles.Add(new Profile { ProfileId = 13, Name = "Chris", Age = 27, Photo = "p401107.jpg", School = "Georgetown", Distance = "1 mile away" });
        Profiles.Add(new Profile { ProfileId = 14, Name = "Alexander", Age = 30, Photo = "p731150.jpg", School = "Brown University", Distance = "20 miles away" });
        Profiles.Add(new Profile { ProfileId = 15, Name = "Steve", Age = 31, Photo = "p327144.jpg", School = "Northwestern", Distance = "14 miles away" });
    }
}