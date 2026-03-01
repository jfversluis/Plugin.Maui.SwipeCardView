#nullable enable
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plugin.Maui.SwipeCardView.Core;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Plugin.Maui.SwipeCardView.Tests;

[TestClass]
public class UnitTests : TestBase
{
    private static DataTemplate CreateSimpleTemplate()
    {
        return new DataTemplate(() =>
        {
            var stackLayout = new StackLayout();
            var label = new Label();
            label.SetBinding(Label.TextProperty, ".");
            stackLayout.Children.Add(label);
            return stackLayout;
        });
    }

    [TestMethod]
    public async Task Swipe_EmptyObservableCollection_ShouldNotInvoke()
    {
        var cardItems = new ObservableCollection<string>();
        var swipeCardView = new SwipeCardView();
        var swipeCardDirection = SwipeCardDirection.None;

        swipeCardView.ItemsSource = cardItems;
        swipeCardView.Swiped += (sender, args) => { swipeCardDirection = args.Direction; };

        await swipeCardView.InvokeSwipe(SwipeCardDirection.Right);

        Assert.AreEqual(swipeCardDirection, SwipeCardDirection.None);
        Assert.AreEqual(swipeCardView.ItemsSource.Count, 0);
    }

    [TestMethod]
    public async Task Swipe_ObservableCollection_UpdatesTopItem()
    {
        var swipeCardView = new SwipeCardView { ItemTemplate = CreateSimpleTemplate() };
        swipeCardView.ItemsSource = new ObservableCollection<string>() { "Item1", "Item2" };

        var swipeCardDirection = SwipeCardDirection.None;
        swipeCardView.Swiped += (sender, args) => { swipeCardDirection = args.Direction; };
        var initialTopItem = swipeCardView.TopItem;

        await swipeCardView.InvokeSwipe(SwipeCardDirection.Right);

        var afterSwipeTopItem = swipeCardView.TopItem;

        Assert.AreEqual(swipeCardDirection, SwipeCardDirection.Right);
        Assert.AreEqual(swipeCardView.ItemsSource.Count, 2);
        Assert.AreNotEqual(initialTopItem, afterSwipeTopItem);
        Assert.AreEqual(initialTopItem, "Item1");
        Assert.AreEqual(afterSwipeTopItem, "Item2");
    }

    [TestMethod]
    public async Task Swipe_SetObservableCollectionTwice()
    {
        var swipeCardView = new SwipeCardView { ItemTemplate = CreateSimpleTemplate() };

        swipeCardView.ItemsSource = new ObservableCollection<string>() { "Item1", "Item2" };
        swipeCardView.ItemsSource = new ObservableCollection<string>() { "Item3", "Item4" };

        var swipeCardDirection = SwipeCardDirection.None;
        swipeCardView.Swiped += (sender, args) => { swipeCardDirection = args.Direction; };
        var initialTopItem = swipeCardView.TopItem;

        await swipeCardView.InvokeSwipe(SwipeCardDirection.Right);

        var afterSwipeTopItem = swipeCardView.TopItem;

        Assert.AreEqual(swipeCardDirection, SwipeCardDirection.Right);
        Assert.AreEqual(swipeCardView.ItemsSource.Count, 2);
        Assert.AreNotEqual(initialTopItem, afterSwipeTopItem);
        Assert.AreEqual(initialTopItem, "Item3");
        Assert.AreEqual(afterSwipeTopItem, "Item4");
    }

    [TestMethod]
    public void ItemsSourceBeforeItemTemplate_ShouldNotCrash()
    {
        // F2: Setting ItemsSource before ItemTemplate should not throw NullReferenceException
        var swipeCardView = new SwipeCardView();
        swipeCardView.ItemsSource = new ObservableCollection<string>() { "Item1", "Item2" };
        swipeCardView.ItemTemplate = CreateSimpleTemplate();

        Assert.AreEqual("Item1", swipeCardView.TopItem);
    }

    [TestMethod]
    public async Task SingleItemCollection_SwipeShouldWork()
    {
        var swipeCardView = new SwipeCardView { ItemTemplate = CreateSimpleTemplate() };
        swipeCardView.ItemsSource = new ObservableCollection<string>() { "OnlyItem" };

        Assert.AreEqual("OnlyItem", swipeCardView.TopItem);

        await swipeCardView.InvokeSwipe(SwipeCardDirection.Right);

        Assert.IsNull(swipeCardView.TopItem);
    }

