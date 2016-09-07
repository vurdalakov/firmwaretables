namespace Vurdalakov
{
    using System;
    using System.IO;

    public class Application : DosToolsApplication
    {
        protected override Int32 Execute()
        {
            if (_commandLineParser.IsOptionSet("l", "list"))
            {
                foreach (FirmwareTables.TableType tableType in Enum.GetValues(typeof(FirmwareTables.TableType)))
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
                foreach (FirmwareTables.TableType tableType in Enum.GetValues(typeof(FirmwareTables.TableType)))
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
            else
            {
                Help();
            }

            return 0;
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

        protected override void Help()
        {
            Console.WriteLine("FirmwareTables {0} | https://github.com/vurdalakov/firmwaretables\n", ApplicationVersion);
            Console.WriteLine("Lists system firmware tables and saves them to binary files.\n");
            Console.WriteLine("Usage:\n\tfirmwaretables <-list | -all | -save <table type> <table id> [filename]> [-silent]\n");
            Console.WriteLine("Commands:\n\t-l - list available system firmware tables\n\t-a - save all system firmware tables to files\n\t-s - save specific system firmware table to file\n");
            Console.WriteLine("Options:\n\t-silent - no error messages are shown; check exit code\n");
            Console.WriteLine("Exit codes:\n\t0 - operation succeeded\n\t1 - operation failed\n\t-1 - invalid command line syntax\n");

            base.Help();
        }
    }
}
