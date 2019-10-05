using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DotNes.NES
{
    public class PPU_2C02
    {
        public bool frame_complete = false;
        RNGCryptoServiceProvider rnd;

        Cartridge cart;

        Color[] palScreen = new Color[0x40];
        Texture2D sprScreen;
        Texture2D[] sprNameTable;
        Texture2D[] sprPatternTable;
        

        byte[][] tblName = new byte[2][];
        byte[][] tblPattern = new byte[2][];
        byte[] tblPallet  = new byte[32];

        short scanline = 0;
        ushort cycle = 0;

        private RandomGen randomGen;
        private Color[] _Buffer = new Color[256 * 240];

        private Color[][] _BufferNameTable = new Color[2][];
        private Color[][] _BufferPatternTable = new Color[2][];

        private bool isWhite = false;

        private BitField _status = new BitField();
        private BitField _mask = new BitField();
        private BitField _control = new BitField();


        byte address_latch = 0x00;
        byte ppu_data_buffer = 0x00;
        ushort ppu_address = 0x0000;



        

        public PPU_2C02(GraphicsDevice gd)
        {

            randomGen = new RandomGen();

            rnd = new RNGCryptoServiceProvider();

            sprScreen = new Texture2D(gd, 256, 240);
            sprNameTable = new Texture2D[] { new Texture2D(gd, 256, 240), new Texture2D(gd, 256, 240) };
            sprPatternTable = new Texture2D[] { new Texture2D(gd, 128, 128), new Texture2D(gd, 128, 128) };

            for (int x = 0; x < 2; x++)
            {
                tblName[x] = new byte[1024];
                tblPattern[x] = new byte[1024];
                _BufferNameTable[x] = new Color[256 * 240];
                _BufferPatternTable[x] = new Color[128*128];
            }

            

            palScreen[0x00] = new Color(84 , 84, 84);
            palScreen[0x01] = new Color(0, 30, 116);
            palScreen[0x02] = new Color(8, 16, 144);
            palScreen[0x03] = new Color(48, 0, 136);
            palScreen[0x04] = new Color(68, 0, 100);
            palScreen[0x05] = new Color(92, 0, 48);
            palScreen[0x06] = new Color(84, 4, 0);
            palScreen[0x07] = new Color(60, 24, 0);
            palScreen[0x08] = new Color(32, 42, 0);
            palScreen[0x09] = new Color(8, 58, 0);
            palScreen[0x0A] = new Color(0, 64, 0);
            palScreen[0x0B] = new Color(0, 60, 0);
            palScreen[0x0C] = new Color(0, 50, 60);
            palScreen[0x0D] = new Color(0, 0, 0);
            palScreen[0x0E] = new Color(0, 0, 0);
            palScreen[0x0F] = new Color(0, 0, 0);

            palScreen[0x10] = new Color(152, 150, 152);
            palScreen[0x11] = new Color(8, 76, 196);
            palScreen[0x12] = new Color(48, 50, 236);
            palScreen[0x13] = new Color(92, 30, 228);
            palScreen[0x14] = new Color(136, 20, 176);
            palScreen[0x15] = new Color(160, 20, 100);
            palScreen[0x16] = new Color(152, 34, 32);
            palScreen[0x17] = new Color(120, 60, 0);
            palScreen[0x18] = new Color(84, 90, 0);
            palScreen[0x19] = new Color(40, 114, 0);
            palScreen[0x1A] = new Color(8, 124, 0);
            palScreen[0x1B] = new Color(0, 118, 40);
            palScreen[0x1C] = new Color(0, 102, 120);
            palScreen[0x1D] = new Color(0, 0, 0);
            palScreen[0x1E] = new Color(0, 0, 0);
            palScreen[0x1F] = new Color(0, 0, 0);

            palScreen[0x20] = new Color(236, 238, 236);
            palScreen[0x21] = new Color(76, 154, 236);
            palScreen[0x22] = new Color(120, 124, 236);
            palScreen[0x23] = new Color(176, 98, 236);
            palScreen[0x24] = new Color(228, 84, 236);
            palScreen[0x25] = new Color(236, 88, 180);
            palScreen[0x26] = new Color(236, 106, 100);
            palScreen[0x27] = new Color(212, 136, 32);
            palScreen[0x28] = new Color(160, 170, 0);
            palScreen[0x29] = new Color(116, 196, 0);
            palScreen[0x2A] = new Color(76, 208, 32);
            palScreen[0x2B] = new Color(56, 204, 108);
            palScreen[0x2C] = new Color(56, 180, 204);
            palScreen[0x2D] = new Color(60, 60, 60);
            palScreen[0x2E] = new Color(0, 0, 0);
            palScreen[0x2F] = new Color(0, 0, 0);

            palScreen[0x30] = new Color(236, 238, 236);
            palScreen[0x31] = new Color(168, 204, 236);
            palScreen[0x32] = new Color(188, 188, 236);
            palScreen[0x33] = new Color(212, 178, 236);
            palScreen[0x34] = new Color(236, 174, 236);
            palScreen[0x35] = new Color(236, 174, 212);
            palScreen[0x36] = new Color(236, 180, 176);
            palScreen[0x37] = new Color(228, 196, 144);
            palScreen[0x38] = new Color(204, 210, 120);
            palScreen[0x39] = new Color(180, 222, 120);
            palScreen[0x3A] = new Color(168, 226, 144);
            palScreen[0x3B] = new Color(152, 226, 180);
            palScreen[0x3C] = new Color(160, 214, 228);
            palScreen[0x3D] = new Color(160, 162, 160);
            palScreen[0x3E] = new Color(0, 0, 0);
            palScreen[0x3F] = new Color(0, 0, 0);
        }

        public Texture2D GetScreen()
        {
            return sprScreen;
        }

        private Color GetColorFromPalleteRam(byte pallete, byte pixel)
        {
            ushort tmpaddr = (ushort)(0x3F00 + (pallete << 2 + pixel));
            int a = ppuRead((ushort)tmpaddr);

            return palScreen[a];
        }

        Texture2D GetNameTable(byte i)
        {
            return sprNameTable[i];
        }

        public Texture2D GetPatternTable(byte i, byte palette)
        {

            for(int nTiley = 0; nTiley < 16; nTiley++)
            {
                for(int nTilex = 0; nTilex < 16; nTilex++)
                {
                    int offset = nTiley * 256 + nTilex * 16;

                    for(int row = 0; row < 8; row++)
                    {

                        byte tile_lsb = ppuRead((ushort)(i * 0x1000 + offset + row + 0));
                        byte tile_msb = ppuRead((ushort)(i * 0x1000 + offset + row + 8));

                        for (int col = 0; col < 8; col++)
                        {
                            byte pixel = (byte)((tile_lsb & 0x01) + (tile_msb & 0x01));

                            if (pixel != 0)
                            {
                                int ha = 0;
                            }
                            tile_lsb >>= 1;
                            tile_msb >>= 1;

                            int _row = (nTiley * 8 + row) * 128;
                            int _col = (nTilex * 8 + (7 - col));

                            var _tmp = GetColorFromPalleteRam((byte)palette, pixel);
                            _BufferPatternTable[i][(_row + _col)] = _tmp;
                        }
                    }
                }
            }

            var x = 0;                  

            sprPatternTable[i].SetData<Color>(0, new Rectangle(0, 0, 128, 128), _BufferPatternTable[i], 0, 128 * 128);

            return sprPatternTable[i];

        }

        public void ConnectCartridge(Cartridge cart)
        {
            this.cart = cart;
        }


        public byte cpuRead(ushort addr, bool rdonly)
        {
            byte data = 0x00;

            switch (addr)
            {
                case 0x0000: // Control
                    
                    break;
                case 0x0001: // Mask
                    break;
                case 0x0002: // Status
                    _status.s[7] = true;
                    data = (byte)((_status.w & 0xE0) | (ppu_data_buffer & 0x1f));
                    _status.s[7] = false;
                    address_latch = 0;
                    break;
                case 0x0003: // OAM Address
                    break;
                case 0x0004: // OAM Data
                    break;
                case 0x0005: // Scroll
                    break;
                case 0x0006: // PPU Address

                    break;
                case 0x0007: // PPU Data
                    data = ppu_data_buffer;
                    ppu_data_buffer = ppuRead(ppu_address);

                    if (ppu_address > 0x3f00) data = ppu_data_buffer;
                    break;
            }

            return data;
        }

        public void cpuWrite(ushort addr, byte data)
        {
            switch (addr)
            {
                case 0x0000: // Control
                    _control.w = data;
                    break;
                case 0x0001: // Mask
                    _mask.w = data;
                    break;
                case 0x0002: // Status
                    break;
                case 0x0003: // OAM Address
                    break;
                case 0x0004: // OAM Data
                    break;
                case 0x0005: // Scroll
                    break;
                case 0x0006: // PPU Address
                    if(address_latch == 0)
                    {
                        ppu_address = (ushort)((ppu_address & 0x00FF) | data);
                        address_latch = 1;
                    }
                    else
                    {
                        ppu_address = (ushort)((ppu_address & 0xFF00) | data);
                        address_latch = 0;
                    }
                    break;
                case 0x0007: // PPU Data
                    ppuWrite(ppu_address, data);
                    break;
            }
        }

        public byte ppuRead(ushort addr, bool rdonly = false)
        {
            byte data = 0x00;
            addr &= 0x3FFF;

            if (cart.ppuRead(addr, ref data))
            {

            }
            else if(addr >= 0x0000 && addr <= 0x1FFF)
            {
                addr &= 0x0FFF;
                data = tblPattern[(addr & 0x1000) >> 12][addr & 0x0FFF];
            }
            else if (addr >= 0x2000 && addr <= 0x3EFF)
            {

            }
            else if (addr >= 0x3F00 && addr <= 0x3FFF)
            {
                addr &= 0x001f;
                if (addr == 0x0010) addr = 0x0000;
                if (addr == 0x0014) addr = 0x0004;
                if (addr == 0x0018) addr = 0x0008;
                if (addr == 0x001C) addr = 0x000C;
                data = tblPallet[addr];
            }

            return data;
        }

        public void ppuWrite(ushort addr, byte data)
        {
            addr &= 0x3FFF;

            if (cart.ppuWrite(addr, data))
            {

            }
            else if (addr >= 0x0000 && addr <= 0x1FFF)
            {
                tblPattern[(addr & 0x1000) >> 12][addr & 0x0FFF] = data;
            }
            else if (addr >= 0x2000 && addr <= 0x3EFF)
            {

            }
            else if (addr >= 0x3F00 && addr <= 0x3FFF)
            {
                addr &= 0x001f;
                if (addr == 0x0010) addr = 0x0000;
                if (addr == 0x0014) addr = 0x0004;
                if (addr == 0x0018) addr = 0x0008;
                if (addr == 0x001C) addr = 0x000C;
                tblPallet[addr] = data;
            }
        }


       



        public void clock()
        {

            


            //_Buffer[cycle + (scanline * 256)] = c;
           /*
            if(cycle >= 0 && cycle <= 255 && scanline >= 0 && scanline <= 239)
                _Buffer[cycle + (scanline * 256)] = c
           */

            //Advance renderer - it never stops, it's relentless
            cycle++;
            if (cycle >= 341)
            {
                cycle = 0;
                scanline++;
                if (scanline >= 261)
                {
                    scanline = -1;
                    sprScreen.SetData<Color>(0, new Rectangle(0, 0, 256, 240), _Buffer, 0, 256*240);
                    frame_complete = true;
                }
            }
        }


        #region 8BitField
        struct _8BitField
        {
            byte bits;
            public bool this[int i]
            {
                get
                {
                    return (bits & (1 << i)) != 0;
                }

                set
                {
                    if (value)
                    {
                        bits |= (byte)(1 << i);
                    }

                    else
                    {
                        bits &= (byte)~(1 << i);
                    }
                }
            }
        }



        [StructLayout(LayoutKind.Explicit)]
        struct BitField
        {
            [FieldOffset(0)]
            public _8BitField s;

            [FieldOffset(0)]
            public byte w;

        }
        #endregion




    }
}
