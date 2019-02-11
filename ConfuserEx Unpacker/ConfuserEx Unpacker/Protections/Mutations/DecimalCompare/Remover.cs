using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace ConfuserEx_Unpacker.Protections.Mutations.DecimalCompare
{
	class Remover:Base
	{
		public override void Deobfuscate()
		{
			
			Console.WriteLine("[!] Cleaning Decimal.Compare");
			DecimalCompare(ModuleDef);
			
		}

		public static void DecimalCompare(ModuleDefMD module)
		{
			foreach (var types in module.GetTypes())
			foreach (var methods in types.Methods)
			{
				if (!methods.HasBody) continue;
				for (var i = 0; i < methods.Body.Instructions.Count; i++)
					if (methods.Body.Instructions[i].OpCode == OpCodes.Call)
						if (methods.Body.Instructions[i].OpCode == OpCodes.Call &&
						    methods.Body.Instructions[i].Operand.ToString().Contains("Compare"))
							if (methods.Body.Instructions[i - 1].OpCode == OpCodes.Newobj)
								if (methods.Body.Instructions[i - 2].IsLdcI4() && methods.Body.Instructions[i - 4].IsLdcI4())
								{
									var val1 = methods.Body.Instructions[i - 4].GetLdcI4Value();
									var val2 = methods.Body.Instructions[i - 2].GetLdcI4Value();
									var newValue = decimal.Compare(val1, val2);
								
									methods.Body.Instructions[i].OpCode = OpCodes.Nop;
									methods.Body.Instructions[i - 1].OpCode = OpCodes.Nop;
									methods.Body.Instructions[i - 2].OpCode = OpCodes.Nop;
									methods.Body.Instructions[i - 3].OpCode = OpCodes.Nop;
									methods.Body.Instructions[i - 4].OpCode = OpCodes.Ldc_I4;
									methods.Body.Instructions[i - 4].Operand = newValue;
								}
			}
		}
	}
}
