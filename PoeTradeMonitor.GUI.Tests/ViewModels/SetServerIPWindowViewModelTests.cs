using System.Net;
using PoeTradeMonitor.GUI.ViewModels;

namespace PoeTradeMonitor.GUI.Tests.ViewModels;

[TestClass]
public class SetServerIPWindowViewModelTests
{
    private SetServerIPWindowViewModel viewModel = null!;
    private IPAddress testIPAddress = null!;

    [TestInitialize]
    public void Setup()
    {
        testIPAddress = IPAddress.Parse("192.168.1.100");
        viewModel = new SetServerIPWindowViewModel(testIPAddress);
    }

    [TestMethod]
    public void Constructor_ShouldInitializeWithProvidedIPAddress()
    {
        Assert.AreEqual("192.168.1.100", viewModel.ServerIP);
    }

    [TestMethod]
    public void Constructor_WithLocalhostIP_ShouldInitializeCorrectly()
    {
        var localhostViewModel = new SetServerIPWindowViewModel(IPAddress.Loopback);

        Assert.AreEqual("127.0.0.1", localhostViewModel.ServerIP);
    }

    [TestMethod]
    public void Constructor_WithIPv6_ShouldInitializeCorrectly()
    {
        var ipv6Address = IPAddress.Parse("2001:db8::1");
        var ipv6ViewModel = new SetServerIPWindowViewModel(ipv6Address);

        Assert.AreEqual("2001:db8::1", ipv6ViewModel.ServerIP);
    }

    [TestMethod]
    public void ServerIP_GetSet_ShouldUpdateCorrectly()
    {
        viewModel.ServerIP = "10.0.0.1";

        Assert.AreEqual("10.0.0.1", viewModel.ServerIP);
    }

    [TestMethod]
    public void SetCommand_WithValidIP_ShouldKeepProvidedIP()
    {
        viewModel.ServerIP = "172.16.0.1";

        // Execute the Set command with null window (window parameter not critical for IP validation)
        viewModel.SetCommand.Execute(null);

        Assert.AreEqual("172.16.0.1", viewModel.ServerIP);
    }

    [TestMethod]
    public void SetCommand_WithInvalidIP_ShouldSetToLocalhost()
    {
        viewModel.ServerIP = "invalid-ip-address";

        viewModel.SetCommand.Execute(null);

        Assert.AreEqual("127.0.0.1", viewModel.ServerIP);
    }

    [TestMethod]
    public void SetCommand_WithEmptyIP_ShouldSetToLocalhost()
    {
        viewModel.ServerIP = "";

        viewModel.SetCommand.Execute(null);

        Assert.AreEqual("127.0.0.1", viewModel.ServerIP);
    }

    [TestMethod]
    public void SetCommand_WithWhitespaceIP_ShouldSetToLocalhost()
    {
        viewModel.ServerIP = "   ";

        viewModel.SetCommand.Execute(null);

        Assert.AreEqual("127.0.0.1", viewModel.ServerIP);
    }

    [TestMethod]
    public void SetCommand_WithMalformedIP_ShouldSetToLocalhost()
    {
        viewModel.ServerIP = "999.999.999.999";

        viewModel.SetCommand.Execute(null);

        Assert.AreEqual("127.0.0.1", viewModel.ServerIP);
    }

    [TestMethod]
    public void SetCommand_WithPartialIP_ShouldKeepValueIfNotStrictlyValidated()
    {
        viewModel.ServerIP = "192.168.1";

        viewModel.SetCommand.Execute(null);

        // The implementation only validates against IPAddress.TryParse()
        // "192.168.1" might not trigger the validation logic, so it remains unchanged
        // This is the actual behavior of the implementation
        Assert.AreEqual("192.168.1", viewModel.ServerIP);
    }

    [TestMethod]
    public void SetCommand_WithValidIPv6_ShouldKeepProvidedIP()
    {
        viewModel.ServerIP = "::1";

        viewModel.SetCommand.Execute(null);

        Assert.AreEqual("::1", viewModel.ServerIP);
    }

    [TestMethod]
    public void SetCommand_WithZeroIP_ShouldKeepProvidedIP()
    {
        viewModel.ServerIP = "0.0.0.0";

        viewModel.SetCommand.Execute(null);

        Assert.AreEqual("0.0.0.0", viewModel.ServerIP);
    }

    [TestMethod]
    public void SetCommand_WithBroadcastIP_ShouldKeepProvidedIP()
    {
        viewModel.ServerIP = "255.255.255.255";

        viewModel.SetCommand.Execute(null);

        Assert.AreEqual("255.255.255.255", viewModel.ServerIP);
    }

    [TestMethod]
    public void SetCommand_CanExecute_ShouldAlwaysReturnTrue()
    {
        Assert.IsTrue(viewModel.SetCommand.CanExecute(null));
        
        viewModel.ServerIP = "invalid";
        Assert.IsTrue(viewModel.SetCommand.CanExecute(null));
        
        viewModel.ServerIP = "192.168.1.1";
        Assert.IsTrue(viewModel.SetCommand.CanExecute(null));
    }

    [TestMethod]
    public void PropertyChanged_ServerIP_ShouldTriggerEvent()
    {
        var propertyChangedEvents = new List<string>();
        viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName != null)
                propertyChangedEvents.Add(e.PropertyName);
        };

        viewModel.ServerIP = "192.168.1.200";

        Assert.IsTrue(propertyChangedEvents.Contains(nameof(viewModel.ServerIP)));
    }

    [TestMethod]
    public void SetCommand_WithNullWindow_ShouldNotThrow()
    {
        // Test that passing null window doesn't cause exceptions
        viewModel.SetCommand.Execute(null);
        
        // If we reach here, no exception was thrown
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void Multiple_IPAddressChanges_ShouldAllBeHandled()
    {
        var testIPs = new[]
        {
            "192.168.1.1",
            "10.0.0.1", 
            "172.16.0.1",
            "127.0.0.1",
            "invalid",
            "8.8.8.8"
        };

        foreach (var ip in testIPs)
        {
            viewModel.ServerIP = ip;
            viewModel.SetCommand.Execute(null);
            
            // Valid IPs should remain unchanged, invalid should become 127.0.0.1
            if (IPAddress.TryParse(ip, out _))
            {
                Assert.AreEqual(ip, viewModel.ServerIP);
            }
            else
            {
                Assert.AreEqual("127.0.0.1", viewModel.ServerIP);
            }
        }
    }
}