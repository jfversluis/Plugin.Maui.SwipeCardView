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
        // CanExecute and Execute both receive DraggingCardEventArgs with the correct parameter
        var swipeCardView = new SwipeCardView { ItemTemplate = CreateSimpleTemplate() };
        swipeCardView.ItemsSource = new ObservableCollection<string>() { "Item1", "Item2" };

        object? receivedCanExecuteParam = null;
        object? receivedExecuteParam = null;
        swipeCardView.DraggingCommandParameter = "DragParam";
        swipeCardView.SwipedCommandParameter = "SwipeParam";

        swipeCardView.DraggingCommand = new Command<object>(
            execute: param => receivedExecuteParam = param,
            canExecute: param =>
            {
                receivedCanExecuteParam = param;
                return true;
            });

        await swipeCardView.InvokeSwipe(SwipeCardDirection.Right);

        // Both CanExecute and Execute receive the same DraggingCardEventArgs
        Assert.IsInstanceOfType(receivedCanExecuteParam, typeof(DraggingCardEventArgs));
        Assert.IsInstanceOfType(receivedExecuteParam, typeof(DraggingCardEventArgs));

        // The EventArgs contains the DraggingCommandParameter (not SwipedCommandParameter)
        var canExecArgs = (DraggingCardEventArgs)receivedCanExecuteParam!;
        Assert.AreEqual("DragParam", canExecArgs.Parameter);
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
    public async Task GoBack_ShouldReturnToPreviousCard()
    {
        // Issue #5: ShowPreviousCard / GoBack
        var swipeCardView = new SwipeCardView { ItemTemplate = CreateSimpleTemplate() };
        swipeCardView.ItemsSource = new ObservableCollection<string>() { "Item1", "Item2", "Item3" };

        Assert.AreEqual("Item1", swipeCardView.TopItem);

        await swipeCardView.InvokeSwipe(SwipeCardDirection.Right);
        Assert.AreEqual("Item2", swipeCardView.TopItem);

        var result = swipeCardView.GoBack();
        Assert.IsTrue(result);
        Assert.AreEqual("Item1", swipeCardView.TopItem);
    }

    [TestMethod]
    public async Task GoBack_Animated_ShouldReturnToPreviousCard()
    {
        var swipeCardView = new SwipeCardView { ItemTemplate = CreateSimpleTemplate() };
        swipeCardView.ItemsSource = new ObservableCollection<string>() { "Item1", "Item2", "Item3" };

        await swipeCardView.InvokeSwipe(SwipeCardDirection.Right);
        Assert.AreEqual("Item2", swipeCardView.TopItem);

        var result = swipeCardView.GoBack(animated: true);
        Assert.IsTrue(result);
        // In unit tests, animation completes instantly (no dispatcher)
        Assert.AreEqual("Item1", swipeCardView.TopItem);
    }

    [TestMethod]
    public void GoBack_AtFirstItem_ShouldReturnFalse()
    {
        var swipeCardView = new SwipeCardView { ItemTemplate = CreateSimpleTemplate() };
        swipeCardView.ItemsSource = new ObservableCollection<string>() { "Item1", "Item2" };

        Assert.AreEqual("Item1", swipeCardView.TopItem);

        // Can't go back from first item
        var result = swipeCardView.GoBack();
        Assert.IsFalse(result);
        Assert.AreEqual("Item1", swipeCardView.TopItem);
    }

    [TestMethod]
    public async Task GoBack_MultipleTimes_ShouldWorkCorrectly()
    {
        var swipeCardView = new SwipeCardView { ItemTemplate = CreateSimpleTemplate() };
        swipeCardView.ItemsSource = new ObservableCollection<string>() { "Item1", "Item2", "Item3", "Item4" };

        // Swipe forward 3 times
        await swipeCardView.InvokeSwipe(SwipeCardDirection.Right);
        await swipeCardView.InvokeSwipe(SwipeCardDirection.Right);
        await swipeCardView.InvokeSwipe(SwipeCardDirection.Right);
        Assert.AreEqual("Item4", swipeCardView.TopItem);

        // Go back 3 times
        Assert.IsTrue(swipeCardView.GoBack());
        Assert.AreEqual("Item3", swipeCardView.TopItem);

        Assert.IsTrue(swipeCardView.GoBack());
        Assert.AreEqual("Item2", swipeCardView.TopItem);

        Assert.IsTrue(swipeCardView.GoBack());
        Assert.AreEqual("Item1", swipeCardView.TopItem);

        // Can't go back further
        Assert.IsFalse(swipeCardView.GoBack());
        Assert.AreEqual("Item1", swipeCardView.TopItem);
    }

    [TestMethod]
    public async Task GoBack_ThenSwipeForward_ShouldWorkCorrectly()
    {
        var swipeCardView = new SwipeCardView { ItemTemplate = CreateSimpleTemplate() };
        swipeCardView.ItemsSource = new ObservableCollection<string>() { "Item1", "Item2", "Item3" };

        // Swipe forward twice
        await swipeCardView.InvokeSwipe(SwipeCardDirection.Right);
        await swipeCardView.InvokeSwipe(SwipeCardDirection.Right);
        Assert.AreEqual("Item3", swipeCardView.TopItem);

        // Go back once
        Assert.IsTrue(swipeCardView.GoBack());
        Assert.AreEqual("Item2", swipeCardView.TopItem);

        // Swipe forward again
        await swipeCardView.InvokeSwipe(SwipeCardDirection.Right);
        Assert.AreEqual("Item3", swipeCardView.TopItem);
    }

    [TestMethod]
    public void GoBack_EmptyCollection_ShouldReturnFalse()
    {
        var swipeCardView = new SwipeCardView { ItemTemplate = CreateSimpleTemplate() };
        swipeCardView.ItemsSource = new ObservableCollection<string>();

        var result = swipeCardView.GoBack();
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task PreviousItem_TrackedCorrectly()
    {
        var swipeCardView = new SwipeCardView { ItemTemplate = CreateSimpleTemplate() };
        swipeCardView.ItemsSource = new ObservableCollection<string>() { "Item1", "Item2", "Item3" };

        // Initially no previous item
        Assert.IsNull(swipeCardView.PreviousItem);

        await swipeCardView.InvokeSwipe(SwipeCardDirection.Right);
        Assert.AreEqual("Item1", swipeCardView.PreviousItem);

        await swipeCardView.InvokeSwipe(SwipeCardDirection.Right);
        Assert.AreEqual("Item2", swipeCardView.PreviousItem);
    }

    [TestMethod]
    public async Task GoBack_WithDuplicateItems_ShouldWorkCorrectly()
    {
        // Regression test: GoBack must use index tracking, not Equals() search,
        // to correctly handle collections with duplicate items.
        var swipeCardView = new SwipeCardView { ItemTemplate = CreateSimpleTemplate() };
        swipeCardView.ItemsSource = new ObservableCollection<string>() { "A", "B", "A", "C" };

        Assert.AreEqual("A", swipeCardView.TopItem);

        // Swipe to B
        await swipeCardView.InvokeSwipe(SwipeCardDirection.Right);
        Assert.AreEqual("B", swipeCardView.TopItem);

        // Swipe to second A
        await swipeCardView.InvokeSwipe(SwipeCardDirection.Right);
        Assert.AreEqual("A", swipeCardView.TopItem);

        // GoBack from second A should go to B, not refuse (the old Equals() bug)
        var result = swipeCardView.GoBack();
        Assert.IsTrue(result);
        Assert.AreEqual("B", swipeCardView.TopItem);

        // GoBack again to first A
        result = swipeCardView.GoBack();
        Assert.IsTrue(result);
        Assert.AreEqual("A", swipeCardView.TopItem);

        // GoBack at first A should return false
        result = swipeCardView.GoBack();
        Assert.IsFalse(result);
        Assert.AreEqual("A", swipeCardView.TopItem);
    }

    [TestMethod]
    public async Task BorderTemplate_ShouldWorkAsDataTemplateRoot()
    {
        // Issue #3: Border should work as DataTemplate root, not just Frame.
        // Tests functional behavior (binding, swiping); visual rendering verified in BorderPage sample.
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

    [TestMethod]
    public void CollectionChanged_Add_WhenAllSwiped_ShowsNewItems()
    {
        var items = new ObservableCollection<string>() { "Item1" };
        var swipeCardView = new SwipeCardView { ItemTemplate = CreateSimpleTemplate() };
        swipeCardView.ItemsSource = items;

        // Swipe the only item away
        swipeCardView.InvokeSwipe(SwipeCardDirection.Right).Wait();
        Assert.IsNull(swipeCardView.TopItem);

        // Adding items should reinitialize the view
        items.Add("Item2");
        Assert.AreEqual("Item2", swipeCardView.TopItem);
    }

    [TestMethod]
    public void CollectionChanged_Remove_AllItems_ClearsView()
    {
        var items = new ObservableCollection<string>() { "Item1", "Item2" };
        var swipeCardView = new SwipeCardView { ItemTemplate = CreateSimpleTemplate() };
        swipeCardView.ItemsSource = items;

        Assert.AreEqual("Item1", swipeCardView.TopItem);

        items.RemoveAt(0);
        items.RemoveAt(0);

        Assert.IsNull(swipeCardView.TopItem);
    }

    [TestMethod]
    public void CollectionChanged_Replace_UpdatesCurrentCard()
    {
        var items = new ObservableCollection<string>() { "Item1", "Item2" };
        var swipeCardView = new SwipeCardView { ItemTemplate = CreateSimpleTemplate() };
        swipeCardView.ItemsSource = items;

        Assert.AreEqual("Item1", swipeCardView.TopItem);

        // Replace the current top item
        items[0] = "Replaced";
        Assert.AreEqual("Replaced", swipeCardView.TopItem);
    }

    [TestMethod]
    public void SwipedCommand_CanExecute_ReceivesEventArgs()
    {
        // CanExecute should receive the same EventArgs as Execute (breaking change from PR #6)
        var swipeCardView = new SwipeCardView { ItemTemplate = CreateSimpleTemplate() };
        swipeCardView.ItemsSource = new ObservableCollection<string>() { "Item1", "Item2" };

        object? receivedCanExecParam = null;
        swipeCardView.SwipedCommandParameter = "MyParam";
        swipeCardView.SwipedCommand = new Command<object>(
            execute: _ => { },
            canExecute: param =>
            {
                receivedCanExecParam = param;
                return true;
            });

        swipeCardView.InvokeSwipe(SwipeCardDirection.Right).Wait();

        Assert.IsInstanceOfType(receivedCanExecParam, typeof(SwipedCardEventArgs));
        var args = (SwipedCardEventArgs)receivedCanExecParam!;
        Assert.AreEqual("MyParam", args.Parameter);
        Assert.AreEqual(SwipeCardDirection.Right, args.Direction);
    }
}
