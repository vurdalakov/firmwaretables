# FirmwareTables

### Overview

A set of classes and command line tools that work with system firmware tables (ACPI, SMBIOS).

Distributed under the [MIT license](http://opensource.org/licenses/MIT).

### `FirmwareTables` class

`FirmwareTables` class enumerates system firmware tables and gets them in binary form, using [EnumSystemFirmwareTables](https://msdn.microsoft.com/en-us/library/windows/desktop/ms724259.aspx) and [GetSystemFirmwareTable](https://msdn.microsoft.com/en-us/library/windows/desktop/ms724379.aspx) Windows API functions.

Example 1: System firmware tables enumaration

```
foreach (FirmwareTableType tableType in Enum.GetValues(typeof(FirmwareTableType)))
{
    var tableIds =  EnumFirmwareTables(tableType);

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

### `AcpiTable` class

`AcpiTable` class parses a given ACPI table binary and provides access to ACPI table header fields and table payload.

Example 1: Extract OEM Windows product key from ACPI MSDM table

```
var table = FirmwareTables.GetAcpiTable("MDSM");
var productKeyLength = (int)table.GetPayloadUInt32(16);
var productKey = table.GetPayloadString(20, productKeyLength);

Console.WriteLine("OEM Windows product key: '{0}'", productKey);
```

### `firmwaretables` tool

**FirmwareTables** is a command-line C# program that lists system firmware tables and saves them to binary files.

This tool is part of [`dostools` collection](https://github.com/vurdalakov/dostools). 

Syntax:

```
firmwaretables -list # lists available system firmware tables
firmwaretables -save <table type> <table id> [filename] [-silent] # saves specific system firmware table to binary file
firmwaretables -all [-silent] # saves all system firmware tables to binary files
firmwaretables -decode <table type> <table id> # decodes given table
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

Example 4: Decode specific firmware table.

```
firmwaretables -decode acpi mdsm
firmwaretables -d -0x52534D42 0x00000000
```

Example 1 output: List all available system firmware tables.

```
C:\temp>firmwaretables.exe -l
0x41435049 'ACPI' - 0x50474244 'PGBD'
0x41435049 'ACPI' - 0x4746434D 'GFCM'
0x41435049 'ACPI' - 0x50434146 'PCAF'
0x41435049 'ACPI' - 0x43495041 'CIPA'
0x41435049 'ACPI' - 0x544F4F42 'TOOB'
0x41435049 'ACPI' - 0x52414D44 'RAMD'
0x41435049 'ACPI' - 0x54455048 'TEPH'
0x41435049 'ACPI' - 0x54445046 'TDPF'
0x41435049 'ACPI' - 0x41504354 'APCT'
0x41435049 'ACPI' - 0x49464555 'IFEU'
0x41435049 'ACPI' - 0x54445353 'TDSS'
0x41435049 'ACPI' - 0x324D5054 '2MPT'
0x41435049 'ACPI' - 0x4D44534D 'MDSM'
0x41435049 'ACPI' - 0x32474244 '2GBD'
0x41435049 'ACPI' - 0x21465341 '!FSA'
0x41435049 'ACPI' - 0x54505341 'TPSA'
0x41435049 'ACPI' - 0x5449504C 'TIPL'
0x4649524D 'FIRM' - 0x000C0000 ' ♀  '
0x4649524D 'FIRM' - 0x000E0000 ' ♫  '
0x52534D42 'RSMB' - 0x00000000 '    '
```

Example 4 output: Decode specific firmware table.

```
C:\temp>firmwaretables -decode acpi mdsm
=== 'MSDM' ACPI table 0x4D44534D or 'MDSM'
Table size: 85 bytes
--- ACPI table header
Signature:       'MSDM'
Length:          85 bytes
Revision:        3
Checksum:        0x24 - OK
OEM ID:          'LENOVO'
OEM Table ID:    'CB-01   '
OEM Revison:     1
Creator ID:      'ACPI'
Creator Revison: 262144 or 0x00040000
--- ACPI table payload
Version:         1
Reserved:        0
Data Type:       1
Data Reserved:   0
Data Length:     29
Product Key:     XYQGN-7YFFX-2FCYC-8T4FY-MY47F
```

```
C:\temp>firmwaretables.exe -d acpi tdsx
=== 'XSDT' ACPI table 0x54445358 or 'TDSX'
Table size: 252 bytes
--- ACPI table header
Signature:       'XSDT'
Length:          252 bytes
Revision:        1
Checksum:        0x29 - OK
OEM ID:          'LENOVO'
OEM Table ID:    'CB-01   '
OEM Revison:     1
Creator ID:      '    '
Creator Revison: 16777235 or 0x01000013
--- ACPI table payload
Length:          216 bytes
000000: 00 90 FE 77 00 00 00 00 00 C0 FF 77 00 00 00 00   ??w     Ayw
000010: 00 B0 FF 77 00 00 00 00 00 A0 FF 77 00 00 00 00   °yw      yw
000020: 00 90 FF 77 00 00 00 00 00 80 FF 77 00 00 00 00   ?yw     ?yw
000030: 00 70 FF 77 00 00 00 00 00 60 FF 77 00 00 00 00   pyw     `yw
000040: 00 00 FF 77 00 00 00 00 00 F0 FE 77 00 00 00 00    yw     ??w
000050: 00 E0 FE 77 00 00 00 00 00 D0 FE 77 00 00 00 00   a?w     ??w
000060: 00 C0 FE 77 00 00 00 00 00 B0 FE 77 00 00 00 00   A?w     °?w
000070: 00 A0 FE 77 00 00 00 00 00 80 FE 77 00 00 00 00    ?w     ??w
000080: 00 70 FE 77 00 00 00 00 00 60 FE 77 00 00 00 00   p?w     `?w
000090: 00 50 FE 77 00 00 00 00 00 50 FC 77 00 00 00 00   P?w     Puw
0000A0: 00 40 FC 77 00 00 00 00 00 20 FC 77 00 00 00 00   @uw      uw
0000B0: 00 10 FC 77 00 00 00 00 00 00 FC 77 00 00 00 00    uw      uw
0000C0: 00 B0 FB 77 00 00 00 00 00 A0 FB 77 00 00 00 00   °uw      uw
0000D0: 00 90 FB 77 00 00 00 00                           ?uw
```
