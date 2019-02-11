using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;


namespace ConfuserEx_Unpacker.Protections.Constants
{
	// Token: 0x02000004 RID: 4
	internal class SizeOf
	{
        // Token: 0x06000005 RID: 5 RVA: 0x00002640 File Offset: 0x00000840
        public static void SizeOfFixer(ModuleDef module)
        {
            foreach (TypeDef type in (IEnumerable<TypeDef>)module.Types)
            {
                foreach (MethodDef method in (IEnumerable<MethodDef>)type.Methods)
                {
                    if (method.HasBody)
                    {
                        for (int index = 0; index < method.Body.Instructions.Count; ++index)
                        {
                            if (method.Body.Instructions[index].OpCode == OpCodes.Sizeof)
                            {
                                switch ((method.Body.Instructions[index].Operand as ITypeDefOrRef).ToString())
                                {
                                    case "System.Boolean":
                                        method.Body.Instructions[index].OpCode = OpCodes.Ldc_I4_1;
                                    
                                        break;
                                    case "System.Byte":
                                        method.Body.Instructions[index].OpCode = OpCodes.Ldc_I4_1;
                                      
                                        break;
                                    case "System.Decimal":
                                        method.Body.Instructions[index].OpCode = OpCodes.Ldc_I4;
                                        method.Body.Instructions[index].Operand = (object)16;
                                      
                                        break;
                                    case "System.Double":
                                    case "System.Int64":
                                    case "System.UInt64":
                                        method.Body.Instructions[index].OpCode = OpCodes.Ldc_I4_8;
                                        
                                        break;
                                    case "System.Guid":
                                        method.Body.Instructions[index].OpCode = OpCodes.Ldc_I4;
                                        method.Body.Instructions[index].Operand = (object)16;
                                        break;
                                    case "System.Int16":
                                    case "System.UInt16":
                                        method.Body.Instructions[index].OpCode = OpCodes.Ldc_I4_2;
                                        break;
                                    case "System.Int32":
                                    case "System.Single":
                                    case "System.UInt32":
                                        method.Body.Instructions[index].OpCode = OpCodes.Ldc_I4_4;
                                        break;
                                    case "System.SByte":
                                        method.Body.Instructions[index].OpCode = OpCodes.Ldc_I4_1;
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
