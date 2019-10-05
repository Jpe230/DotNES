using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNes.NES
{
    public delegate Byte Operate();
    public delegate Byte AddrMode();

    public class CPU6502
    {
        private Bus bus;

        public byte a = 0x00;
        public byte x = 0x00;
        public byte y = 0x00;
        public byte status = 0x00;
        public byte stkp = 0x00;
        public ushort pc = 0x0000;

        private byte fetched = 0x00;
        private ushort temp = 0x0000;
        private ushort addr_abs = 0x0000;
        private ushort addr_rel = 0x00;
        private byte opcode = 0x00;
        private byte cycles = 0x00;
        private byte clock_count = 0;

        private Instruction[] instructions;
        

        public void ConnectBus(Bus _bus)
        {
            this.bus = _bus;
        }


        public CPU6502()
        {
            InitializeInstructions();
           
        }

        void InitializeInstructions()
        {
            instructions = new Instruction[]
            {
                new Instruction( "BRK", new Operate(BRK), new AddrMode(IMM), 7 ), new Instruction( "ORA", new Operate(ORA), new AddrMode(IZX), 6 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 2 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 8 ), new Instruction( "???", new Operate(NOP), new AddrMode(IMP), 3 ), new Instruction( "ORA", new Operate(ORA), new AddrMode(ZP0), 3 ), new Instruction( "ASL", new Operate(ASL), new AddrMode(ZP0), 5 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 5 ), new Instruction( "PHP", new Operate(PHP), new AddrMode(IMP), 3 ), new Instruction( "ORA", new Operate(ORA), new AddrMode(IMM), 2 ), new Instruction( "ASL", new Operate(ASL), new AddrMode(IMP), 2 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 2 ), new Instruction( "???", new Operate(NOP), new AddrMode(IMP), 4 ), new Instruction( "ORA", new Operate(ORA), new AddrMode(ABS), 4 ), new Instruction( "ASL", new Operate(ASL), new AddrMode(ABS), 6 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 6 ),
                new Instruction( "BPL", new Operate(BPL), new AddrMode(REL), 2 ), new Instruction( "ORA", new Operate(ORA), new AddrMode(IZY), 5 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 2 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 8 ), new Instruction( "???", new Operate(NOP), new AddrMode(IMP), 4 ), new Instruction( "ORA", new Operate(ORA), new AddrMode(ZPX), 4 ), new Instruction( "ASL", new Operate(ASL), new AddrMode(ZPX), 6 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 6 ), new Instruction( "CLC", new Operate(CLC), new AddrMode(IMP), 2 ), new Instruction( "ORA", new Operate(ORA), new AddrMode(ABY), 4 ), new Instruction( "???", new Operate(NOP), new AddrMode(IMP), 2 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 7 ), new Instruction( "???", new Operate(NOP), new AddrMode(IMP), 4 ), new Instruction( "ORA", new Operate(ORA), new AddrMode(ABX), 4 ), new Instruction( "ASL", new Operate(ASL), new AddrMode(ABX), 7 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 7 ),
                new Instruction( "JSR", new Operate(JSR), new AddrMode(ABS), 6 ), new Instruction( "AND", new Operate(AND), new AddrMode(IZX), 6 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 2 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 8 ), new Instruction( "BIT", new Operate(BIT), new AddrMode(ZP0), 3 ), new Instruction( "AND", new Operate(AND), new AddrMode(ZP0), 3 ), new Instruction( "ROL", new Operate(ROL), new AddrMode(ZP0), 5 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 5 ), new Instruction( "PLP", new Operate(PLP), new AddrMode(IMP), 4 ), new Instruction( "AND", new Operate(AND), new AddrMode(IMM), 2 ), new Instruction( "ROL", new Operate(ROL), new AddrMode(IMP), 2 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 2 ), new Instruction( "BIT", new Operate(BIT), new AddrMode(ABS), 4 ), new Instruction( "AND", new Operate(AND), new AddrMode(ABS), 4 ), new Instruction( "ROL", new Operate(ROL), new AddrMode(ABS), 6 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 6 ),
                new Instruction( "BMI", new Operate(BMI), new AddrMode(REL), 2 ), new Instruction( "AND", new Operate(AND), new AddrMode(IZY), 5 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 2 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 8 ), new Instruction( "???", new Operate(NOP), new AddrMode(IMP), 4 ), new Instruction( "AND", new Operate(AND), new AddrMode(ZPX), 4 ), new Instruction( "ROL", new Operate(ROL), new AddrMode(ZPX), 6 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 6 ), new Instruction( "SEC", new Operate(SEC), new AddrMode(IMP), 2 ), new Instruction( "AND", new Operate(AND), new AddrMode(ABY), 4 ), new Instruction( "???", new Operate(NOP), new AddrMode(IMP), 2 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 7 ), new Instruction( "???", new Operate(NOP), new AddrMode(IMP), 4 ), new Instruction( "AND", new Operate(AND), new AddrMode(ABX), 4 ), new Instruction( "ROL", new Operate(ROL), new AddrMode(ABX), 7 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 7 ),
                new Instruction( "RTI", new Operate(RTI), new AddrMode(IMP), 6 ), new Instruction( "EOR", new Operate(EOR), new AddrMode(IZX), 6 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 2 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 8 ), new Instruction( "???", new Operate(NOP), new AddrMode(IMP), 3 ), new Instruction( "EOR", new Operate(EOR), new AddrMode(ZP0), 3 ), new Instruction( "LSR", new Operate(LSR), new AddrMode(ZP0), 5 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 5 ), new Instruction( "PHA", new Operate(PHA), new AddrMode(IMP), 3 ), new Instruction( "EOR", new Operate(EOR), new AddrMode(IMM), 2 ), new Instruction( "LSR", new Operate(LSR), new AddrMode(IMP), 2 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 2 ), new Instruction( "JMP", new Operate(JMP), new AddrMode(ABS), 3 ), new Instruction( "EOR", new Operate(EOR), new AddrMode(ABS), 4 ), new Instruction( "LSR", new Operate(LSR), new AddrMode(ABS), 6 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 6 ),
                new Instruction( "BVC", new Operate(BVC), new AddrMode(REL), 2 ), new Instruction( "EOR", new Operate(EOR), new AddrMode(IZY), 5 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 2 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 8 ), new Instruction( "???", new Operate(NOP), new AddrMode(IMP), 4 ), new Instruction( "EOR", new Operate(EOR), new AddrMode(ZPX), 4 ), new Instruction( "LSR", new Operate(LSR), new AddrMode(ZPX), 6 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 6 ), new Instruction( "CLI", new Operate(CLI), new AddrMode(IMP), 2 ), new Instruction( "EOR", new Operate(EOR), new AddrMode(ABY), 4 ), new Instruction( "???", new Operate(NOP), new AddrMode(IMP), 2 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 7 ), new Instruction( "???", new Operate(NOP), new AddrMode(IMP), 4 ), new Instruction( "EOR", new Operate(EOR), new AddrMode(ABX), 4 ), new Instruction( "LSR", new Operate(LSR), new AddrMode(ABX), 7 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 7 ),
                new Instruction( "RTS", new Operate(RTS), new AddrMode(IMP), 6 ), new Instruction( "ADC", new Operate(ADC), new AddrMode(IZX), 6 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 2 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 8 ), new Instruction( "???", new Operate(NOP), new AddrMode(IMP), 3 ), new Instruction( "ADC", new Operate(ADC), new AddrMode(ZP0), 3 ), new Instruction( "ROR", new Operate(ROR), new AddrMode(ZP0), 5 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 5 ), new Instruction( "PLA", new Operate(PLA), new AddrMode(IMP), 4 ), new Instruction( "ADC", new Operate(ADC), new AddrMode(IMM), 2 ), new Instruction( "ROR", new Operate(ROR), new AddrMode(IMP), 2 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 2 ), new Instruction( "JMP", new Operate(JMP), new AddrMode(IND), 5 ), new Instruction( "ADC", new Operate(ADC), new AddrMode(ABS), 4 ), new Instruction( "ROR", new Operate(ROR), new AddrMode(ABS), 6 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 6 ),
                new Instruction( "BVS", new Operate(BVS), new AddrMode(REL), 2 ), new Instruction( "ADC", new Operate(ADC), new AddrMode(IZY), 5 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 2 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 8 ), new Instruction( "???", new Operate(NOP), new AddrMode(IMP), 4 ), new Instruction( "ADC", new Operate(ADC), new AddrMode(ZPX), 4 ), new Instruction( "ROR", new Operate(ROR), new AddrMode(ZPX), 6 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 6 ), new Instruction( "SEI", new Operate(SEI), new AddrMode(IMP), 2 ), new Instruction( "ADC", new Operate(ADC), new AddrMode(ABY), 4 ), new Instruction( "???", new Operate(NOP), new AddrMode(IMP), 2 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 7 ), new Instruction( "???", new Operate(NOP), new AddrMode(IMP), 4 ), new Instruction( "ADC", new Operate(ADC), new AddrMode(ABX), 4 ), new Instruction( "ROR", new Operate(ROR), new AddrMode(ABX), 7 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 7 ),
                new Instruction( "???", new Operate(NOP), new AddrMode(IMP), 2 ), new Instruction( "STA", new Operate(STA), new AddrMode(IZX), 6 ), new Instruction( "???", new Operate(NOP), new AddrMode(IMP), 2 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 6 ), new Instruction( "STY", new Operate(STY), new AddrMode(ZP0), 3 ), new Instruction( "STA", new Operate(STA), new AddrMode(ZP0), 3 ), new Instruction( "STX", new Operate(STX), new AddrMode(ZP0), 3 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 3 ), new Instruction( "DEY", new Operate(DEY), new AddrMode(IMP), 2 ), new Instruction( "???", new Operate(NOP), new AddrMode(IMP), 2 ), new Instruction( "TXA", new Operate(TXA), new AddrMode(IMP), 2 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 2 ), new Instruction( "STY", new Operate(STY), new AddrMode(ABS), 4 ), new Instruction( "STA", new Operate(STA), new AddrMode(ABS), 4 ), new Instruction( "STX", new Operate(STX), new AddrMode(ABS), 4 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 4 ),
                new Instruction( "BCC", new Operate(BCC), new AddrMode(REL), 2 ), new Instruction( "STA", new Operate(STA), new AddrMode(IZY), 6 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 2 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 6 ), new Instruction( "STY", new Operate(STY), new AddrMode(ZPX), 4 ), new Instruction( "STA", new Operate(STA), new AddrMode(ZPX), 4 ), new Instruction( "STX", new Operate(STX), new AddrMode(ZPY), 4 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 4 ), new Instruction( "TYA", new Operate(TYA), new AddrMode(IMP), 2 ), new Instruction( "STA", new Operate(STA), new AddrMode(ABY), 5 ), new Instruction( "TXS", new Operate(TXS), new AddrMode(IMP), 2 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 5 ), new Instruction( "???", new Operate(NOP), new AddrMode(IMP), 5 ), new Instruction( "STA", new Operate(STA), new AddrMode(ABX), 5 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 5 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 5 ),
                new Instruction( "LDY", new Operate(LDY), new AddrMode(IMM), 2 ), new Instruction( "LDA", new Operate(LDA), new AddrMode(IZX), 6 ), new Instruction( "LDX", new Operate(LDX), new AddrMode(IMM), 2 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 6 ), new Instruction( "LDY", new Operate(LDY), new AddrMode(ZP0), 3 ), new Instruction( "LDA", new Operate(LDA), new AddrMode(ZP0), 3 ), new Instruction( "LDX", new Operate(LDX), new AddrMode(ZP0), 3 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 3 ), new Instruction( "TAY", new Operate(TAY), new AddrMode(IMP), 2 ), new Instruction( "LDA", new Operate(LDA), new AddrMode(IMM), 2 ), new Instruction( "TAX", new Operate(TAX), new AddrMode(IMP), 2 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 2 ), new Instruction( "LDY", new Operate(LDY), new AddrMode(ABS), 4 ), new Instruction( "LDA", new Operate(LDA), new AddrMode(ABS), 4 ), new Instruction( "LDX", new Operate(LDX), new AddrMode(ABS), 4 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 4 ),
                new Instruction( "BCS", new Operate(BCS), new AddrMode(REL), 2 ), new Instruction( "LDA", new Operate(LDA), new AddrMode(IZY), 5 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 2 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 5 ), new Instruction( "LDY", new Operate(LDY), new AddrMode(ZPX), 4 ), new Instruction( "LDA", new Operate(LDA), new AddrMode(ZPX), 4 ), new Instruction( "LDX", new Operate(LDX), new AddrMode(ZPY), 4 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 4 ), new Instruction( "CLV", new Operate(CLV), new AddrMode(IMP), 2 ), new Instruction( "LDA", new Operate(LDA), new AddrMode(ABY), 4 ), new Instruction( "TSX", new Operate(TSX), new AddrMode(IMP), 2 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 4 ), new Instruction( "LDY", new Operate(LDY), new AddrMode(ABX), 4 ), new Instruction( "LDA", new Operate(LDA), new AddrMode(ABX), 4 ), new Instruction( "LDX", new Operate(LDX), new AddrMode(ABY), 4 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 4 ),
                new Instruction( "CPY", new Operate(CPY), new AddrMode(IMM), 2 ), new Instruction( "CMP", new Operate(CMP), new AddrMode(IZX), 6 ), new Instruction( "???", new Operate(NOP), new AddrMode(IMP), 2 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 8 ), new Instruction( "CPY", new Operate(CPY), new AddrMode(ZP0), 3 ), new Instruction( "CMP", new Operate(CMP), new AddrMode(ZP0), 3 ), new Instruction( "DEC", new Operate(DEC), new AddrMode(ZP0), 5 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 5 ), new Instruction( "INY", new Operate(INY), new AddrMode(IMP), 2 ), new Instruction( "CMP", new Operate(CMP), new AddrMode(IMM), 2 ), new Instruction( "DEX", new Operate(DEX), new AddrMode(IMP), 2 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 2 ), new Instruction( "CPY", new Operate(CPY), new AddrMode(ABS), 4 ), new Instruction( "CMP", new Operate(CMP), new AddrMode(ABS), 4 ), new Instruction( "DEC", new Operate(DEC), new AddrMode(ABS), 6 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 6 ),
                new Instruction( "BNE", new Operate(BNE), new AddrMode(REL), 2 ), new Instruction( "CMP", new Operate(CMP), new AddrMode(IZY), 5 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 2 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 8 ), new Instruction( "???", new Operate(NOP), new AddrMode(IMP), 4 ), new Instruction( "CMP", new Operate(CMP), new AddrMode(ZPX), 4 ), new Instruction( "DEC", new Operate(DEC), new AddrMode(ZPX), 6 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 6 ), new Instruction( "CLD", new Operate(CLD), new AddrMode(IMP), 2 ), new Instruction( "CMP", new Operate(CMP), new AddrMode(ABY), 4 ), new Instruction( "NOP", new Operate(NOP), new AddrMode(IMP), 2 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 7 ), new Instruction( "???", new Operate(NOP), new AddrMode(IMP), 4 ), new Instruction( "CMP", new Operate(CMP), new AddrMode(ABX), 4 ), new Instruction( "DEC", new Operate(DEC), new AddrMode(ABX), 7 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 7 ),
                new Instruction( "CPX", new Operate(CPX), new AddrMode(IMM), 2 ), new Instruction( "SBC", new Operate(SBC), new AddrMode(IZX), 6 ), new Instruction( "???", new Operate(NOP), new AddrMode(IMP), 2 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 8 ), new Instruction( "CPX", new Operate(CPX), new AddrMode(ZP0), 3 ), new Instruction( "SBC", new Operate(SBC), new AddrMode(ZP0), 3 ), new Instruction( "INC", new Operate(INC), new AddrMode(ZP0), 5 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 5 ), new Instruction( "INX", new Operate(INX), new AddrMode(IMP), 2 ), new Instruction( "SBC", new Operate(SBC), new AddrMode(IMM), 2 ), new Instruction( "NOP", new Operate(NOP), new AddrMode(IMP), 2 ), new Instruction( "???", new Operate(SBC), new AddrMode(IMP), 2 ), new Instruction( "CPX", new Operate(CPX), new AddrMode(ABS), 4 ), new Instruction( "SBC", new Operate(SBC), new AddrMode(ABS), 4 ), new Instruction( "INC", new Operate(INC), new AddrMode(ABS), 6 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 6 ),
                new Instruction( "BEQ", new Operate(BEQ), new AddrMode(REL), 2 ), new Instruction( "SBC", new Operate(SBC), new AddrMode(IZY), 5 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 2 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 8 ), new Instruction( "???", new Operate(NOP), new AddrMode(IMP), 4 ), new Instruction( "SBC", new Operate(SBC), new AddrMode(ZPX), 4 ), new Instruction( "INC", new Operate(INC), new AddrMode(ZPX), 6 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 6 ), new Instruction( "SED", new Operate(SED), new AddrMode(IMP), 2 ), new Instruction( "SBC", new Operate(SBC), new AddrMode(ABY), 4 ), new Instruction( "NOP", new Operate(NOP), new AddrMode(IMP), 2 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 7 ), new Instruction( "???", new Operate(NOP), new AddrMode(IMP), 4 ), new Instruction( "SBC", new Operate(SBC), new AddrMode(ABX), 4 ), new Instruction( "INC", new Operate(INC), new AddrMode(ABX), 7 ), new Instruction( "???", new Operate(XXX), new AddrMode(IMP), 7 )
            };
        }


        public byte Read(ushort addr)
        {
            return bus.cpuRead(addr, false);
        }

        public void Write(ushort addr, byte data)
        {
            bus.cpuWrite(addr, data);
        }

        public byte GetFlag(FLAGS6502 f)
        {
            byte b = (byte)f;
            byte a = (byte)(status & b);

            return (byte)((a > 0) ? 1 : 0);
        }

        public void SetFlag(FLAGS6502 f, bool v)
        {
            if (v)
                status |= (byte)f;
            else
                status &= (byte)~f;
        }

        public void SetFlag(FLAGS6502 f, int v)
        {
            if (v != 0)
                status |= (byte)f;
            else
                status &= (byte)~f;
        }


        //ADDRESSING MODE
        #region ADDRESSING MODE
        public byte IMP() {
            fetched = a;
            return 0x00;
        }
        public byte IMM() {
            addr_abs = pc++;
            return 0x00;
        }
        public byte ZP0() {
            addr_abs = Read(pc);
            pc++;
            addr_abs &= 0x00FF;
            return 0x00;
        }
        public byte ZPX() {
            addr_abs = (ushort)(Read(pc) + x);
            pc++;
            addr_abs &= 0x00FF;
            return 0x00;
        }
        public byte ZPY() {
            addr_abs = (ushort)(Read(pc) + y);
            pc++;
            addr_abs &= 0x00FF;
            return 0x00;
        }
        public byte REL() {
            addr_rel = Read(pc);
            pc++;
            if ((ushort)(addr_rel & 0x80) != 0)
                addr_rel |= 0xFF00;
            return 0x00;
        }
        public byte ABS() {
            ushort lo = Read(pc);
            pc++;
            ushort hi = Read(pc);
            pc++;

            addr_abs = (ushort)((hi << 8) | lo);
            return 0x00;
        }
        public byte ABX() {
            ushort lo = Read(pc);
            pc++;
            ushort hi = Read(pc);
            pc++;

            addr_abs = (ushort)((hi << 8) | lo);
            addr_abs += x;

            if ((addr_abs & 0xFF00) != (hi << 8))
                return 1;
            else
                return 0;
        }
        public byte ABY() {
            ushort lo = Read(pc);
            pc++;
            ushort hi = Read(pc);
            pc++;

            addr_abs = (ushort)((hi << 8) | lo);
            addr_abs += y;

            if ((addr_abs & 0xFF00) != (hi << 8))
                return 1;
            else
                return 0;
           
        }
        public byte IND() {
            ushort ptr_lo = Read(pc);
            pc++;
            ushort ptr_hi = Read(pc);
            pc++;

            ushort ptr = (ushort)((ptr_hi << 8) | ptr_lo);

            if (ptr_lo == 0x00FF)
            {
                addr_abs = (ushort)((Read((ushort)(ptr & 0xFF00)) << 8) | Read((ushort)(ptr + 0)));
            }
            else
            {
                addr_abs = (ushort)((Read((ushort)(ptr +1)) << 8) | Read((ushort)(ptr + 0)));
            }
            return 0x00;
        }
        public byte IZX() {
            ushort t = Read(pc);
            pc++;

            ushort lo = Read((ushort)((t + (ushort)x) & 0x00FF));
            ushort hi = Read((ushort)((t + (ushort)x + 1) & 0x00FF));

            addr_abs = (ushort)((hi << 8) | lo);

            return 0;
        }
        public byte IZY() {
            ushort t = Read(pc);
            pc++;

            ushort lo = Read((ushort)((t + (ushort)y) & 0x00FF));
            ushort hi = Read((ushort)((t + (ushort)y + 1) & 0x00FF));

            addr_abs = (ushort)((hi << 8) | lo);
            return 0;
        }

        #endregion

        //OPCODES
        #region OPCODES
        public byte ADC() {
            Fetch();
            temp = (ushort)(a + fetched + GetFlag(FLAGS6502.C));
            SetFlag(FLAGS6502.C, temp > 255);
            SetFlag(FLAGS6502.Z, (temp & 0x00FF) == 0x00);
            SetFlag(FLAGS6502.V, (~((ushort)a ^ (ushort)fetched) & ((ushort)a ^ (ushort)temp)) & 0x0080);
            SetFlag(FLAGS6502.N, temp & 0x80);
            a = (byte)(temp & 0x00FF);
            return 1;
        }
        public byte AND() {
            Fetch();
            a = (byte)(a & fetched);
            SetFlag(FLAGS6502.Z, a == 0x00);
            SetFlag(FLAGS6502.N, a & 0x80);
            return 1;
        }
        public byte ASL() {
            Fetch();
            temp = (ushort)(fetched << 1);
            SetFlag(FLAGS6502.C, (temp & 0xFF00) > 0);
            SetFlag(FLAGS6502.Z, (temp & 0x00FF) == 0x00);
            SetFlag(FLAGS6502.N, temp & 0x80);

            AddrMode addr = new AddrMode(IMP);

            if (instructions[opcode].addrMode.Equals(addr))
                a = (byte)(temp & 0x00FF);
            else
                Write(addr_abs, (byte)(temp & 0x00FF));
            return 0;
        }
        public byte BCC() {
            if (GetFlag(FLAGS6502.C) == 0)
            {
                cycles++;
                addr_abs = (ushort)(pc + addr_rel);

                if ((addr_abs & 0xFF00) != (pc & 0xFF00))
                    cycles++;

                pc = addr_abs;
            }
            return 0;
        }
        public byte BCS() {
            if (GetFlag(FLAGS6502.C) == 1)
            {
                cycles++;
                addr_abs = (ushort)(pc + addr_rel);

                if ((addr_abs & 0xFF00) != (pc & 0xFF00))
                    cycles++;

                pc = addr_abs;
            }
            return 0;
        }
        public byte BEQ() {
            if (GetFlag(FLAGS6502.Z) == 1)
            {
                cycles++;
                addr_abs = (ushort)(pc + addr_rel);

                if ((addr_abs & 0xFF00) != (pc & 0xFF00))
                    cycles++;

                pc = addr_abs;
            }
            return 0;
        }
        public byte BIT() {
            Fetch();
            temp = (ushort)(a & fetched);
            SetFlag(FLAGS6502.Z, (temp & 0x00FF) == 0x00);
            SetFlag(FLAGS6502.N, fetched & (1 << 7));
            SetFlag(FLAGS6502.V, fetched & (1 << 6));
            return 0;
        }
        public byte BMI() {
            if (GetFlag(FLAGS6502.N) == 1)
            {
                cycles++;
                addr_abs = (ushort)(pc + addr_rel);

                if ((addr_abs & 0xFF00) != (pc & 0xFF00))
                    cycles++;

                pc = addr_abs;
            }
            return 0;
        }
        public byte BNE() {
            if (GetFlag(FLAGS6502.Z) == 0)
            {
                cycles++;
                addr_abs = (ushort)(pc + addr_rel);

                if ((addr_abs & 0xFF00) != (pc & 0xFF00))
                    cycles++;

                pc = addr_abs;
            }
            return 0;
        }
        public byte BPL() {
            if (GetFlag(FLAGS6502.N) == 0)
            {
                cycles++;
                addr_abs = (ushort)(pc + addr_rel);

                if ((addr_abs & 0xFF00) != (pc & 0xFF00))
                    cycles++;

                pc = addr_abs;
            }
            return 0;
        }
        public byte BRK() {
            pc++;

            SetFlag(FLAGS6502.I, 1);
            Write((ushort)(0x0100 + stkp), (byte)((pc >> 8) & 0x00FF));
            stkp--;
            Write((ushort)(0x0100 + stkp), (byte)(pc & 0x00FF));
            stkp--;

            SetFlag(FLAGS6502.B, 1);
            Write((ushort)(0x0100 + stkp), status);
            stkp--;
            SetFlag(FLAGS6502.B, 0);

            pc = (ushort)((ushort)Read(0xFFFE) | ((ushort)(Read(0xFFFF) << 8)));
            return 0;
        }
        public byte BVC() {
            if (GetFlag(FLAGS6502.V) == 0)
            {
                cycles++;
                addr_abs = (ushort)(pc + addr_rel);

                if ((addr_abs & 0xFF00) != (pc & 0xFF00))
                    cycles++;

                pc = addr_abs;
            }
            return 0;
        }
        public byte BVS() {
            if (GetFlag(FLAGS6502.V) == 1)
            {
                cycles++;
                addr_abs = (ushort)(pc + addr_rel);

                if ((addr_abs & 0xFF00) != (pc & 0xFF00))
                    cycles++;

                pc = addr_abs;
            }
            return 0;
        }
        public byte CLC() {
            SetFlag(FLAGS6502.C, false);
            return 0;
        }
        public byte CLD()
        {
            SetFlag(FLAGS6502.D, false);
            return 0;
        }
        public byte CLI()
        {
            SetFlag(FLAGS6502.I, false);
            return 0;
        }
        public byte CLV()
        {
            SetFlag(FLAGS6502.V, false);
            return 0;
        }
        public byte CMP() {
            Fetch();
            temp = (ushort)((ushort)a - (ushort)fetched);
            SetFlag(FLAGS6502.C, a >= fetched);
            SetFlag(FLAGS6502.Z, (temp & 0x00FF) == 0x0000);
            SetFlag(FLAGS6502.N, temp & 0x0080);
            return 1;
        }
        public byte CPX() {
            Fetch();
            temp = (ushort)((ushort)x - (ushort)fetched);
            SetFlag(FLAGS6502.C, x >= fetched);
            SetFlag(FLAGS6502.Z, (temp & 0x00FF) == 0x0000);
            SetFlag(FLAGS6502.N, temp & 0x0080);
            return 0;
        }
        public byte CPY() {
            Fetch();
            temp = (ushort)((ushort)y - (ushort)fetched);
            SetFlag(FLAGS6502.C, y >= fetched);
            SetFlag(FLAGS6502.Z, (temp & 0x00FF) == 0x0000);
            SetFlag(FLAGS6502.N, temp & 0x0080);
            return 0;
        }
        public byte DEC() {
            Fetch();
            temp = (ushort)(fetched - 1);
            Write(addr_abs, (byte)(temp & 0x00FF));
            SetFlag(FLAGS6502.Z, (temp & 0x00FF) == 0x0000);
            SetFlag(FLAGS6502.N, temp & 0x0080);
            return 0;
        }
        public byte DEX() {
            x--;
            SetFlag(FLAGS6502.Z, x == 0x00);
            SetFlag(FLAGS6502.N, x & 0x80);
            return 0;
        }
        public byte DEY()
        {
            y--;
            SetFlag(FLAGS6502.Z, y == 0x00);
            SetFlag(FLAGS6502.N, y & 0x80);
            return 0;
        }
        public byte EOR() {
            Fetch();
            a = (byte)(a ^ fetched);
            SetFlag(FLAGS6502.Z, a == 0x00);
            SetFlag(FLAGS6502.N, a & 0x80);
            return 1;
        }
        public byte INC() {
            Fetch();
            temp = (ushort)(fetched + 1);
            Write(addr_abs, (byte)(temp & 0x00FF));
            SetFlag(FLAGS6502.Z, (temp & 0x00FF) == 0x0000);
            SetFlag(FLAGS6502.N, temp & 0x0080);
            return 0;
        }
        public byte INX()
        {
            x++;
            SetFlag(FLAGS6502.Z, x == 0x00);
            SetFlag(FLAGS6502.N, x & 0x80);
            return 0;
        }
        public byte INY() {
            y++;
            SetFlag(FLAGS6502.Z, y == 0x00);
            SetFlag(FLAGS6502.N, y & 0x80);
            return 0;
        }
        public byte JMP() {
            pc = addr_abs;
            return 0;
        }
        public byte JSR() {
            pc--;

            Write((ushort)(0x0100 + stkp), (byte)((pc >> 8) & 0x00FF));
            stkp--;
            Write((ushort)(0x0100 + stkp), (byte)(pc & 0x00FF));
            stkp--;

            pc = addr_abs;
            return 0;
        }
        public byte LDA() {
            Fetch();
            a = fetched;
            SetFlag(FLAGS6502.Z, a == 0x00);
            SetFlag(FLAGS6502.N, a & 0x80);
            return 1;
        }
        public byte LDX() {
            Fetch();
            x = fetched;
            SetFlag(FLAGS6502.Z, x == 0x00);
            SetFlag(FLAGS6502.N, x & 0x80);
            return 1;
        }
        public byte LDY() {
            Fetch();
            y = fetched;
            SetFlag(FLAGS6502.Z, y == 0x00);
            SetFlag(FLAGS6502.N, y & 0x80);
            return 1;
        }
        public byte LSR() {
            Fetch();
            SetFlag(FLAGS6502.C, fetched & 0x0001);
            temp = (ushort)(fetched >> 1);
            SetFlag(FLAGS6502.Z, (temp & 0x00FF) == 0x0000);
            SetFlag(FLAGS6502.N, temp & 0x0080);

            AddrMode addr = new AddrMode(IMP);

            if (instructions[opcode].addrMode.Equals(addr))
                a = (byte)(temp & 0x00FF);
            else
                Write(addr_abs, (byte)(temp & 0x00FF));
            return 0;
        }
        public byte NOP() {
            switch (opcode)
            {
                case 0x1C:
                case 0x3C:
                case 0x5C:
                case 0x7C:
                case 0xDC:
                case 0xFC:
                    return 1;
            }
            return 0;
        }
        public byte ORA()
        {
            Fetch();
            a = (byte)(a | fetched);
            SetFlag(FLAGS6502.Z, a == 0x00);
            SetFlag(FLAGS6502.N, a & 0x80);
            return 1;
        }
        public byte PHA() {
            Write((ushort)(0x0100 + stkp), a);
            stkp--;
            return 0;
        }
        public byte PHP() {
            Write((ushort)(0x0100 + stkp), (byte)(status | (byte)FLAGS6502.B | (byte)FLAGS6502.U));
            SetFlag(FLAGS6502.B, 0);
            SetFlag(FLAGS6502.U, 0);
            stkp--;
            return 0;
        }
        public byte PLA() {
            stkp++;
            a = Read((ushort)(0x0100 + stkp));
            SetFlag(FLAGS6502.Z, a == 0x00);
            SetFlag(FLAGS6502.N, a & 0x80);
            return 0;
        }
        public byte PLP() {
            stkp++;
            status = Read((ushort)(0x0100 + stkp));
            SetFlag(FLAGS6502.U, 1);
            return 0;
        }
        public byte ROL() {
            Fetch();
            
            temp = (ushort)((byte)(fetched << 1) | (GetFlag(FLAGS6502.C)));
            SetFlag(FLAGS6502.C, temp & 0xFF00);
            SetFlag(FLAGS6502.Z, (temp & 0x00FF) == 0x0000);
            SetFlag(FLAGS6502.N, temp & 0x0080);

            AddrMode addr = new AddrMode(IMP);

            if (instructions[opcode].addrMode.Equals(addr))
                a = (byte)(temp & 0x00FF);
            else
                Write(addr_abs, (byte)(temp & 0x00FF));
            return 0;
        }
        public byte ROR() {
            Fetch();

            temp = (ushort)((byte)((GetFlag(FLAGS6502.C) << 7) | (byte)(fetched >> 1)));
            SetFlag(FLAGS6502.C, temp & 0x01);
            SetFlag(FLAGS6502.Z, (temp & 0x00FF) == 0x00);
            SetFlag(FLAGS6502.N, temp & 0x0080);

            AddrMode addr = new AddrMode(IMP);

            if (instructions[opcode].addrMode.Equals(addr))
                a = (byte)(temp & 0x00FF);
            else
                Write(addr_abs, (byte)(temp & 0x00FF));
            return 0;
        }
        public byte RTI() {
            stkp++;
            status = Read((byte)(0x0100 + stkp));
            status &= (byte)0x0001;
            status &= (byte)0x00001;

            stkp++;
            pc = (ushort)Read((ushort)(0x0100 + stkp));
            stkp++;
            pc |= (ushort)(Read((ushort)(0x0100 + stkp)) << 8);
            return 0;
        }
        public byte RTS() {
            stkp++;
            pc = (ushort)Read((ushort)(0x0100 + stkp));
            stkp++;
            pc |= (ushort)(Read((ushort)(0x0100 + stkp)) << 8);

            pc++;
            return 0;
        }
        public byte SBC() {
            Fetch();
            ushort value = (ushort)((ushort)fetched ^ 0x00FF);

            temp = (ushort)(a + value + GetFlag(FLAGS6502.C));
            SetFlag(FLAGS6502.C, temp > 255);
            SetFlag(FLAGS6502.Z, (temp & 0x00FF) == 0x00);
            SetFlag(FLAGS6502.V, (temp ^ (ushort)a) & (temp ^ value) & 0x0080);
            SetFlag(FLAGS6502.N, temp & 0x80);
            a = (byte)(temp & 0x00FF);
            return 1;
        }


        public byte SEC() {
            SetFlag(FLAGS6502.C, true);
            return 0;
        }
        public byte SED()
        {
            SetFlag(FLAGS6502.D, true);
            return 0;
        }
        public byte SEI()
        {
            SetFlag(FLAGS6502.I, true);
            return 0;
        }
        public byte STA() {
            Write(addr_abs, a);
            return 0;
        }
        public byte STX() {
            Write(addr_abs, x);
            return 0;
        }
        public byte STY() {
            Write(addr_abs, y);
            return 0;
        }
        public byte TAX() {
            x = a;
            SetFlag(FLAGS6502.Z, x == 0x00);
            SetFlag(FLAGS6502.N, x & 0x80);
            return 0;
        }
        public byte TAY() {
            y = a;
            SetFlag(FLAGS6502.Z, y == 0x00);
            SetFlag(FLAGS6502.N, y & 0x80);
            return 0;
        }
        public byte TSX() {
            x = stkp;
            SetFlag(FLAGS6502.Z, x == 0x00);
            SetFlag(FLAGS6502.N, x & 0x80);
            return 0;
        }
        public byte TXA()
        {
            a = x;
            SetFlag(FLAGS6502.Z, a == 0x00);
            SetFlag(FLAGS6502.N, a & 0x80);
            return 0;
        }
        public byte TXS() {
            stkp = x;
            return 0;
        }
        public byte TYA() {
            a = y;
            SetFlag(FLAGS6502.Z, a == 0x00);
            SetFlag(FLAGS6502.N, a & 0x80);
            return 0;
        }

        public byte XXX() { return 0x00; }
        #endregion

        bool test = false;

        public void Clock()
        {

            if (!test) pc = 0xC000;
            test = true;

            if(cycles == 0)
            {
                opcode = Read(pc);
                pc++;

                cycles = instructions[opcode].cycles;

                byte ac_1 = instructions[opcode].addrMode();
                byte ac_2 = instructions[opcode].operate();

                string instruction = instructions[opcode].name;

                cycles += (byte)(ac_1 & ac_2);

                SetFlag(FLAGS6502.U, true);

            }

            clock_count++;
            cycles--;
        }
        public void Reset()
        {

            addr_abs = 0xFFFC;
            ushort lo = Read((ushort)(addr_abs + 0));
            ushort hi = Read((ushort)(addr_abs + 1));

            pc = (ushort)((ushort)(hi << 8) | lo);

            a = 0;
            x = 0;
            y = 0;

            stkp = 0xFD;
            status = 0x00 | (byte)FLAGS6502.U;

            addr_abs = 0;
            addr_rel = 0;
            fetched = 0x00;

            cycles = 8;
        }
        public void irq()
        {
            if(GetFlag(FLAGS6502.I) == 0)
            {
                Write((ushort)(0x0100 + stkp), (byte)((pc >> 8) & 0x00FF));
                stkp--;
                Write((ushort)(0x0100 + stkp), (byte)(pc & 0x00FF));
                stkp--;

                SetFlag(FLAGS6502.B, false);
                SetFlag(FLAGS6502.U, true);
                SetFlag(FLAGS6502.I, true);

                Write((ushort)(0x0100 + stkp), (byte)(status));

                stkp--;

                addr_abs = 0xFFFE;
                ushort lo = Read((ushort)(addr_abs + 0));
                ushort hi = Read((ushort)(addr_abs + 1));
                pc = (ushort)((hi << 8) | lo);


                cycles = 7;
            }
        }
        public void nmi()
        {
            Write((ushort)(0x0100 + stkp), (byte)((pc >> 8) & 0x00FF));
            stkp--;
            Write((ushort)(0x0100 + stkp), (byte)(pc & 0x00FF));
            stkp--;

            SetFlag(FLAGS6502.B, false);
            SetFlag(FLAGS6502.U, true);
            SetFlag(FLAGS6502.I, true);

            Write((ushort)(0x0100 + stkp), (byte)(status));

            stkp--;

            addr_abs = 0xFFFA;
            ushort lo = Read((ushort)(addr_abs + 0));
            ushort hi = Read((ushort)(addr_abs + 1));
            pc = (ushort)((hi << 8) | lo);


            cycles = 8;
        }
        public byte Fetch()
        {
            AddrMode tmpAddr = new AddrMode(IMP);

            if (!instructions[opcode].addrMode.Equals(tmpAddr))
                fetched = Read(addr_abs);

            return fetched;
        }
        public bool Complete()
        {
            return cycles == 0;
        }
    
        public Dictionary<ushort, string> Disassemble(ushort nStart, ushort nStop)
        {

            Console.WriteLine("START");
            var mapLines = new Dictionary<ushort, string>();
            ushort addr = nStart;
            ushort line_addr = 0;
            byte value = 0x00, lo = 0x00, hi = 0x00;


            AddrMode _IMP = new AddrMode(IMP);
            AddrMode _IMM = new AddrMode(IMM);
            AddrMode _ZP0 = new AddrMode(ZP0);
            AddrMode _ZPX = new AddrMode(ZPX);
            AddrMode _ZPY = new AddrMode(ZPY);
            AddrMode _IZX = new AddrMode(IZX);
            AddrMode _IZY = new AddrMode(IZY);
            AddrMode _ABS = new AddrMode(ABS);
            AddrMode _ABX = new AddrMode(ABX);
            AddrMode _ABY = new AddrMode(ABY);
            AddrMode _IND = new AddrMode(IND);
            AddrMode _REL = new AddrMode(REL);
            

            while (addr < nStop-1)
            {
                line_addr = addr;
                string sInst = "$" + Hex(addr, 4) + ": ";

                byte opcode = bus.cpuRead(addr, true);
                addr++;

                sInst += instructions[opcode].name + " ";

                if (instructions[opcode].addrMode.Equals(_IMP))
                {
                    sInst += " {IMP}";
                }
                else if (instructions[opcode].addrMode.Equals(_IMM))
                {
                    value = bus.cpuRead(addr, true); addr++;
                    sInst += "#$" + Hex(value, 2) + " {IMM}";
                }
                else if (instructions[opcode].addrMode.Equals(_ZP0))
                {
                    lo = bus.cpuRead(addr, true); addr++;
                    hi = 0x00;
                    sInst += "$" + Hex(lo, 2) + " {ZP0}";
                }
                else if (instructions[opcode].addrMode.Equals(_ZPX))
                {
                    lo = bus.cpuRead(addr, true); addr++;
                    hi = 0x00;
                    sInst += "$" + Hex(lo, 2) + ", X {ZPX}";
                }
                else if (instructions[opcode].addrMode.Equals(_ZPY))
                {
                    lo = bus.cpuRead(addr, true); addr++;
                    hi = 0x00;
                    sInst += "$" + Hex(lo, 2) + ", Y {ZPY}";

                }
                else if (instructions[opcode].addrMode.Equals(_IZX))
                {
                    lo = bus.cpuRead(addr, true); addr++;
                    hi = 0x00;
                    sInst += "($" + Hex(lo, 2) + ", X) {IZX}";
                }
                else if (instructions[opcode].addrMode.Equals(_IZY))
                {
                    lo = bus.cpuRead(addr, true); addr++;
                    hi = 0x00;
                    sInst += "($" + Hex(lo, 2) + "), Y {IZY}";
                }
                else if (instructions[opcode].addrMode.Equals(_ABS))
                {
                    lo = bus.cpuRead(addr, true); addr++;
                    hi = bus.cpuRead(addr, true); addr++;
                    sInst += "$" + Hex((ushort)((hi << 8) | lo), 4) + " {ABS}";
                }
                else if (instructions[opcode].addrMode.Equals(_ABX))
                {
                    lo = bus.cpuRead(addr, true); addr++;
                    hi = bus.cpuRead(addr, true); addr++;
                    sInst += "$" + Hex((ushort)((hi << 8) | lo), 4) + ", X {ABX}";
                }
                else if (instructions[opcode].addrMode.Equals(_ABY))
                {
                    lo = bus.cpuRead(addr, true); addr++;
                    hi = bus.cpuRead(addr, true); addr++;
                    sInst += "$" + Hex((ushort)((hi << 8) | lo), 4) + ", Y {ABY}";
                }
                else if (instructions[opcode].addrMode.Equals(_IND))
                {
                    lo = bus.cpuRead(addr, true); addr++;
                    hi = bus.cpuRead(addr, true); addr++;
                    sInst += "($" + Hex((ushort)((hi << 8) | lo), 4) + ") {IND}";

                }
                else if (instructions[opcode].addrMode.Equals(_REL))
                {
                    value = bus.cpuRead(addr, true); addr++;
                    sInst += "$" + Hex(value, 2) + " [$" + Hex((ushort)(addr + value), 4) + "] {REL}";
                }

               
                mapLines[line_addr] = sInst;

                
            }


          
            return mapLines;

        }

        private string Hex(ushort d, int size)
        {
            string format = "X" + size;
            return d.ToString(format);
        }

    }

    public enum FLAGS6502
    {
        C = (byte)(1 << 0),
        Z = (byte)(1 << 1),
        I = (byte)(1 << 2),
        D = (byte)(1 << 3),
        B = (byte)(1 << 4),
        U = (byte)(1 << 5),
        V = (byte)(1 << 6),
        N = (byte)(1 << 7)
    }

    class Instruction
    {
        
        public string name { get; set; }
        public byte cycles { get; set; }
        public Operate operate;
        public AddrMode addrMode;
        

        public Instruction(string name, Operate operate, AddrMode addrMode, byte cycles)
        {
            this.name = name;
            this.cycles = cycles;
            this.operate = operate;
            this.addrMode = addrMode;
        }
    }
}
