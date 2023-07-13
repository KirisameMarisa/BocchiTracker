using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BocchiTracker.IssueAssetCollector.Utils.Win32
{
    public interface IGetWindowHandleFromPid
    {
        IntPtr Get(int inPid);
    }

    public class GetWindowHandleFromPid : IGetWindowHandleFromPid
    {
        public IntPtr Get(int inPid)
        {
            IntPtr handle = IntPtr.Zero;
            try
            {
                var process = Process.GetProcessById(inPid);
                if (process == null)
                    return IntPtr.Zero;
                return process.MainWindowHandle;
            }
            catch { }
            return IntPtr.Zero;
        }
    }
}
