namespace Vurdalakov
{
    using System;
    using System.Text;

    public class AcpiTable
    {
        const Int32 AcpiTableHeaderLength = 36;

        //public static AcpiTable Parse(Byte[] data)
        //{
        //    if (data.Length < AcpiTableHeaderLength)
        //    {
        //        throw new ArgumentException("Invalid ACPI data");
        //    }

        //    var signature = Encoding.ASCII.GetString(data, 0, 4);

        //    AcpiTable acpiTable = null;
        //    switch (signature.ToUpper())
        //    {
        //        case "MSDM":
        //            acpiTable = new AcpiTableMsdm(data);
        //            break;
        //        default:
        //            acpiTable = new AcpiTable(data);
        //            break;
        //    }

        //    return acpiTable;
        //}

        public AcpiTable(Byte[] data)
        {
            if (data.Length < AcpiTableHeaderLength)
            {
                throw new ArgumentException("Invalid ACPI data");
            }

            this.RawData = data;

            this.Signature = Encoding.ASCII.GetString(data, 0, 4);
            this.Length = BitConverter.ToUInt32(data, 4);
            this.Revision = data[8];
            this.Checksum = data[9];
            this.OemId = Encoding.ASCII.GetString(data, 10, 6);
            this.OemTableId = Encoding.ASCII.GetString(data, 16, 8);
            this.OemRevision = BitConverter.ToUInt32(data, 24);
            this.CreatorId = Encoding.ASCII.GetString(data, 28, 4);
            this.CreatorRevision = BitConverter.ToUInt32(data, 32);

            var payloadLength = data.Length - AcpiTableHeaderLength;
            this.Payload = new Byte[payloadLength];
            Array.Copy(data, AcpiTableHeaderLength, this.Payload, 0, payloadLength);

            this.ChecksumIsValid = ValidateChecksum(data);
        }

        public Byte[] RawData { get; private set; }

        public String Signature { get; private set; }
        public UInt32 Length { get; private set; }
        public Byte Revision { get; private set; }
        public Byte Checksum { get; private set; }
        public Boolean ChecksumIsValid { get; private set; }
        public String OemId { get; private set; }
        public String OemTableId { get; private set; }
        public UInt32 OemRevision { get; private set; }
        public String CreatorId { get; private set; }
        public UInt32 CreatorRevision { get; private set; }
        public Byte[] Payload { get; private set; }

        public Byte GetPayloadByte(Int32 index)
        {
            return this.Payload[index];
        }

        public UInt16 GetPayloadUInt16(Int32 index)
        {
            return BitConverter.ToUInt16(this.Payload, index);
        }

        public UInt32 GetPayloadUInt32(Int32 index)
        {
            return BitConverter.ToUInt32(this.Payload, index);
        }

        public UInt64 GetPayloadUInt64(Int32 index)
        {
            return BitConverter.ToUInt64(this.Payload, index);
        }

        public String GetPayloadString(Int32 index, Int32 length)
        {
            return Encoding.ASCII.GetString(this.Payload, index, length);
        }

        private static Boolean ValidateChecksum(Byte[] data)
        {
            Byte sum = 0;

            for (var i = 0; i < data.Length; i++)
            {
                sum += data[i];
            }

            return 0 == sum;
        }
    }

    //public class AcpiTableMsdm : AcpiTable
    //{
    //    public AcpiTableMsdm(Byte[] data) : base(data)
    //    {
    //        this.Version = BitConverter.ToUInt32(data, 36);
    //        this.Reserved = BitConverter.ToUInt32(data, 40);
    //        this.DataType = BitConverter.ToUInt32(data, 44);
    //        this.DataReserved = BitConverter.ToUInt32(data, 48);
    //        this.DataLength = BitConverter.ToUInt32(data, 52);
    //        this.ProductKey = Encoding.ASCII.GetString(data, 56, (int)this.DataLength);
    //    }

    //    public UInt32 Version { get; private set; }
    //    public UInt32 Reserved { get; private set; }
    //    public UInt32 DataType { get; private set; }
    //    public UInt32 DataReserved { get; private set; }
    //    public UInt32 DataLength { get; private set; }
    //    public String ProductKey { get; private set; }
    //}
}
