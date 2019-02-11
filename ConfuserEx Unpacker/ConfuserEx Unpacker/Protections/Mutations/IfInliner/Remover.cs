﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using de4dot.blocks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace ConfuserEx_Unpacker.Protections.Mutations.IfInliner
{
	class Remover:Base
	{
		public override void Deobfuscate()
		{
			Console.WriteLine("[!] Cleaning If Statements");
			removerRemade(ModuleDef);
		
		}

		public static bool Checker(Block block, MethodDef methods) => block.Instructions.Where((t, i) => t.IsStloc() && block.Instructions[i - 1].IsLdcI4()).Select(t => t.Instruction.GetLocal(methods.Body.Variables)).Any(local => local.Type == methods.Module.CorLibTypes.Int32);

		public static void removerRemade(ModuleDefMD module)
		{
			foreach (var type in module.GetTypes())
			foreach (var methods in type.Methods)
			{
				
				var found = true;
				var dictionary = new Dictionary<Local, int>();
				if (!methods.HasBody) continue;
				var blocks = new Blocks(methods);
				var firstBlock = blocks.MethodBlocks.GetAllBlocks()[0];
				if (Checker(firstBlock, methods))
					for (var i = 0; i < firstBlock.Instructions.Count; i++)
					{
						//Console.WriteLine(firstBlock.Instructions[i]);
						if (firstBlock.Instructions[i].IsStloc() && firstBlock.Instructions[i - 1].IsLdcI4())
						{
							var loc = firstBlock.Instructions[i].Instruction.GetLocal(methods.Body.Variables);
							var val = firstBlock.Instructions[i - 1].GetLdcI4Value();
							if (val == 1544209)
							{

							}
							if (val == -1)
								dictionary.Add(loc, val);
							else if (val > 16035 || val < -10030)
							{
								if (!dictionary.ContainsKey(loc))
									dictionary.Add(loc, val);
							}

						}
					}
				for (var i = 0; i < methods.Body.Instructions.Count; i++)
					if (methods.Body.Instructions[i].IsLdloc())
					{
						var loc = methods.Body.Instructions[i].GetLocal(methods.Body.Variables);
						if (dictionary.ContainsKey(loc))
						{
							var val = dictionary[loc];
							if (val == -1)
							{
								if (!methods.Body.Instructions[i + 1].IsLdcI4() || methods.Body.Instructions[i + 2].OpCode != OpCodes.Ceq ||
								    !methods.Body.Instructions[i + 3].IsConditionalBranch()) continue;
								var val2 = methods.Body.Instructions[i + 1].GetLdcI4Value();
								if (val2 == -1)
								{

									methods.Body.Instructions[i].OpCode = OpCodes.Ldc_I4_M1;

								}

							}
							else
							{
								if (isReassigned(methods, loc) == 1)
								{

									methods.Body.Instructions[i].OpCode = OpCodes.Ldc_I4;
									methods.Body.Instructions[i].Operand = val;

								}
								else
								{

								}


							}
						}
					}
			}
		}

		public static int isReassigned(MethodDef methods, Local loc)
		{
			int amount = 0;
			foreach (Instruction bodyInstruction in methods.Body.Instructions)
			{
				if (bodyInstruction.IsStloc())
				{
					Local curLocal = bodyInstruction.GetLocal(methods.Body.Variables);
					if (curLocal == loc)
						amount++;
				}
			}
			return amount;
		}
	}
}
