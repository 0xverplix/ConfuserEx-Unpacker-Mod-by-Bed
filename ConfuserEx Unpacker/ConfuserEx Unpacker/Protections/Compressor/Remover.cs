using CawkEmulatorV4;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using System.Runtime.InteropServices;
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
           
                StepOne(ModuleDef);
            
        }
        public static bool IsPacked(ModuleDefMD module)
        {

            // Thanks to 0xd4d https://github.com/0xd4d/dnlib/issues/72
            for (uint rid = 1; rid <= module.Metadata.TablesStream.FileTable.Rows; rid++)
            {
                dnlib.DotNet.MD.RawFileRow row = new dnlib.DotNet.MD.RawFileRow();
                module.TablesStream.TryReadFileRow(rid,out row);
                string name = module.StringsStream.ReadNoNull(row.Name);
                //if (name != "koi") continue;


                return true;
            }

            return false;
        }
        private void StepOne(ModuleDefMD module)
        {
            var ep = module.EntryPoint;
          
                foreach (TypeDef typeDef in module.GetTypes())
                {
                    foreach (MethodDef methods in typeDef.Methods)
                    {
                        if (!methods.HasBody) continue;
                        for (int i = 0; i < methods.Body.Instructions.Count; i++)
                        {
                            if (methods.Body.Instructions[i].OpCode == OpCodes.Call)
                            {
                           
                                
                                    if (methods.Body.Instructions[i].Operand.ToString().Contains("Assembly::Load"))
                                    {
                              
                                byte[] bytees = (byte[])methods.Body.Instructions[i-2].Operand;
                               Protections.Base.ModuleDef = ModuleDefMD.Load(bytees);
                                  //Protections.Base.ModuleDef.EntryPoint = Protections.Base.ModuleDef.ResolveToken(ModuleEp) as MethodDef;
                                Base.CompressorRemoved = true;


                            }

                        }
                        }
                    }
                }
            
        //Protections.Base.ModuleDef = ModuleDefMD.Load(ModuleBytes);
        //    Protections.Base.ModuleDef.EntryPoint = Protections.Base.ModuleDef.ResolveToken(ModuleEp) as MethodDef;
        //    Base.CompressorRemoved = true;
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
