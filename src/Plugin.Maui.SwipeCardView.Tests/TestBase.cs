using System.Globalization;
using Microsoft.Maui.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Plugin.Maui.SwipeCardView.Tests;

[TestClass]
public abstract class TestBase
{
    CultureInfo _defaultCulture;
    CultureInfo _defaultUICulture;

    [TestInitialize]
    public void Initialize()
    {
        _defaultCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
        _defaultUICulture = System.Threading.Thread.CurrentThread.CurrentUICulture;
        //Device.PlatformServices = new MockPlatformServices();

        MauiApp.CreateBuilder().Build();
    }

    [TestCleanup]
    public void Cleanup()
    {
        //Device.PlatformServices = null;
        System.Threading.Thread.CurrentThread.CurrentCulture = _defaultCulture;
        System.Threading.Thread.CurrentThread.CurrentUICulture = _defaultUICulture;
    }
}
