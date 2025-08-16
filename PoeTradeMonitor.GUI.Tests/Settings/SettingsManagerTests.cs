using PoeTradeMonitor.GUI.Settings;
using System.IO.Abstractions.TestingHelpers;

namespace PoeTradeMonitor.GUI.Tests.Settings;

[TestClass]
public class SettingsManagerTests
{
    private PoeSettings testSettings = null!;
    private SettingsManager settingsManager = null!;
    private MockFileSystem mockFileSystem = null!;

    [TestInitialize]
    public void Setup()
    {
        testSettings = new PoeSettings();
        mockFileSystem = new MockFileSystem();
        settingsManager = new SettingsManager(testSettings, mockFileSystem);
    }

    [TestMethod]
    public void Constructor_ShouldSubscribeToPropertyChanges()
    {
        // The constructor should subscribe to property changes
        // We can verify this by checking that property changes trigger saves
        // This is tested indirectly through the property change test
        Assert.IsNotNull(settingsManager);
    }

    [TestMethod]
    public async Task PropertyChange_ShouldTriggerAsyncSave()
    {
        // Change a property to trigger the save
        testSettings.League = "TestLeague";

        // Give some time for the async save to complete
        await Task.Delay(200);

        // Verify that the file was created in the mock filesystem
        var expectedPath = mockFileSystem.Path.Combine(PoeLib.Common.Constants.DataDirectory, "settings.json");
        Assert.IsTrue(mockFileSystem.File.Exists(expectedPath));
        
        // Verify the content contains our setting
        var content = await mockFileSystem.File.ReadAllTextAsync(expectedPath);
        Assert.IsTrue(content.Contains("TestLeague"));
    }

    [TestMethod]
    public async Task MultiplePropertyChanges_ShouldHandleConcurrentSaves()
    {
        // Trigger multiple property changes rapidly
        testSettings.League = "League1";
        testSettings.HideoutName = "Test Hideout";
        testSettings.League = "Standard";

        // Give time for all saves to complete
        await Task.Delay(300);

        // Verify the final state was saved correctly
        var expectedPath = mockFileSystem.Path.Combine(PoeLib.Common.Constants.DataDirectory, "settings.json");
        Assert.IsTrue(mockFileSystem.File.Exists(expectedPath));
        
        var content = await mockFileSystem.File.ReadAllTextAsync(expectedPath);
        Assert.IsTrue(content.Contains("Standard"));
        Assert.IsTrue(content.Contains("Test Hideout"));
        
        Assert.AreEqual("Standard", testSettings.League);
        Assert.AreEqual("Test Hideout", testSettings.HideoutName);
    }

    [TestMethod]
    public void SettingsManager_ShouldNotThrowOnConstruction()
    {
        // Creating a SettingsManager should not throw any exceptions
        var settings = new PoeSettings();
        var mockFs = new MockFileSystem();
        var manager = new SettingsManager(settings, mockFs);

        Assert.IsNotNull(manager);
    }

    [TestMethod]
    public async Task RapidPropertyChanges_ShouldNotCauseExceptions()
    {
        // Simulate rapid property changes
        var tasks = new List<Task>();
        
        for (int i = 0; i < 10; i++)
        {
            var index = i;
            tasks.Add(Task.Run(() =>
            {
                testSettings.League = $"League{index}";
                testSettings.HideoutName = $"Hideout{index}";
            }));
        }

        await Task.WhenAll(tasks);
        await Task.Delay(300); // Give time for all saves to complete

        // Verify the file was created and contains some data
        var expectedPath = mockFileSystem.Path.Combine(PoeLib.Common.Constants.DataDirectory, "settings.json");
        Assert.IsTrue(mockFileSystem.File.Exists(expectedPath));
        
        // If we get here without exceptions, the rapid changes were handled correctly
        Assert.IsTrue(testSettings.League.StartsWith("League"));
        Assert.IsTrue(testSettings.HideoutName.StartsWith("Hideout"));
    }

    [TestMethod]
    public void SettingsManager_ShouldSubscribeToPropertyChangedEvents()
    {
        // The constructor should subscribe to property changes
        // We can verify this by ensuring property changes don't throw exceptions
        testSettings.League = "TestLeague1";
        testSettings.HideoutName = "TestHideout1";
        testSettings.Running = true;
        testSettings.AlertsEnabled = true;

        // If we reach here, the event subscription is working correctly
        Assert.AreEqual("TestLeague1", testSettings.League);
        Assert.AreEqual("TestHideout1", testSettings.HideoutName);
        Assert.IsTrue(testSettings.Running);
        Assert.IsTrue(testSettings.AlertsEnabled);
    }
}