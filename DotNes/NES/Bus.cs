using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNes.NES
{
    public class Bus
    {

        //Devices
        public CPU6502 CPU = new CPU6502();
        public PPU_2C02 ppu;
        public Cartridge cart;
        public byte[] cpuRam = new byte[2048];
        uint nSystemClockCounter = 0;

        public Bus(GraphicsDevice gd)
        {
            CPU.ConnectBus(this);
            ppu = new PPU_2C02(gd);
            
        }

        public void InsertCartridge(Cartridge cartridge)
        {
	        // Connects cartridge to both Main Bus and CPU Bus
	        this.cart = cartridge;
	        ppu.ConnectCartridge(cartridge);
        }

        public void Reset()
        {
            CPU.Reset();
            nSystemClockCounter = 0;

        }

        public void Clock()
        {
            ppu.clock();

            if (nSystemClockCounter % 3 == 0)
            {
                CPU.Clock();
            }


            nSystemClockCounter++;
        }

        public void cpuWrite(ushort addr, byte data)
        {
            if (cart.cpuWrite(addr, data))
            {
              
            }
            else if (addr >= 0x0000 && addr <= 0x1FFF)
            {
                cpuRam[addr & 0x07FF] = data;
            }
            else if (addr >= 0x2000 && addr <= 0x3FFF)
            {
                ppu.cpuWrite((ushort)(addr & 0x0007), data);
            }

        }

        public byte cpuRead(ushort addr, bool bReadOnly = false)
        {
            byte data = 0x00;
            if (cart.cpuRead(addr, ref data))
            {
              
            }
            else if (addr >= 0x0000 && addr <= 0x1FFF)
            {
                data = cpuRam[addr & 0x07FF];
            }
            else if (addr >= 0x2000 && addr <= 0x3FFF)
            {
                data = ppu.cpuRead((ushort)(addr & 0x0007), bReadOnly);
            }

            return data;
        }
    }
}
