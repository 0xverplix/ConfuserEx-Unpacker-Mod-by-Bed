using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace ConfuserEx_Unpacker.Protections.Mutations.EmptyTypes
{
	class Remover:Base
	{
		public override void Deobfuscate()
		{
			Console.WriteLine("[!] Cleaning Empty Types");
			EmptyTypesCleaner(ModuleDef);
		
		}

		public static void EmptyTypesCleaner(ModuleDefMD module)
		{
			foreach (var type in module.GetTypes())
			foreach (var methods in type.Methods)
			{
				if (!methods.HasBody) continue;
				for (var i = 0; i < methods.Body.Instructions.Count; i++)
					if (methods.Body.Instructions[i].OpCode == OpCodes.Ldsfld && methods.Body.Instructions[i]
						    .Operand.ToString().Contains("::EmptyTypes") && methods.Body.Instructions[i + 1].OpCode == OpCodes.Ldlen)
					{
						methods.Body.Instructions[i].OpCode = OpCodes.Ldc_I4_0;
						methods.Body.Instructions[i + 1].OpCode = OpCodes.Nop;
					}
			}
		}
	}
}
