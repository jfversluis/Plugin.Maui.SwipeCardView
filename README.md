![](nuget.png)

# SwipeCardView Control for .NET MAUI

SwipeCardView is a lightweight MVVM-friendly UI control that brings Tinder-style swipeable cards to .NET MAUI applications. It supports swiping in all directions while providing constant dragging feedback, which enables awesome interactivity.

This library is intended for anyone who wants to build swipeable card UI in C#. As it's built on top of .NET MAUI and uses no platform-specific code, it works on Android, iOS, macOS, Windows and any other .NET MAUI supported platform. If you are familiar with .NET MAUI `ListView`, `CollectionView` and `SwipeGestureRecognizer` interfaces, you will be able to utilize SwipeCardView with minimal effort.

It's highly customizable too, by giving you options to set supported directions, back card scale, rotation angle, looping behavior, and more.

![SwipeCardView Android TinderPage Like](docs/images/SwipeCardView_Android_TinderPage_Like.gif)
![SwipeCardView Android TinderPage SuperLike](docs/images/SwipeCardView_Android_TinderPage_SuperLike.gif)
![SwipeCardView Android TinderPage Nope](docs/images/SwipeCardView_Android_TinderPage_Nope.gif)
![SwipeCardView Android TinderPage Down](docs/images/SwipeCardView_Android_TinderPage_Down.gif)

## Install Plugin

