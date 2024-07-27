![](nuget.png)

# SwipeCardView Control for .NET MAUI

SwipeCardView is a lightweight MVVM friendly UI control that brings Tinder-style swipeable cards to .NET MAUI applications. It supports swiping in all directions while providing constant dragging feedback, which enables awesome interactivity.

This library is intended for anyone who wants to build swipeable UI in C#. As it's built on top of .NET MAUI and uses no platform-specific code, it works on Android, iOS and any other supported platforms. If you are familiar with .NET MAUI `ListView`, `CollectionView` and `SwipeGestureRecognizer` interfaces, you will be able to utilize SwipeCardView with minimal effort. 

It's highly customizable too, by giving you options to set supported directions, back card scale, rotation angle etc.

Have a look at the sample app in this repository, or screen recordings of the sample app below.

![SwipeCardView Android TinderPage Like](docs/images/SwipeCardView_Android_TinderPage_Like.gif)
![SwipeCardView Android TinderPage SuperLike](docs/images/SwipeCardView_Android_TinderPage_SuperLike.gif)
![SwipeCardView Android TinderPage Nope](docs/images/SwipeCardView_Android_TinderPage_Nope.gif)
![SwipeCardView Android TinderPage Down](docs/images/SwipeCardView_Android_TinderPage_Down.gif)

## Install Plugin

[![NuGet](https://img.shields.io/nuget/v/Plugin.Maui.SwipeCardView.svg?label=NuGet)](https://www.nuget.org/packages/Plugin.Maui.SwipeCardView/)

Available on [NuGet](http://www.nuget.org/packages/Plugin.Maui.SwipeCardView).

Install with the dotnet CLI: `dotnet add package Plugin.Maui.SwipeCardView`, or through the NuGet Package Manager in Visual Studio.

## Features

- Data Source – Populate a SwipeCardView with data using ItemSource, with or without data binding
- Card Appearance – Customize the appearance of the cards using ItemTemplate
- Adjustability – Customize the behavior of SwipeCardView. Set card rotation, animation length, back card scale etc.
- Interactivity – Respond to dragging and swipe gestures using events or commands

For more info about the features check out [the full documentation](docs/index.md).

## API

[SwipeCardView API](docs/api.md)

## Changelog

[Change Log - February 2020](docs/changelog.md)

## Samples

You can try all the samples by cloning this project and running the [sample app solution](samples/) in Visual Studio.

### Simple Page

The intention of this sample is to show how simple it is to start using SwipeCardView in your MVVM app. All you need is a collection of items and a command handler.

![SwipeCardView Android Simple Page](docs/images/SwipeCardView_Android_SimplePage.png)

```XML
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

- Data source is CardItems, a list of strings defined in the bound ViewModel
- Card appearance is defined by a simple DataTemplate, which contains only a Label
- Various SwipeCardView properties are not being set, so the control is using default values
- SwipedCommand will be triggered when the card is swiped over threshold in any direction

### Colors Page

The intention of this sample is to demonstrate the SwipeCardView interactivity. Each color represents one DraggingCardPosition. Labels on cards represent the current values of SwipeCardDirection and DraggingCardPosition.

![SwipeCardView Android Simple Page](docs/images/SwipeCardView_Android_ColorsPage.gif)

- Data source is CardItems, a list of strings defined in the bound ViewModel
- Card appearance is defined by a simple DataTemplate. DirectionLabel, PositionLabel and background color will be updated from code behind
- Various SwipeCardView properties are not being set, so the control is using default values
- SwipedCommand will be triggered when the card is swiped over threshold in any direction. Dragging event is being triggered while dragging gesture is being performed, which updates the card appearance

### Tinder Page

The intention of this sample is to replicate Tinder UI.

![SwipeCardView Android TinderPage Like](docs/images/SwipeCardView_Android_TinderPageLikeM.jpg)
![SwipeCardView Android TinderPage SuperLike](docs/images/SwipeCardView_Android_TinderPageSuperLikeM.jpg)
![SwipeCardView Android TinderPage Nope](docs/images/SwipeCardView_Android_TinderPageNopeM.jpg)

- Data source is Profiles, an ObservableCollection of Profiles defined in the bound ViewModel
- Card appearance is defined as a Frame with AbsoluteLayout containing all the necessary layers. Opacity LikeFrame, NopeFrame and SuperLikeFrame are updated on Dragging event
- Various SwipeCardView properties are not being set, so the control is using default values
- SwipedCommand will be triggered when the card is swiped over threshold. Dragging event is triggered while dragging gesture is being performed which updates the card appearance, as well as the scale of the control buttons
- Just like in Tinder UI, all 4 dragging directions are supported, but swipe gesture is recognized only on Right, Left and Up

### Customizable Page

The intention of this page is to demonstrate the use of all the properties of SwipeCardView. All properties can be updated in runtime using the UI controls below:

![SwipeCardView Android Simple Page](docs/images/SwipeCardView_Android_CustomizablePage.png)-->

## Migration From SwipeCardView for Xamarin.Forms

The migration should be fairly simple:

- Breaking change from MLToolkit.Forms.SwipeCardView to Plugin.Maui.SwipeCardView

## Acknowledgements

### Original Project MLToolkit.Forms.SwipeCardView

This project is forked from Marko Lazic's [MLToolkit.Forms.SwipeCardView project](https://github.com/markolazic88/SwipeCardView) and updated to be compatible with .NET MAUI.

Thank you Marko and contributors for doing the initial work on this amazing control!

### Icon

Although I used a [CC0 License](https://choosealicense.com/licenses/cc0-1.0/) icon, I still want to be transparent about where I got them and who made them. Credit where credit is due!

* Swipe library icon: https://www.svgrepo.com/svg/409928/swipe-left