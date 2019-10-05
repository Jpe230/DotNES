using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNes.NES
{
    public class Mapper000 : Mapper
    {
        public Mapper000(byte prgBanks, byte chrBanks) : base(prgBanks, chrBanks)
        {
            this.nPRGBanks = prgBanks;
            this.nCHRBanks = chrBanks;
        }

        override public bool cpuMapRead(ushort addr, ref uint mapped_addr)
        {
           
            if (addr >= 0x8000 && addr <= 0xFFFF)
            {
                mapped_addr = (uint)(addr & (nPRGBanks > 1 ? 0x7FFF : 0x3FFF));
                return true;
            }

            return false;
        }

        override public bool cpuMapWrite(ushort addr,ref uint mapped_addr)
        {
            if (addr >= 0x8000 && addr <= 0xFFFF)
            {
                mapped_addr = (uint)(addr & (nPRGBanks > 1 ? 0x7FFF : 0x3FFF));
                return true;
            }

            return false;
        }

        override public bool ppuMapRead(ushort addr, ref uint mapped_addr)
        {
          
            if (addr >= 0x0000 && addr <= 0x1FFF)
            {
                mapped_addr = addr;
                return true;
            }

            return false;
        }

        override public bool ppuMapWrite(ushort addr, ref uint mapped_addr)
        {
            if (addr >= 0x0000 && addr <= 0x1FFF)
            {
                if (nCHRBanks == 0)
                {
                    mapped_addr = addr;
                    return true;
                }
            }

            return false;
        }
    }
}
