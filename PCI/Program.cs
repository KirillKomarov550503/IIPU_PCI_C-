using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.IO;
using System.Text.RegularExpressions;
namespace PCI
{
    class Program
    {
        private static Boolean IsPciDevice(String str)
        {
            return str.Contains("PCI");
        }

        private static string ParseDeviceId(String name)
        {
            int index = name.IndexOf("DEV_");
            return name.Substring(index + 4, 4);
        }

        private static string ParseVendorId(String name)
        {
            int index = name.IndexOf("VEN_");
            return name.Substring(index + 4, 4);
        }
       
        private static void PrintInformation(String vendorId, String deviceId)
        {
            string vendorPattern = @"^[0-9a-fA-F]{4}\s{2}.+$";
            string devicePattern = @"^\t[0-9a-fA-F]{4}\s{2}.+$";
            string[] strings = File.ReadAllLines(@"../../pci.ids.txt", Encoding.Default);
            for(int i =  0 ; i < strings.Count();i++)
            {
                if(Regex.IsMatch(strings[i], vendorPattern) && strings[i].Contains(vendorId))
                {
                    Console.WriteLine("Vendor name: " + strings[i].Substring(strings[i].IndexOf("  ") + 2));
                    for (int j = i + 1; j < strings.Count(); j++)
                    {
                        if (Regex.IsMatch(strings[j], devicePattern) && strings[j].Contains(deviceId))
                        {
                            Console.WriteLine("Device name: " + strings[j].Substring(strings[j].IndexOf("  ") + 2));
                            i = j;
                            break;
                        }
                    }
                }
          
            }
        }
        static void Main(string[] args)
        {
            
            ManagementObjectSearcher objectSearcher = new ManagementObjectSearcher("root\\CIMV2",
"SELECT * FROM Win32_PnPEntity");
            foreach(ManagementObject managementObject in objectSearcher.Get())
            {
                String name = managementObject["DeviceID"].ToString();
                if(IsPciDevice(name))
                {
                    string vendorId = ParseVendorId(name);
                    string deviceId = ParseDeviceId(name);
                    Console.WriteLine("VendorID: " + vendorId);
                    Console.WriteLine("DeviceID: " + deviceId);
                    PrintInformation(vendorId.ToLower(), deviceId.ToLower());
                    Console.WriteLine("____________________________________________________");
                }
                
            }
            Console.ReadLine();
        }
    }
}
