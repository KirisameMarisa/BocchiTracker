using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.ServiceClientAdapters
{
    public interface IMacAddressProvider
    {
        List<string> GetMacAddresses();
    }

    public class MacAddressProvider : IMacAddressProvider
    {
        public List<string> GetMacAddresses()
        {
            var list = new List<string>();

            foreach (var adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (adapter.OperationalStatus == OperationalStatus.Up)
                {
                    if (IsValidInterface(adapter))
                    {
                        list.Add(adapter.GetPhysicalAddress().ToString());
                    }
                }
            }
            return list;
        }

        private bool IsValidInterface(NetworkInterface adapter)
        {
            return adapter.NetworkInterfaceType != NetworkInterfaceType.Unknown &&
                   adapter.NetworkInterfaceType != NetworkInterfaceType.Loopback;
        }
    }
}
