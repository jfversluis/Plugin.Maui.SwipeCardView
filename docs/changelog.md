# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/) and this project adheres to [Semantic Versioning](http://semver.org/).

## 1.1.0

### Added

- **Card Stack Visual Effect** — New `StackDepth` property to show stacked cards behind the top card, creating a visual deck effect. When enabled, the back card is visible at rest and decorative shadow cards peek below for depth
- **StackOffset property** — Controls the vertical offset (in dp) between each stacked card (default: 10)
- **StackScaleStep property** — Controls the scale reduction per successive shadow card (default: 0.03)

### Fixed

- **Memory leak in Dispose** — Shadow card `Border` elements are now properly disposed when the control is disposed
- **SizeChanged handler leak** — `OnSizeChangedForStack` event handler is now unsubscribed in `Dispose()` to prevent leaks when the control is disposed before layout completes
- **CS8602 null warning** — Added null check for `ItemsSource` before accessing `.Count`
- **StackScaleStep documentation** — Fixed XML doc that incorrectly stated default was 0.04 (actual: 0.03)

## 1.0.1

### Fixed

- **GoBack card ordering** — Fixed ZIndex race condition where the restored card could appear behind the current top card
- **GoBack item tracking** — Fixed off-by-one error in `_itemIndex` after GoBack, which caused the wrong next card to load

## 1.0.0

### Added

- **GoBack navigation** — `GoBack()` and `GoBack(bool animated)` methods to navigate back to the previously swiped card, with optional slide-in animation
- **PreviousItem property** — Bindable property that exposes the previously swiped item
- **LoopCards property** — Enable infinite card looping so the stack cycles back to the first item
- **Border support** — `Border` can now be used as the root element in `DataTemplate` (sample included)
- **InvokeSwipe overload** — `InvokeSwipe(SwipeCardDirection)` simplified overload with default parameters
- **CardView property on DraggingCardEventArgs** — Access the card `View` being dragged for template element lookups
- **IDisposable implementation** — Proper resource cleanup with virtual `Dispose(bool)` for subclassing
- **BackCardScale validation** — Value is now validated to be in the range (0, 1]
- **Collection version guard** — Prevents double-advance when a `Swiped` handler removes items from the collection
- **Full ObservableCollection support** — Add, Remove, Replace, Move, and Reset operations are all handled correctly
- **XML documentation** — All public API members are fully documented
- **35 unit tests** — Comprehensive test coverage for all features

### Fixed

- **async void → async Task** — `HandleTouchEnd` and `OnPanUpdated` converted from dangerous `async void` to properly awaited `async Task`
- **Android layout cache corruption** — Fixed visual glitch where back card rendered at wrong scale/position after swipe due to Android caching layout bounds at non-1.0 Scale values
- **Back card not visible after Clear+Add** — Fixed issue where the second card wasn't initialized when items were added one at a time to an empty collection
- **Race conditions** — Added `_ignoreTouch` guard to prevent concurrent gesture processing
- **NaN/Infinity guards** — Protected against invalid translation/rotation values during edge-case drag calculations
- **Snap-back animation** — Restored the nice scale-down animation on snap-back while preventing Android layout cache corruption
- **Swipe-off animation** — Improved from abrupt 125ms SpringOut to smooth 250ms CubicIn for more natural card dismissal
- **Event sender convention** — Events now correctly use `sender = this` (standard .NET convention)
- **GestureStatus.Canceled** — Properly handled to reset card position on interrupted gestures
- **ZIndex management** — Correct Z-ordering ensures top card always renders above back card
- **Null reference protections** — Added null checks throughout gesture handling and collection operations
- **PreviousItem cleanup** — Cleared on collection Reset and when all items are removed
- **LoopCards + GoBack** — Fixed `ArgumentOutOfRangeException` when calling `GoBack()` after a full loop cycle

### Changed

- **Target framework** — Updated from .NET 9 to .NET 10
- **Microsoft.Maui.Controls** — Updated from 9.0.40 to 10.0.41
- **Animation APIs** — Migrated to .NET 10 async animation APIs (`TranslateToAsync`, `RotateToAsync`, `ScaleToAsync`)
- **SwipedCommand/DraggingCommand parameters** — Commands now consistently receive typed EventArgs as the parameter (falls back to `SwipedCommandParameter`/`DraggingCommandParameter` when set)
- **Consistent command parameter naming** — Renamed `SwipeCommand` → `SwipedCommand` to match event name

### Breaking Changes

- **Minimum .NET version** — Requires .NET 10+
- **SwipeCommand → SwipedCommand** — Property renamed for consistency with the `Swiped` event
- **Command parameters** — `SwipedCommand` and `DraggingCommand` now receive typed EventArgs by default instead of the raw `CommandParameter`
- **Dispose pattern** — `Dispose(bool disposing)` is now `protected virtual` (allows subclass cleanup)
- **EventArgs properties** — `SwipedCardEventArgs` and `DraggingCardEventArgs` properties are now get-only (immutable)
