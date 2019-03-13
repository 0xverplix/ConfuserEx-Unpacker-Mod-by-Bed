using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace ConfuserEx_Unpacker.Protections.Mutations.Maths
{
	class Remover:Base
	{
		public override void Deobfuscate()
		{
			//Console.WriteLine("[!] Cleaning Math.* Mutations");
			MathResolver(ModuleDef);
            SizeOf.SizeOfFixer(ModuleDef);
            MathsEquations.MathsFixer(ModuleDef);

        }

		public static void MathResolver(ModuleDefMD modulee)
		{
           
			foreach (var types in modulee.GetTypes())
			foreach (var methods in types.Methods)
			{
				if (!methods.HasBody) continue;

				for (var i = 0; i < methods.Body.Instructions.Count; i++)
					if (methods.Body.Instructions[i].OpCode == OpCodes.Call)
						if (methods.Body.Instructions[i].Operand.ToString().ToLower().Contains("math"))
							if (methods.Body.Instructions[i - 1].OpCode == OpCodes.Ldc_R8)
							{
								var abc = methods.Body.Instructions[i - 1].Operand.ToString();
								if (methods.Body.Instructions[i].Operand.ToString().ToLower().Contains("sin"))
								{
									methods.Body.Instructions[i - 1].Operand = Math.Sin(double.Parse(abc));
									methods.Body.Instructions[i].OpCode = OpCodes.Nop;
								}
								else if (methods.Body.Instructions[i].Operand.ToString().ToLower().Contains("sqrt"))
								{
									methods.Body.Instructions[i - 1].Operand = Math.Sqrt(double.Parse(abc));
									methods.Body.Instructions[i].OpCode = OpCodes.Nop;
								}

								else if (methods.Body.Instructions[i].Operand.ToString().ToLower().Contains("cos"))
								{
									methods.Body.Instructions[i - 1].Operand = Math.Cos(double.Parse(abc));
									methods.Body.Instructions[i].OpCode = OpCodes.Nop;
								}
								else if (methods.Body.Instructions[i].Operand.ToString().ToLower().Contains("floor"))
								{
									methods.Body.Instructions[i - 1].Operand = Math.Floor(double.Parse(abc));
									methods.Body.Instructions[i].OpCode = OpCodes.Nop;
								}
								else if (methods.Body.Instructions[i].Operand.ToString().ToLower().Contains("log"))
								{
									methods.Body.Instructions[i - 1].Operand =
										Math.Log(double.Parse(abc));
									methods.Body.Instructions[i].OpCode = OpCodes.Nop;
								}
								else
								{
									continue;
								}
							
							}
							else if (methods.Body.Instructions[i - 1].IsLdcI4())
							{
								var abc = methods.Body.Instructions[i - 1].GetLdcI4Value().ToString();
								if (methods.Body.Instructions[i].Operand.ToString().ToLower().Contains("abs"))
								{
									methods.Body.Instructions[i - 1].Operand = Math.Abs(int.Parse(abc));
									methods.Body.Instructions[i].OpCode = OpCodes.Nop;
								}
							}

			}
		}
	}
}
