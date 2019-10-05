using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNes.NES
{
    public abstract class Mapper
    {

        protected byte nPRGBanks = 0;
        protected byte nCHRBanks = 0;


        public Mapper(byte prgBanks, byte chrBanks)
        {
            nPRGBanks = prgBanks;
            nCHRBanks = chrBanks;
        }

        public virtual bool cpuMapRead(ushort addr, ref uint mapped_addr)
        {
            return false;
        }
        public virtual bool cpuMapWrite(ushort addr, ref uint mapped_addr)
        {
            return false;
        }
        public virtual bool ppuMapRead(ushort addr, ref uint mapped_addr)
        {
            return false;
        }
        public virtual bool ppuMapWrite(ushort addr, ref uint mapped_addr)
        {
            return false;
        }


    }
}
