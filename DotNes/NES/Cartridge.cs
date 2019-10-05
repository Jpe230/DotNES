using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNes.NES
{
    struct sHeader
    {
        public char[] name;
        public byte prg_rom_chunks;
        public byte chr_rom_chunks;
        public byte mapper1;
        public byte mapper2;
        public byte prg_ram_size;
        public byte tv_system1;
        public byte tv_system2;
        public char[] unused;
    }

    public class Cartridge
    {
        public enum MIRROR
        {
            HORIZONTAL,
            VERTICAL,
            ONESCREEN_LO,
            ONESCREEN_HI,
        }

        public MIRROR mirror = MIRROR.HORIZONTAL;

        bool bImageValid = false;
        byte nMapperID = 0;
        byte nPRGBanks = 0;
        byte nCHRBanks = 0;

        List<byte> vPRGMemory = new List<byte>();
        List<byte> vCHRMemory = new List<byte>();

        Mapper pMapper;

        public Cartridge(string sFileName)
        {
            sHeader header = new sHeader();
            header.name = new char[4];
            header.unused = new char[5];
            bImageValid = false;


            BinaryReader br;
            try
            {
                br = new BinaryReader(new FileStream(sFileName, FileMode.Open));
            }
            catch (IOException e)
            {
                return;
            }

            header.name = Encoding.ASCII.GetChars(br.ReadBytes(4));
            header.prg_rom_chunks = br.ReadByte();
            header.chr_rom_chunks = br.ReadByte();
            header.mapper1 = br.ReadByte();
            header.mapper2 = br.ReadByte();
            header.prg_ram_size = br.ReadByte();
            header.tv_system1 = br.ReadByte();
            header.tv_system2 = br.ReadByte();
            header.unused = Encoding.ASCII.GetChars(br.ReadBytes(5));

            if ((header.mapper1 & 0x04) != 0)
                br.BaseStream.Seek(512, SeekOrigin.Current);

            nMapperID = (byte)(((header.mapper2 >> 4) << 4) | (header.mapper1 >> 4));
            mirror = ((header.mapper1 & 0x01) != 0) ? MIRROR.VERTICAL : MIRROR.HORIZONTAL;

            byte nFileType = 1;

            if (nFileType == 1)
            {
                nPRGBanks = header.prg_rom_chunks;
                vPRGMemory = new List<byte>(nPRGBanks * 16384);
                vPRGMemory = br.ReadBytes(vPRGMemory.Capacity).ToList();

                nCHRBanks = header.chr_rom_chunks;
                vCHRMemory = new List<byte>(nCHRBanks * 8192);
                vCHRMemory = br.ReadBytes(vCHRMemory.Capacity).ToList();
                //ifs.read((char*)vCHRMemory.data(), vCHRMemory.size());
            }

            switch (nMapperID)
            {
                case 0: pMapper = new Mapper000(nPRGBanks, nCHRBanks); break;
            }


            bImageValid = true;
            br.Close();
        }

        public bool ImageValid()
        {
            return bImageValid;
        }
        
        public bool cpuRead(ushort addr, ref byte data)
        {
            uint mapped_addr = 0;
            if (pMapper.cpuMapRead(addr, ref mapped_addr))
            {
                data = vPRGMemory[(int)mapped_addr];
                return true;
            }
            else return false;
        }
        public bool cpuWrite(ushort addr, byte data)
        {
            uint mapped_addr = 0;
            if (pMapper.cpuMapWrite(addr, ref mapped_addr))
            {
                data = vPRGMemory[(int)mapped_addr];
                return true;
            }
            else return false;
        }
        public bool ppuRead(ushort addr, ref byte data)
        {
            uint mapped_addr = 0;
            if (pMapper.ppuMapRead(addr, ref mapped_addr))
            {
                data = vCHRMemory[(int)mapped_addr];
                return true;
            }
            else return false;
        }
        public bool ppuWrite(ushort addr, byte data)
        {
            uint mapped_addr = 0;
            if (pMapper.ppuMapWrite(addr, ref mapped_addr))
            {
                data = vCHRMemory[(int)mapped_addr];
                return true;
            }
            else return false;
        }



    }
}
