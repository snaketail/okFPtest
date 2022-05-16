using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpalKelly.FrontPanel;

namespace okFPtest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            okCFrontPanel m_dev;
            okCFrontPanelDevices devices;
            devices = new okCFrontPanelDevices();
            m_dev = devices.Open();
            if (m_dev == null)
            {
                Console.WriteLine("A device could not be opened.  Is one connected?");
                Console.ReadLine();
            }
            okTDeviceInfo devInfo = new okTDeviceInfo();
            m_dev.GetDeviceInfo(devInfo);
            Console.WriteLine("Device firmware version: " +
                devInfo.deviceMajorVersion + "." +
                devInfo.deviceMinorVersion);
            Console.WriteLine("Device serial number: " + devInfo.serialNumber);
            Console.WriteLine("Device ID: " + devInfo.deviceID);

            // Setup the PLL from defaults.
            m_dev.LoadDefaultPLLConfiguration();

            if (okCFrontPanel.ErrorCode.NoError != m_dev.ConfigureFPGA(@"C:\FPGA\slow\fpRunner.bit"))
            {
                Console.WriteLine("FPGA configuration failed.");
                Console.ReadLine();
            }

            // Check for FrontPanel support in the FPGA configuration.
            if (false == m_dev.IsFrontPanelEnabled())
            {
                Console.WriteLine("FrontPanel support is not available.");
            }

            Console.WriteLine("FrontPanel support is available.");
            Console.ReadLine();

            m_dev.SetWireInValue(0x00, 0x0a);
            m_dev.UpdateWireIns();
            m_dev.SetWireInValue(0x01, 0x0a);
            m_dev.UpdateWireIns(); 
            m_dev.SetWireInValue(0x02, 0x0a);
            m_dev.UpdateWireIns();

            m_dev.SetWireInValue(0x03, 0x01);
            m_dev.UpdateWireIns();
            Console.WriteLine("Enabled Counter. Press to send a trigger");

            Console.ReadLine();
            m_dev.ActivateTriggerIn(0x40, 1);

            Console.WriteLine("trigger sent, press to read the count");
            Console.ReadLine();

            m_dev.UpdateWireOuts();
            uint count = m_dev.GetWireOutValue(0x20);

            Console.WriteLine(count);

            Console.WriteLine("press to clear");

            m_dev.ActivateTriggerIn(0x40, 0);

            Console.WriteLine("trigger sent, press to read the count");
            Console.ReadLine();

            m_dev.UpdateWireOuts();
            count = m_dev.GetWireOutValue(0x20);

            Console.WriteLine(count);

            Console.ReadLine();
        }
    }
}
