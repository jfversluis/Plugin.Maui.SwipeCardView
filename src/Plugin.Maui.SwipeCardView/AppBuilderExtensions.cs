using Microsoft.Maui.Hosting;
using Microsoft.Maui.Controls.Compatibility.Hosting;

namespace Plugin.Maui.SwipeCardView;

/// <summary>
/// Extensions for MauiAppBuilder
/// </summary>
public static class AppBuilderExtensions
{
	/// <summary>
	/// Initializes the Plugin.Maui.SwipeCardView library.
	/// </summary>
	/// <param name="builder"><see cref="MauiAppBuilder"/> generated by <see cref="MauiApp"/>.</param>
	/// <returns><see cref="MauiAppBuilder"/> initialized for <see cref="SwipeCardView"/>.</returns>
	public static MauiAppBuilder UseSwipeCardView(this MauiAppBuilder builder)
	{
        builder.UseMauiCompatibility();
		return builder;
	}
}