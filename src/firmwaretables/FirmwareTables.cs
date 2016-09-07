namespace Vurdalakov
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    public static class FirmwareTables
    {
        public enum TableType : UInt32
        {
            Acpi = 0x41435049,
            Firm = 0x4649524D,
            Rsmb = 0x52534D42
        }

        public static UInt32[] EnumFirmwareTables(TableType tableType)
        {
            return EnumFirmwareTables((UInt32)tableType);
        }

        public static UInt32[] EnumFirmwareTables(String tableType)
        {
            return EnumFirmwareTables(StringToUInt32(tableType));
        }

        public static UInt32[] EnumFirmwareTables(UInt32 tableType)
        {
            var bufferSize = EnumSystemFirmwareTables(tableType, IntPtr.Zero, 0);
            if (0 == bufferSize)
            {
                ThrowWin32Exception("EnumSystemFirmwareTables");
            }

            var buffer = Marshal.AllocHGlobal((Int32)bufferSize);

            var result = EnumSystemFirmwareTables(tableType, buffer, bufferSize);
            if (0 == result)
            {
                ThrowWin32Exception("EnumSystemFirmwareTables");
            }

            var byteArray = ReadByteArray(buffer, 0, (Int32)bufferSize);

            var firmwareTables = new List<UInt32>();

            for (int i = 0; i < byteArray.Length; i += sizeof(UInt32))
            {
                var firmwareTable = BitConverter.ToUInt32(byteArray, i);
                firmwareTables.Add(firmwareTable);
            }

            return firmwareTables.ToArray();
        }

        public static Byte[] GetFirmwareTable(TableType tableType, String tableId)
        {
            return GetFirmwareTable((UInt32)tableType, StringToUInt32(tableId));
        }

        public static Byte[] GetFirmwareTable(TableType tableType, UInt32 tableId)
        {
            return GetFirmwareTable((UInt32)tableType, tableId);
        }

        public static Byte[] GetFirmwareTable(String tableType, String tableId)
        {
            return GetFirmwareTable(StringToUInt32(tableType), StringToUInt32(tableId));
        }

        public static Byte[] GetFirmwareTable(UInt32 tableType, UInt32 tableId)
        {

            var firmwareTables = new List<UInt32>();

            var bufferSize = GetSystemFirmwareTable(tableType, tableId, IntPtr.Zero, 0);
            if (0 == bufferSize)
            {
                ThrowWin32Exception("GetSystemFirmwareTable");
            }

            var buffer = Marshal.AllocHGlobal((Int32)bufferSize);

            var result = GetSystemFirmwareTable(tableType, tableId, buffer, bufferSize);
            if (0 == result)
            {
                ThrowWin32Exception("GetSystemFirmwareTable");
            }

            return ReadByteArray(buffer, 0, (Int32)bufferSize);
        }

        public static UInt32 StringToUInt32(String tableType)
        {
            if (String.IsNullOrWhiteSpace(tableType) || (tableType.Length != 4))
            {
                throw new ArgumentException();
            }

            return ((UInt32)tableType[0] << 24) | ((UInt32)tableType[1] << 16) | ((UInt32)tableType[2] << 8) | (UInt32)tableType[3];
        }

        public static String UInt32ToString(UInt32 tableType)
        {
            var tableTypeString = "";

            tableTypeString += (char)((tableType >> 24) & 0xFF);
            tableTypeString += (char)((tableType >> 16) & 0xFF);
            tableTypeString += (char)((tableType >>  8) & 0xFF);
            tableTypeString += (char)((tableType >>  0) & 0xFF);

            return tableTypeString;
        }

        private static void ThrowWin32Exception(String functionName, String format = null, params Object[] args)
        {
            var ex = new Win32Exception();
            var message = String.Format("{0}() function call failed with error {1}: {2}", functionName, ex.NativeErrorCode, ex.Message);
            throw new Exception(message, ex);
        }

        private static Byte[] ReadByteArray(IntPtr source, Int32 startIndex, Int32 length)
        {
            var byteArray = new Byte[length];
            Marshal.Copy(source, byteArray, startIndex, length);
            return byteArray;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern UInt32 EnumSystemFirmwareTables(UInt32 firmwareTableProviderSignature, IntPtr firmwareTableBuffer, UInt32 bufferSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern UInt32 GetSystemFirmwareTable(UInt32 firmwareTableProviderSignature, UInt32 firmwareTableID, IntPtr firmwareTableBuffer, UInt32 bufferSize);
    }
}
