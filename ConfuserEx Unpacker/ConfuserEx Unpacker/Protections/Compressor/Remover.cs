using CawkEmulatorV4;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConfuserEx_Unpacker.Protections.Compressor
{
    internal class Remover : Base
    {
        
        public static int ModuleEp;

        public static byte[] ModuleBytes { get; set; }

        public override void Deobfuscate()
        {
           
                UnpackTrinity(ModuleDef);
            if (IsPacked(ModuleDef))
            {
                UnpackKoi(ModuleDef);
            }
            else
            {
                Checkkoi("'Koi' was not found, would you like to force compressor protecion anyways?");
            }

        }
       
        private void UnpackNoKoi(ModuleDefMD module)
        {
            var ep = module.EntryPoint;
            var emulator = new Emulation(ep);
            emulator.OnCallPrepared += (emulation, args) =>
            {
                var instr = args.Instruction;
                Utils.HandleCall(args, emulation);
            };
            emulator.OnInstructionPrepared += (emulation, args) =>
            {
                if (args.Instruction.OpCode == OpCodes.Ldftn)
                {
                    emulation.ValueStack.CallStack.Push(null);
                    args.Cancel = true;
                }
            };
            emulator.Emulate();
            if (ModuleBytes == null)
            {
                Base.CompressorRemoved = false;
                return;
            }

            Protections.Base.ModuleDef = ModuleDefMD.Load(ModuleBytes);
            Protections.Base.ModuleDef.EntryPoint = Protections.Base.ModuleDef.ResolveToken(ModuleEp) as MethodDef;
            Base.CompressorRemoved = true;
        }
        public static bool IsPacked(ModuleDefMD module)
        {

            // Thanks to 0xd4d https://github.com/0xd4d/dnlib/issues/72
            for (uint rid = 1; rid <= module.Metadata.TablesStream.FileTable.Rows; rid++)
            {
                dnlib.DotNet.MD.RawFileRow row = new dnlib.DotNet.MD.RawFileRow();
                module.TablesStream.TryReadFileRow(rid, out row);
                string name = module.StringsStream.ReadNoNull(row.Name);
                if (name != "koi") continue;


                return true;
            }
            
            return false;
        }
        private void UnpackKoi(ModuleDefMD module)
        {
            var ep = module.EntryPoint;
            var emulator = new Emulation(ep);
            emulator.OnCallPrepared += (emulation, args) =>
            {
                var instr = args.Instruction;
                Utils.HandleCall(args, emulation);
            };
            emulator.OnInstructionPrepared += (emulation, args) =>
            {
                if (args.Instruction.OpCode == OpCodes.Ldftn)
                {
                    emulation.ValueStack.CallStack.Push(null);
                    args.Cancel = true;
                }
            };
            emulator.Emulate();
            if (ModuleBytes == null)
            {
                Base.CompressorRemoved = false;
                return;
            }

            Protections.Base.ModuleDef = ModuleDefMD.Load(ModuleBytes);
            Protections.Base.ModuleDef.EntryPoint = Protections.Base.ModuleDef.ResolveToken(ModuleEp) as MethodDef;
            Base.CompressorRemoved = true;
        }
        private void UnpackTrinity(ModuleDefMD module)
        {
            bool can = false;
            
                if (module.EntryPoint.Body.Instructions[44].Operand.ToString().Contains("Assembly::Load"))
                {
                    can = true;
                    
                }
                
            
            if(can)
            try
            {
                

                string ivKey = "";
                MethodDef epp = module.EntryPoint;
                for (int i = 0; i < module.EntryPoint.Body.Instructions.Count; i++)
                {
                    if (module.EntryPoint.Body.Instructions[i].OpCode == OpCodes.Ldstr)
                    {
                        if (module.EntryPoint.Body.Instructions[i].Operand.ToString().Length == 24)
                        {
                            ivKey = module.EntryPoint.Body.Instructions[i].Operand.ToString();
                        }
                    }
                }
                string key = "";
                for (int i = 0; i < module.EntryPoint.Body.Instructions.Count; i++)
                {
                    if (module.EntryPoint.Body.Instructions[i].OpCode == OpCodes.Ldstr)
                    {
                        if (module.EntryPoint.Body.Instructions[i].Operand.ToString().Length == 44)
                        {
                            key = module.EntryPoint.Body.Instructions[i].Operand.ToString();
                        }
                    }
                }
                string arrayKey = "";
                    for (int i = 0; i < module.EntryPoint.Body.Instructions.Count; i++)
                    {
                        if (module.EntryPoint.Body.Instructions[i].OpCode == OpCodes.Ldstr)
                        {
                            if (module.EntryPoint.Body.Instructions[i].Operand.ToString().Length > 200)
                            {
                                arrayKey = module.EntryPoint.Body.Instructions[i].Operand.ToString();
                            }
                        }
                    }
                RijndaelManaged rijndaelManaged = new RijndaelManaged();
                rijndaelManaged.KeySize = 256;
                rijndaelManaged.Key = Convert.FromBase64String(key);
                rijndaelManaged.IV = Convert.FromBase64String(ivKey);
                rijndaelManaged.Padding = PaddingMode.ISO10126;
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndaelManaged.CreateDecryptor(), CryptoStreamMode.Write);
                byte[] array = Convert.FromBase64String(arrayKey);
                cryptoStream.Write(array, 0, array.Length);
                cryptoStream.Flush();
                memoryStream.Seek(0L, SeekOrigin.Begin);
                Protections.Base.ModuleDef = ModuleDefMD.Load(memoryStream.ToArray());
                cryptoStream.Close();
                memoryStream.Close();
                cryptoStream.Dispose();
                memoryStream.Dispose();
                Base.CompressorRemoved = true;    }
            catch(Exception ex)
            {
                Console.Write(ex.ToString());
                    Console.Read();
            }
                
            
        }
        public string Checkkoi(string Question)
        {

            Console.WriteLine(Question);
            Console.Read();
            while (true)
            {
               
                ConsoleKeyInfo KeyInfoPressed = Console.ReadKey();
                switch (KeyInfoPressed.Key)
                {
                    case ConsoleKey.Y:
                        UnpackNoKoi(ModuleDef);

                        break;
                    case ConsoleKey.N:

                        break;

                  
                }
            }
        }
    
        public static GCHandle HandleDecryptMethod(Emulation mainEmulation, CallEventArgs mainArgs, MethodDef decryptionMethod)
        {
            var decEmulation = new Emulation(decryptionMethod);
            decEmulation.OnCallPrepared += (emulation, args) => { Utils.HandleCall(args, emulation); };
            decEmulation.ValueStack.Parameters[decryptionMethod.Parameters[1]] = (uint)
                mainEmulation.ValueStack.CallStack.Pop();
            decEmulation.ValueStack.Parameters[decryptionMethod.Parameters[0]] =
                mainEmulation.ValueStack.CallStack.Pop();
            decEmulation.Emulate();
            return (GCHandle)decEmulation.ValueStack.CallStack.Pop();
        }


    }
}