[![NuGet](https://img.shields.io/nuget/v/Plugin.Maui.SwipeCardView.svg?label=NuGet)](https://www.nuget.org/packages/Plugin.Maui.SwipeCardView/)

Available on [NuGet](http://www.nuget.org/packages/Plugin.Maui.SwipeCardView).

Install with the dotnet CLI: `dotnet add package Plugin.Maui.SwipeCardView`, or through the NuGet Package Manager in Visual Studio.

### Platform Support

| Platform | Minimum Version |
|----------|----------------|
| Android | 5.0 (API 21) |
| iOS | 14.2 |
| macOS (Catalyst) | 14.0 |
| Windows | 10.0.17763.0 |

### Requirements

- .NET 10+ (for v1.0.0)
- Microsoft.Maui.Controls 10.0.41+

## Features

- **Data Source** – Populate a SwipeCardView with data using `ItemsSource`, with or without data binding. Full `ObservableCollection` support (Add, Remove, Replace, Move, Reset)
- **Card Appearance** – Customize the appearance of the cards using `ItemTemplate`. Supports any .NET MAUI view as the card root, including `Border`
- **Adjustability** – Customize the behavior of SwipeCardView. Set card rotation, animation length, back card scale, etc.
- **Interactivity** – Respond to dragging and swipe gestures using events or commands
- **Navigation** – Go back to previously swiped cards with `GoBack()`, optionally with animation
- **Programmatic Swiping** – Trigger swipes from code with `InvokeSwipe()`
- **Looping** – Enable infinite card looping with the `LoopCards` property
- **Card Stack** – Show stacked cards behind the top card with `StackDepth`, giving the visual illusion of a deck
- **Disposable** – Proper resource cleanup with `IDisposable` implementation

For more info about the features check out [the full documentation](docs/index.md).

## Quick Start

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:swipeCardView="clr-namespace:Plugin.Maui.SwipeCardView;assembly=Plugin.Maui.SwipeCardView">

    <swipeCardView:SwipeCardView
        ItemsSource="{Binding CardItems}"
        SwipedCommand="{Binding SwipedCommand}"
        VerticalOptions="FillAndExpand">
        <swipeCardView:SwipeCardView.ItemTemplate>
            <DataTemplate>
                <Label Text="{Binding .}"
                       FontSize="Large"
                       HorizontalTextAlignment="Center"
                       VerticalTextAlignment="Center"
                       BackgroundColor="Beige"/>
            </DataTemplate>
        </swipeCardView:SwipeCardView.ItemTemplate>
    </swipeCardView:SwipeCardView>
</ContentPage>
```

## API Reference

[SwipeCardView API](docs/api.md)

## Changelog

[See Change Log](docs/changelog.md)

## Samples

You can try all the samples by cloning this project and running the [sample app solution](samples/) in Visual Studio.

### Simple Page

The intention of this sample is to show how simple it is to start using SwipeCardView in your MVVM app. All you need is a collection of items and a command handler.

![SwipeCardView Android Simple Page](docs/images/SwipeCardView_Android_SimplePage.png)

```xml
<swipeCardView:SwipeCardView
    ItemsSource="{Binding CardItems}"
    SwipedCommand="{Binding SwipedCommand}"
    VerticalOptions="FillAndExpand">
    <swipeCardView:SwipeCardView.ItemTemplate>
        <DataTemplate>
            <Label Text="{Binding .}" FontSize="Large" HorizontalTextAlignment="Center" VerticalTextAlignment="Center" BackgroundColor="Beige"/>
        </DataTemplate>
    </swipeCardView:SwipeCardView.ItemTemplate>
</swipeCardView:SwipeCardView>
```

- Data source is `CardItems`, a list of strings defined in the bound ViewModel
- Card appearance is defined by a simple `DataTemplate`, which contains only a `Label`
- Various SwipeCardView properties are not being set, so the control is using default values
- `SwipedCommand` will be triggered when the card is swiped over threshold in any direction

### Colors Page

The intention of this sample is to demonstrate the SwipeCardView interactivity. Each color represents one `DraggingCardPosition`. Labels on cards represent the current values of `SwipeCardDirection` and `DraggingCardPosition`. Also demonstrates `GoBack()` (with animation), dynamic collection manipulation (Clear Items, Add Items), and the `PreviousItem` property.

![SwipeCardView Android Colors Page](docs/images/SwipeCardView_Android_ColorsPage.gif)

### Tinder Page

The intention of this sample is to replicate a Tinder-like UI, including programmatic swipes via `InvokeSwipe()`.

![SwipeCardView Android TinderPage Like](docs/images/SwipeCardView_Android_TinderPageLikeM.jpg)
![SwipeCardView Android TinderPage SuperLike](docs/images/SwipeCardView_Android_TinderPageSuperLikeM.jpg)
![SwipeCardView Android TinderPage Nope](docs/images/SwipeCardView_Android_TinderPageNopeM.jpg)

- Data source is `Profiles`, an `ObservableCollection<Profile>` defined in the bound ViewModel
- Card appearance uses `AbsoluteLayout` with overlay layers. Opacity of Like, Nope, and SuperLike frames are updated on `Dragging` event
- `SwipedCommand` will be triggered when the card is swiped over threshold
- All 4 dragging directions are supported, but swipe gesture is recognized only on Right, Left, and Up

### Customizable Page

The intention of this page is to demonstrate the use of all the properties of SwipeCardView. All properties can be updated at runtime using the UI controls below:

![SwipeCardView Android Customizable Page](docs/images/SwipeCardView_Android_CustomizablePage.png)

### Border Page

Demonstrates using a `Border` as the root element in a `DataTemplate`. This is useful when you want rounded corners, custom strokes, or other border styling on your cards.

```xml
<swipeCardView:SwipeCardView.ItemTemplate>
    <DataTemplate>
        <Border StrokeShape="RoundRectangle 20"
                Stroke="LightGray"
                StrokeThickness="2"
                BackgroundColor="{Binding Color}">
            <Label Text="{Binding Title}" />
        </Border>
    </DataTemplate>
</swipeCardView:SwipeCardView.ItemTemplate>
```

## GoBack / Navigate to Previous Card

SwipeCardView supports navigating back to the previously swiped card:

```csharp
// Go back without animation
swipeCardView.GoBack();

// Go back with a slide-in animation
swipeCardView.GoBack(animated: true);
```

The `PreviousItem` property provides access to the last swiped item. `GoBack()` returns `true` if navigation succeeded, or `false` if there is no previous card to go back to.

## Looping

Enable infinite card looping so the stack cycles back to the first card after reaching the end:

```xml
<swipeCardView:SwipeCardView
    ItemsSource="{Binding CardItems}"
    LoopCards="True" />
```

## Card Stack Visual Effect

Show stacked cards behind the top card to create the visual illusion of a deck. Set `StackDepth` to control how many cards are visible:

```xml
<swipeCardView:SwipeCardView
    ItemsSource="{Binding Profiles}"
    StackDepth="3"
    StackOffset="5"
    StackDirection="Top"
    StackScaleStep="0"
    BackCardScale="0.98" />
```

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `StackDepth` | `int` | `0` | Number of cards visible behind the top card. `0` = off (backward compatible), `1` = back card visible, `2` = back card + 1 shadow, etc. |
| `StackOffset` | `double` | `10` | Vertical offset (in dp) between each stacked card |
| `StackScaleStep` | `double` | `0.03` | Scale reduction per successive card (e.g., 0.02 = each card 2% smaller). Set to `0` for uniform-width strips. |
| `StackDirection` | `StackDirection` | `Bottom` | Direction stacked cards peek: `Bottom` (below top card) or `Top` (above top card) |

In non-stack mode (`StackDepth` = 0), the `BackCardScale` property controls the initial scale of the back card during drag. In stack mode (`StackDepth` > 0), card scaling is driven by `StackScaleStep` for visual consistency across all strips.

All stack properties can be changed at runtime and take effect immediately — no swipe action needed.

> **Note:** `StackDepth="0"` is fully backward compatible — the control behaves exactly as before.

## Migration From SwipeCardView for Xamarin.Forms

The migration should be fairly simple:

- Namespace change from `MLToolkit.Forms.SwipeCardView` to `Plugin.Maui.SwipeCardView`
- Requires .NET 10+ and .NET MAUI

## Acknowledgements

### Original Project MLToolkit.Forms.SwipeCardView

This project is forked from Marko Lazic's [MLToolkit.Forms.SwipeCardView project](https://github.com/markolazic88/SwipeCardView) and updated to be compatible with .NET MAUI.

Thank you Marko and contributors for doing the initial work on this amazing control!

### Icon

Although I used a [CC0 License](https://choosealicense.com/licenses/cc0-1.0/) icon, I still want to be transparent about where I got them and who made them. Credit where credit is due!

* Swipe library icon: https://www.svgrepo.com/svg/409928/swipe-left