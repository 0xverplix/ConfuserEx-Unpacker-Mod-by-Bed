using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace ConfuserEx_Unpacker.Protections.Mutations.AndFixer
{
	class Remover:Base
	{
		public override void Deobfuscate()
		{
			Console.WriteLine("[!] Fixing And's");
			andFixer2(ModuleDef);
			
		}

		public static void andFixer2(ModuleDefMD module)
		{
			foreach (var types in module.GetTypes())
			foreach (var methods in types.Methods)
			{

				if (!methods.HasBody) continue;
				for (var i = 0; i < methods.Body.Instructions.Count; i++)
					if (methods.Body.Instructions[i].IsLdloc())
						if (methods.Body.Instructions[i + 1].IsLdloc())
							if (methods.Body.Instructions[i + 2].OpCode == OpCodes.Neg &&
							    methods.Body.Instructions[i + 3].OpCode == OpCodes.And)
							{
								methods.Body.Instructions[i].OpCode = OpCodes.Ldc_I4;
								methods.Body.Instructions[i + 1].OpCode = OpCodes.Ldc_I4;
								methods.Body.Instructions[i].Operand = 0;
								methods.Body.Instructions[i + 1].Operand = 0;
							}
			}
		}
	}
}
