namespace Vurdalakov
{
    using System;
    using System.IO;
    using System.Text;

    public class Application : DosToolsApplication
    {
        protected override Int32 Execute()
        {
            if (_commandLineParser.IsOptionSet("l", "list"))
            {
                foreach (FirmwareTableType tableType in Enum.GetValues(typeof(FirmwareTableType)))
                {
                    var tableIds = FirmwareTables.EnumFirmwareTables(tableType);

                    foreach (var tableId in tableIds)
                    {
                        Console.WriteLine("0x{0:X8} '{1}' - 0x{2:X8} '{3}'", (UInt32)tableType, tableType.ToString().ToUpper(), tableId, FirmwareTables.UInt32ToString(tableId));
                    }
                }
            }
            else if (_commandLineParser.IsOptionSet("s", "save"))
            {
                if ((_commandLineParser.FileNames.Length < 2) || (_commandLineParser.FileNames.Length > 3))
                {
                    Help();
                }

                var tableType = MakeUInt32(_commandLineParser.FileNames[0]);
                var tableId = MakeUInt32(_commandLineParser.FileNames[1]);

                var data = FirmwareTables.GetFirmwareTable(tableType, tableId);

                var fileName = 3 == _commandLineParser.FileNames.Length ? _commandLineParser.FileNames[2] : FormatFileName(tableType, tableId);

                File.WriteAllBytes(fileName, data);

                Console.WriteLine("OK: '{0}' ({1:N0} bytes)", fileName, data.Length);
            }
            else if (_commandLineParser.IsOptionSet("a", "all"))
            {
                foreach (FirmwareTableType tableType in Enum.GetValues(typeof(FirmwareTableType)))
                {
                    var tableIds = FirmwareTables.EnumFirmwareTables(tableType);

                    foreach (var tableId in tableIds)
                    {
                        var data = FirmwareTables.GetFirmwareTable(tableType, tableId);

                        var fileName = FormatFileName((UInt32)tableType, tableId);

                        File.WriteAllBytes(fileName, data);

                        Console.WriteLine("'{0}' ({1:N0} bytes)", fileName, data.Length);
                    }
                }

                Console.WriteLine("All OK");
            }
            else if (_commandLineParser.IsOptionSet("d", "decode"))
            {
                if ((_commandLineParser.FileNames.Length < 1) || (_commandLineParser.FileNames.Length > 2))
                {
                    Help();
                }

                var tableType = MakeUInt32(_commandLineParser.FileNames[0]);

                if (tableType != (UInt32)FirmwareTableType.Acpi)
                {
                    throw new NotSupportedException("Only the following frimware tables can be decoded: ACPI");
                }

                if (2 == _commandLineParser.FileNames.Length)
                {
                    var tableId = MakeUInt32(_commandLineParser.FileNames[1]);

                    DecodeAcpiTable(tableId);
                }
                else
                {
                    var tableIds = FirmwareTables.EnumFirmwareTables(tableType);

                    foreach (var tableId in tableIds)
                    {
                        DecodeAcpiTable(tableId);
                        Console.WriteLine();
                    }
                }
            }
            else
            {
                Help();
            }

            return 0;
        }

        private void DecodeAcpiTable(UInt32 tableId)
        {
            var tableType = (UInt32)FirmwareTableType.Acpi;
            Console.WriteLine("=== ACPI table 0x{0:X8} '{1}'", tableId, FirmwareTables.UInt32ToString(tableId));

            var data = FirmwareTables.GetFirmwareTable(tableType, tableId);
            Console.WriteLine("Table size: {0:N0} bytes", data.Length);

            var acpiTable = new AcpiTable(data);

            Console.WriteLine("--- ACPI table header");
            Console.WriteLine("Signature:       '{0}'", acpiTable.Signature);
            Console.WriteLine("Length:          {0:N0} bytes", acpiTable.Length);
            Console.WriteLine("Revision:        {0:N0}", acpiTable.Revision);
            Console.WriteLine("Checksum:        0x{0:X2} - {1}OK", acpiTable.Checksum, acpiTable.ChecksumIsValid ? "" : "NOT ");
            Console.WriteLine("OEM ID:          '{0}'", acpiTable.OemId);
            Console.WriteLine("OEM Table ID:    '{0}'", acpiTable.OemTableId);
            Console.WriteLine("OEM Revison:     {0}", acpiTable.OemRevision);
            Console.WriteLine("Creator ID:      '{0}'", acpiTable.CreatorId);
            Console.WriteLine("Creator Revison: {0} or 0x{0:X8}", acpiTable.CreatorRevision);
            Console.WriteLine("--- ACPI table payload");
            Console.WriteLine("Length:          {0:N0} bytes", acpiTable.Payload.Length);
            PrintArray(acpiTable.Payload);
        }

        private UInt32 MakeUInt32(String param)
        {
            param = param.ToUpper();

            if (4 == param.Length)
            {
                return FirmwareTables.StringToUInt32(param);
            }

            if (param.StartsWith("0X"))
            {
                param = param.Substring(2);
            }

            return Convert.ToUInt32(param, 16);
        }

        private String FormatFileName(UInt32 tableType, UInt32 tableId)
        {
            return String.Format("{0:X8}_{1:X8}.bin", tableType, tableId);
        }

        private void PrintArray(Byte[] array)
        {
            const int rowLength = 16;
            var rows = array.Length / rowLength + 1;
            var first = 0;
            var index = 0;
            for (var row = 1; row <= rows; row++)
            {
                Console.Write("{0:X6}: ", first);

                var chars = "";
                for (var i = 0; i < rowLength; i++)
                {
                    if (index < array.Length)
                    {
                        var c = array[index];
                        Console.Write("{0:X2} ", c);
                        chars += c < 32 ? ' ' : (char)c;
                        index++;
                    }
                    else
                    {
                        Console.Write("   ");
                    }
                }

                Console.WriteLine(" {0}", chars);

                first += 16;
            }
        }

        protected override void Help()
        {
            Console.WriteLine("FirmwareTables {0} | https://github.com/vurdalakov/firmwaretables\n", ApplicationVersion);
            Console.WriteLine("Lists, extracts and decodes system firmware tables.\n");
            Console.WriteLine("Usage:\n\tfirmwaretables <-list | -all | -save | -decode <table type> <table id> [filename]> [-silent]\n");
            Console.WriteLine("Commands:\n\t-l - list available system firmware tables\n\t-a - save all system firmware tables to files\n\t-s - save specific system firmware table to file\n\t-d - decode specific system firmware table\n");
            Console.WriteLine("Options:\n\t-silent - no error messages are shown; check exit code\n");
            Console.WriteLine("Exit codes:\n\t0 - operation succeeded\n\t1 - operation failed\n\t-1 - invalid command line syntax\n");

            base.Help();
        }
    }
}
