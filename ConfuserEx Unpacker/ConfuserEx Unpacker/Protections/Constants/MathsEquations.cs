using System;
using dnlib.DotNet;
using dnlib.DotNet.Emit;


namespace ConfuserEx_Unpacker.Protections.Constants
{
	// Token: 0x02000002 RID: 2
	internal class MathsEquations
	{
		// Token: 0x06000001 RID: 1 RVA: 0x0000205C File Offset: 0x0000025C
		public static void MathsFixer(ModuleDef module)
		{
			foreach (TypeDef current in module.Types)
			{
				foreach (MethodDef current2 in current.Methods)
				{
					bool flag = !current2.HasBody;
					if (!flag)
					{
						for (int i = 0; i < current2.Body.Instructions.Count; i++)
						{
							bool flag2 = current2.Body.Instructions[i].OpCode == OpCodes.Add;
							if (flag2)
							{
								bool flag3 = current2.Body.Instructions[i - 1].IsLdcI4() && current2.Body.Instructions[i - 2].IsLdcI4();
								if (flag3)
								{
									int num = current2.Body.Instructions[i - 2].GetLdcI4Value() + current2.Body.Instructions[i - 1].GetLdcI4Value();
									current2.Body.Instructions[i].OpCode = OpCodes.Ldc_I4;
									current2.Body.Instructions[i].Operand = num;
									current2.Body.Instructions[i - 2].OpCode = OpCodes.Nop;
									current2.Body.Instructions[i - 1].OpCode = OpCodes.Nop;
							
								}
							}
							else
							{
								bool flag4 = current2.Body.Instructions[i].OpCode == OpCodes.Mul;
								if (flag4)
								{
									bool flag5 = current2.Body.Instructions[i - 1].IsLdcI4() && current2.Body.Instructions[i - 2].IsLdcI4();
									if (flag5)
									{
										int num2 = current2.Body.Instructions[i - 2].GetLdcI4Value() * current2.Body.Instructions[i - 1].GetLdcI4Value();
										current2.Body.Instructions[i].OpCode = OpCodes.Ldc_I4;
										current2.Body.Instructions[i].Operand = num2;
										current2.Body.Instructions[i - 2].OpCode = OpCodes.Nop;
										current2.Body.Instructions[i - 1].OpCode = OpCodes.Nop;
									
									}
								}
								else
								{
									bool flag6 = current2.Body.Instructions[i].OpCode == OpCodes.Sub;
									if (flag6)
									{
										bool flag7 = current2.Body.Instructions[i - 1].IsLdcI4() && current2.Body.Instructions[i - 2].IsLdcI4();
										if (flag7)
										{
											int num3 = current2.Body.Instructions[i - 2].GetLdcI4Value() - current2.Body.Instructions[i - 1].GetLdcI4Value();
											current2.Body.Instructions[i].OpCode = OpCodes.Ldc_I4;
											current2.Body.Instructions[i].Operand = num3;
											current2.Body.Instructions[i - 2].OpCode = OpCodes.Nop;
											current2.Body.Instructions[i - 1].OpCode = OpCodes.Nop;
										
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}
}
