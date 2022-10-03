using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncoderConsoleApp.TratamentoRuido
{
    public enum CRC8_POLY
    {
        CRC8 = 0xd5,
        CRC8_CCITT = 0x07,
        CRC8_DALLAS_MAXIM = 0x31,
        CRC8_SAE_J1850 = 0x1D,
        CRC_8_WCDMA = 0x9b,
    };

    public class CRC8Calc
    {
        private byte[] table = new byte[256];

        public byte Checksum(params byte[] val)
        {
            if (val == null)
                throw new ArgumentNullException("val");

            byte c = 0;

            foreach (byte b in val)
            {
                c = table[c ^ b];
            }

            return c;
        }

        public byte[] Table
        {
            get
            {
                return this.table;
            }
            set
            {
                this.table = value;
            }
        }

        public byte[] GenerateTable(CRC8_POLY polynomial)
        {
            byte[] csTable = new byte[256];

            for (int i = 0; i < 256; ++i)
            {
                int curr = i;

                for (int j = 0; j < 8; ++j)
                {
                    if ((curr & 0x80) != 0)
                    {
                        curr = (curr << 1) ^ (int)polynomial;
                    }
                    else
                    {
                        curr <<= 1;
                    }
                }

                csTable[i] = (byte)curr;
            }

            return csTable;
        }

        public CRC8Calc(CRC8_POLY polynomial)
        {
            this.table = this.GenerateTable(polynomial);
        }

        public void teste() {

            byte checksum;
            byte[] testVal = new byte[]
            {0xee, 0x01, 0x13, 0x00, 0x06, 0x1c, 0x00, 0x20,  0x1d, 0x00, 0x00};
            CRC8Calc crc_dallas = new CRC8Calc(CRC8_POLY.CRC8_DALLAS_MAXIM);
            checksum = crc_dallas.Checksum(testVal);
        }
    }
}
