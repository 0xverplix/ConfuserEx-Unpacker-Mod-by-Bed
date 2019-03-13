using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfuserEx_Unpacker.Protections.Control_Flow
{
    internal class StackEmulator
    {
        // Token: 0x0600215F RID: 8543 RVA: 0x0009318B File Offset: 0x0009218B
        public static void InitStack()
        {
            StackEmulator.pointer = -1;
            StackEmulator.stack = new List<int>();
        }

        // Token: 0x06002160 RID: 8544 RVA: 0x000931A0 File Offset: 0x000921A0
        public static bool IsStackEmpty()
        {
            return StackEmulator.pointer == -1 || StackEmulator.stack.Count == 0;
        }

        // Token: 0x06002161 RID: 8545 RVA: 0x000931D6 File Offset: 0x000921D6
        public static void PushValue(int value1)
        {
            StackEmulator.stack.Add(value1);
            StackEmulator.pointer++;
        }

        // Token: 0x06002162 RID: 8546 RVA: 0x000931F4 File Offset: 0x000921F4
        public static void Dup()
        {
            if (!StackEmulator.IsStackEmpty())
            {
                int value = StackEmulator.stack[StackEmulator.pointer];
                StackEmulator.PushValue(value);
            }
        }

        // Token: 0x06002163 RID: 8547 RVA: 0x00093228 File Offset: 0x00092228
        public static int PopValue()
        {
            int result;
            if (StackEmulator.IsStackEmpty())
            {
                result = -1;
            }
            else
            {
                int num = StackEmulator.stack[StackEmulator.pointer];
                StackEmulator.stack.RemoveAt(StackEmulator.pointer);
                StackEmulator.pointer--;
                result = num;
            }
            return result;
        }

        // Token: 0x06002164 RID: 8548 RVA: 0x00093278 File Offset: 0x00092278
        public static void Main2(string[] args)
        {
            StackEmulator.InitStack();
            StackEmulator.PushValue(1);
            StackEmulator.Dup();
            int num = StackEmulator.PopValue();
            StackEmulator.PushValue(2);
            int num2 = StackEmulator.PopValue();
            int num3 = StackEmulator.PopValue();
            Console.WriteLine("Hello World!");
            Console.Write("Press any key to continue . . . ");
            Console.ReadKey(true);
        }

        // Token: 0x04000E93 RID: 3731
        public static List<int> stack;

        // Token: 0x04000E94 RID: 3732
        public static int pointer;
    }
}