    [TestMethod]
    public async Task SwipeLeft_ShouldWork()
    {
        var swipeCardView = new SwipeCardView { ItemTemplate = CreateSimpleTemplate() };
        swipeCardView.ItemsSource = new ObservableCollection<string>() { "Item1", "Item2" };

        SwipeCardDirection swipedDirection = SwipeCardDirection.None;
        swipeCardView.Swiped += (s, e) => swipedDirection = e.Direction;

        await swipeCardView.InvokeSwipe(SwipeCardDirection.Left);

        Assert.AreEqual(SwipeCardDirection.Left, swipedDirection);
        Assert.AreEqual("Item2", swipeCardView.TopItem);
    }

    [TestMethod]
    public async Task SwipeUp_ShouldWork()
    {
        var swipeCardView = new SwipeCardView { ItemTemplate = CreateSimpleTemplate() };
        swipeCardView.ItemsSource = new ObservableCollection<string>() { "Item1", "Item2" };

        SwipeCardDirection swipedDirection = SwipeCardDirection.None;
        swipeCardView.Swiped += (s, e) => swipedDirection = e.Direction;

        await swipeCardView.InvokeSwipe(SwipeCardDirection.Up);

        Assert.AreEqual(SwipeCardDirection.Up, swipedDirection);
    }

    [TestMethod]
    public async Task SwipeDown_ShouldWork()
    {
        var swipeCardView = new SwipeCardView { ItemTemplate = CreateSimpleTemplate() };
        swipeCardView.ItemsSource = new ObservableCollection<string>() { "Item1", "Item2" };

        SwipeCardDirection swipedDirection = SwipeCardDirection.None;
        swipeCardView.Swiped += (s, e) => swipedDirection = e.Direction;

        await swipeCardView.InvokeSwipe(SwipeCardDirection.Down);

        Assert.AreEqual(SwipeCardDirection.Down, swipedDirection);
    }

    [TestMethod]
    public async Task DraggingCommand_UsesCorrectParameter()
    {
        // F4: DraggingCommand.CanExecute should use DraggingCommandParameter, not SwipedCommandParameter
        var swipeCardView = new SwipeCardView { ItemTemplate = CreateSimpleTemplate() };
        swipeCardView.ItemsSource = new ObservableCollection<string>() { "Item1", "Item2" };

        object? receivedCanExecuteParam = null;
        swipeCardView.DraggingCommandParameter = "DragParam";
        swipeCardView.SwipedCommandParameter = "SwipeParam";

        var draggingFired = false;
        swipeCardView.DraggingCommand = new Command<object>(
            execute: _ => draggingFired = true,
            canExecute: param =>
            {
                receivedCanExecuteParam = param;
                return true;
            });

        await swipeCardView.InvokeSwipe(SwipeCardDirection.Right);

        Assert.IsTrue(draggingFired);
        Assert.AreEqual("DragParam", receivedCanExecuteParam);
    }

    [TestMethod]
    public void Dispose_ShouldNotThrow()
    {
        var swipeCardView = new SwipeCardView { ItemTemplate = CreateSimpleTemplate() };
        swipeCardView.ItemsSource = new ObservableCollection<string>() { "Item1", "Item2" };

        swipeCardView.Dispose();

        // Double dispose should also not throw
        swipeCardView.Dispose();
    }

    [TestMethod]
    public void CollectionReset_ShouldNotCrash()
    {
        var items = new ObservableCollection<string>() { "Item1", "Item2", "Item3" };
        var swipeCardView = new SwipeCardView { ItemTemplate = CreateSimpleTemplate() };
        swipeCardView.ItemsSource = items;

        Assert.AreEqual("Item1", swipeCardView.TopItem);

        items.Clear();

        Assert.IsNull(swipeCardView.TopItem);
    }

