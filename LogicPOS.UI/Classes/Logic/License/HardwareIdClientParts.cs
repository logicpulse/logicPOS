using System;
using System.IO;
using System.Linq;
using System.Management;
using Microsoft.Win32;

namespace logicpos.Classes.Logic.License
{
    public static class HardwareIdClientParts
    {
        public static GenHardwareIDQuery GetHardwareParts()
        {
            return new GenHardwareIDQuery
            {
                CpuId = GetProcessorId(),
                MotherboardSerial = GetBaseBoardSerialNumber(),
                DiskSerial = GetSystemDriveSerialNumber(),
                MachineGuid = GetMachineGuid(),
                SystemUuid = GetComputerSystemProductUuid()
            };
        }

        static string GetComputerSystemProductUuid()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT UUID FROM Win32_ComputerSystemProduct"))
                {
                    foreach (var x in searcher.Get())
                    {
                        var uuid = x["UUID"]?.ToString();
                        if (!string.IsNullOrWhiteSpace(uuid))
                            return uuid;
                    }
                }
            }
            catch
            {
            }

            return string.Empty;
        }

        static string GetBaseBoardSerialNumber()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BaseBoard"))
                {
                    foreach (var x in searcher.Get())
                    {
                        var serial = x["SerialNumber"]?.ToString();
                        if (!string.IsNullOrWhiteSpace(serial))
                            return serial;
                    }
                }
            }
            catch
            {
            }

            return string.Empty;
        }

        static string GetProcessorId()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT ProcessorId FROM Win32_Processor"))
                {
                    foreach (var x in searcher.Get())
                    {
                        var id = x["ProcessorId"]?.ToString();
                        if (!string.IsNullOrWhiteSpace(id))
                            return id;
                    }
                }
            }
            catch
            {
            }

            return string.Empty;
        }

        static string GetSystemDriveSerialNumber()
        {
            try
            {
                var systemDriveLetter = Path.GetPathRoot(Environment.SystemDirectory);
                if (string.IsNullOrWhiteSpace(systemDriveLetter))
                    return string.Empty;

                var driveLetter = systemDriveLetter.TrimEnd('\\');

                using (var searcher = new ManagementObjectSearcher(
                           $"SELECT VolumeSerialNumber FROM Win32_LogicalDisk WHERE DeviceID = '{driveLetter}'"))
                {
                    foreach (var x in searcher.Get())
                    {
                        var serial = x["VolumeSerialNumber"]?.ToString();
                        if (!string.IsNullOrWhiteSpace(serial))
                            return serial;
                    }
                }
            }
            catch
            {
            }

            return string.Empty;
        }

        static string GetMachineGuid()
        {
            try
            {
                using (var key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                           .OpenSubKey(@"SOFTWARE\Microsoft\Cryptography"))
                {
                    var value = key?.GetValue("MachineGuid")?.ToString();
                    if (!string.IsNullOrWhiteSpace(value))
                        return value;
                }
            }
            catch
            {
            }

            try
            {
                using (var key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)
                           .OpenSubKey(@"SOFTWARE\Microsoft\Cryptography"))
                {
                    var value = key?.GetValue("MachineGuid")?.ToString();
                    if (!string.IsNullOrWhiteSpace(value))
                        return value;
                }
            }
            catch
            {
            }

            return string.Empty;
        }

        static string EscapeWmiPath(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return value.Replace(@"\", @"\\").Replace("'", "\\'");
        }
    }


}
