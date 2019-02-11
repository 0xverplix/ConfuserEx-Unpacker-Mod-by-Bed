using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace ConfuserEx_Unpacker.Protections.Mutations.DateTime
{
	class Remover:Base
	{
		public override void Deobfuscate()
		{
			Console.WriteLine("[!] Cleaning DateTime Mutations");
			DateTime(ModuleDef);
		
		}

		public static void DateTime(ModuleDefMD modulee)
		{
			foreach (var types in modulee.GetTypes())
			foreach (var methods in types.Methods)
			{
				if (!methods.HasBody) continue;

				for (var i = 0; i < methods.Body.Instructions.Count; i++)
					if (methods.Body.Instructions[i].OpCode == OpCodes.Call && methods.Body.Instructions[i]
						    .Operand.ToString().Contains("get_TotalDays"))
						if ((methods.Body.Instructions[i - 1].OpCode == OpCodes.Ldloca_S || methods.Body.Instructions[i - 1].IsLdloc() ||
						     methods.Body.Instructions[i - 1].OpCode == OpCodes.Ldloca) &&
						    methods.Body.Instructions[i - 2].IsStloc() &&
						    methods.Body.Instructions[i - 3].OpCode == OpCodes.Call && methods.Body
							    .Instructions[i - 3].Operand.ToString().Contains("op_Subtraction"))

							if (methods.Body.Instructions[i - 5].IsLdcI4() &&
							    methods.Body.Instructions[i - 6].IsLdcI4() &&
							    methods.Body.Instructions[i - 7].IsLdcI4() &&
							    methods.Body.Instructions[i - 9].IsLdcI4() &&
							    methods.Body.Instructions[i - 10].IsLdcI4() &&
							    methods.Body.Instructions[i - 11].IsLdcI4())
								try
								{
									var int1 = methods.Body.Instructions[i - 11].GetLdcI4Value();
									var int2 = methods.Body.Instructions[i - 10].GetLdcI4Value();
									var int3 = methods.Body.Instructions[i - 9].GetLdcI4Value();
									var date1 = new System.DateTime(int1, int2, int3);

									var int4 = methods.Body.Instructions[i - 7].GetLdcI4Value();
									var int5 = methods.Body.Instructions[i - 6].GetLdcI4Value();
									var int6 = methods.Body.Instructions[i - 5].GetLdcI4Value();
									var date2 = new System.DateTime(int4, int5, int6);
									var result = (date1 - date2).TotalDays;
									
									for (var y = 0; y < 12; y++)
										methods.Body.Instructions[i - y].OpCode = OpCodes.Nop;
									methods.Body.Instructions[i - 4].OpCode = OpCodes.Ldc_I4;
									methods.Body.Instructions[i - 4].Operand = (int)result;
								}
								catch
								{
								}
			}
		}
	}
}