    [TestMethod]
    public void CollectionReset_WithNewItems_ShouldReinitialize()
    {
        var items = new ObservableCollection<string>() { "Item1", "Item2" };
        var swipeCardView = new SwipeCardView { ItemTemplate = CreateSimpleTemplate() };
        swipeCardView.ItemsSource = items;

        items.Clear();
        items.Add("NewItem1");
        items.Add("NewItem2");

        // After adding items when both cards invisible, Setup should be called
        Assert.IsNotNull(swipeCardView.TopItem);
    }

    [TestMethod]
    public async Task SwipeThroughAllItems_TopItemTrackedCorrectly()
    {
        var swipeCardView = new SwipeCardView { ItemTemplate = CreateSimpleTemplate() };
        var items = new ObservableCollection<string>() { "A", "B", "C", "D" };
        swipeCardView.ItemsSource = items;

        Assert.AreEqual("A", swipeCardView.TopItem);

        await swipeCardView.InvokeSwipe(SwipeCardDirection.Right);
        Assert.AreEqual("B", swipeCardView.TopItem);

        await swipeCardView.InvokeSwipe(SwipeCardDirection.Right);
        Assert.AreEqual("C", swipeCardView.TopItem);

        await swipeCardView.InvokeSwipe(SwipeCardDirection.Right);
        Assert.AreEqual("D", swipeCardView.TopItem);

        await swipeCardView.InvokeSwipe(SwipeCardDirection.Right);
        Assert.IsNull(swipeCardView.TopItem);
    }

    [TestMethod]
    public void SwipedEventArgs_ContainsCorrectItem()
    {
        var swipeCardView = new SwipeCardView { ItemTemplate = CreateSimpleTemplate() };
        swipeCardView.ItemsSource = new ObservableCollection<string>() { "Item1", "Item2" };

        object? swipedItem = null;
        swipeCardView.Swiped += (s, e) => swipedItem = e.Item;

        swipeCardView.InvokeSwipe(SwipeCardDirection.Right).Wait();

        Assert.AreEqual("Item1", swipedItem);
    }

    [TestMethod]
    public void ItemsSourceSetToNull_ShouldNotCrash()
    {
        var swipeCardView = new SwipeCardView { ItemTemplate = CreateSimpleTemplate() };
        swipeCardView.ItemsSource = new ObservableCollection<string>() { "Item1" };
        swipeCardView.ItemsSource = null!;

        // Should not crash
        Assert.IsNotNull(swipeCardView);
    }

    [TestMethod]
    public async Task BorderTemplate_ShouldWorkAsDataTemplateRoot()
    {
        // Issue #3: Border should work as DataTemplate root, not just Frame
        var borderTemplate = new DataTemplate(() =>
        {
            var border = new Border
            {
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
                BackgroundColor = Colors.White,
                InputTransparent = true
            };
            var label = new Label();
            label.SetBinding(Label.TextProperty, ".");
            border.Content = label;
            return border;
        });

        var swipeCardView = new SwipeCardView { ItemTemplate = borderTemplate };
        swipeCardView.ItemsSource = new ObservableCollection<string>() { "Card1", "Card2", "Card3" };

        Assert.AreEqual("Card1", swipeCardView.TopItem);

        await swipeCardView.InvokeSwipe(SwipeCardDirection.Right);
        Assert.AreEqual("Card2", swipeCardView.TopItem);

        await swipeCardView.InvokeSwipe(SwipeCardDirection.Left);
        Assert.AreEqual("Card3", swipeCardView.TopItem);
    }

    [TestMethod]
    public async Task VerticalStackLayoutTemplate_ShouldWorkAsDataTemplateRoot()
    {
        // Issue #3: StackLayout/VerticalStackLayout should work as DataTemplate root
        var layoutTemplate = new DataTemplate(() =>
        {
            var layout = new VerticalStackLayout();
            var label = new Label();
            label.SetBinding(Label.TextProperty, ".");
            layout.Children.Add(label);
            return layout;
        });

        var swipeCardView = new SwipeCardView { ItemTemplate = layoutTemplate };
        swipeCardView.ItemsSource = new ObservableCollection<string>() { "Card1", "Card2" };

        Assert.AreEqual("Card1", swipeCardView.TopItem);

        await swipeCardView.InvokeSwipe(SwipeCardDirection.Right);
        Assert.AreEqual("Card2", swipeCardView.TopItem);
    }
}
