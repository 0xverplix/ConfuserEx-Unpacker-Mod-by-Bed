using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace ConfuserEx_Unpacker.Protections.Mutations.StringLength
{
	class Remover:Base
	{
		public override void Deobfuscate()
		{
			Console.WriteLine("[!] Fixing String.Length");
			stringLengthFixer(ModuleDef);
			
		}

		public static void stringLengthFixer(ModuleDefMD module)
		{
			foreach (TypeDef typeDef in module.GetTypes())
			{
				foreach (MethodDef methods in typeDef.Methods)
				{
					if (!methods.HasBody) continue;
					for (int i = 0; i < methods.Body.Instructions.Count; i++)
					{
						if (methods.Body.Instructions[i].OpCode == OpCodes.Call &&
						    methods.Body.Instructions[i].Operand.ToString().Contains("get_Length") &&
						    methods.Body.Instructions[i - 1].OpCode == OpCodes.Ldstr)
						{
							string val = methods.Body.Instructions[i - 1].Operand.ToString();
							int len = val.Length;
							
							methods.Body.Instructions[i].OpCode = OpCodes.Ldc_I4;
							methods.Body.Instructions[i].Operand = len;
							methods.Body.Instructions[i - 1].OpCode = OpCodes.Nop;
						}
					}
				}
			}
		}
	}
}
