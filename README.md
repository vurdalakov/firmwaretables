# FirmwareTables

#### Overview

A set of classes and command line tools that work with system firmware tables (ACPI, SMBIOS).

Distributed under the [MIT license](http://opensource.org/licenses/MIT).

#### `FirmwareTables` class

`FirmwareTables` class enumerates system firmware tables and gets them in binary form, using [EnumSystemFirmwareTables](https://msdn.microsoft.com/en-us/library/windows/desktop/ms724259.aspx) and [GetSystemFirmwareTable](https://msdn.microsoft.com/en-us/library/windows/desktop/ms724379.aspx) Windows API functions.

Example 1: System firmware tables enumaration

```
foreach (FirmwareTables.TableType tableType in Enum.GetValues(typeof(FirmwareTables.TableType)))
{
    var tableIds = FirmwareTables.EnumFirmwareTables(tableType);

    foreach (var tableId in tableIds)
    {
        Console.WriteLine("'{0}' - 0x{1:X8}", tableType.ToString().ToUpper(), tableId);
    }
}
```

Example 2: Save specific firmware table to binary file.

```
var data = FirmwareTables.GetFirmwareTable("ACPI", "MDSM");

File.WriteAllBytes("Acpi-Windows-License-Key.bin", data);
```

#### `firmwaretables` tool

**FirmwareTables** is a command-line C# program that lists system firmware tables and saves them to binary files.

This tool is part of [`dostools` collection](https://github.com/vurdalakov/dostools). 

Syntax:

```
firmwaretables -list # lists available system firmware tables
firmwaretables -save <table type> <table id> [filename] [-silent] # saves specific system firmware table to binary file
firmwaretables -all [-silent] # saves all system firmware tables to binary files
```

`table type` and `table id` can be a string (e.g. `ACPI`) or a hex number (e.g. `0x52534D42`).

If file name is omitted, it is created based on table type and ID in hex form, e.g. `52534D42_00000000.bin`.

Exit codes:

* 0 - operation succeeded;
* 1 - operation failed;
* -1 - invalid command line syntax.

Example 1: List all available system firmware tables.

```
firmwaretables -list
firmwaretables -l
```

Example 2: Save specific firmware table to binary file.

```
firmwaretables -save ACPI MDSM "Acpi-Windows-License-Key.bin"
firmwaretables -s 0x52534D42 0x00000000 -silent
```

Example 3: Save all firmware tables to binary files.

```
firmwaretables -all"
firmwaretables -a -silent
```
