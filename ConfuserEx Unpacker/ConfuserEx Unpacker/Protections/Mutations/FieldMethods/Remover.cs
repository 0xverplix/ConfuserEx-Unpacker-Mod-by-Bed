using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CawkEmulatorV4;
using de4dot.blocks.cflow;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace ConfuserEx_Unpacker.Protections.Mutations.FieldMethods
{
	class Remover:Base
	{
		public override void Deobfuscate()
		{
			Console.WriteLine("[!] Cleaning Fields Assigned In Method");
			fieldFixer(ModuleDef);
		}

		public static void fieldFixer(ModuleDefMD module)
		{
			foreach (var types in module.GetTypes())
			foreach (var method in types.Methods)
			{
				string fieldName = null;
				int? fieldValue = null;
				if (!method.HasBody) continue;
				if (method.MDToken.ToInt32() == 0x06000002)
				{
				}
				for (var i = 0; i < method.Body.Instructions.Count; i++)
					if (fieldName == null)
					{

						if (method.Body.Instructions[i].OpCode == OpCodes.Call && method.Body.Instructions[i].Operand is MethodDef)
							if (method.Body.Instructions[i].Operand.ToString().ToLower().Contains("int32") &&
							    method.Body.Instructions[i].Operand.ToString().ToLower().Contains("void") && method.Body.Instructions[i]
								    .Operand.ToString().Contains(method.DeclaringType.Name))
								if (method.Body.Instructions[i - 1].IsLdcI4())
									if (method.Body.Instructions[i].Operand.ToString().Contains("ErrorWrapperApplicationException"))
									{
									}
									else
									{
										var setMethod = (MethodDef)method.Body.Instructions[i].Operand;
										if (!setMethod.HasBody) continue;
										if (setMethod.Body.Instructions[setMethod.Body.Instructions.Count - 2]
											    .OpCode == OpCodes.Stsfld)
										{
										
											fieldValue = GetFieldValue(setMethod, module, method.Body.Instructions[i - 1].GetLdcI4Value(),
												out fieldName);
											if (fieldValue == null)
											{
												throw new Exception("Field Value Is Empty!!");
												fieldValue = GetFieldValue(setMethod, module, method.Body.Instructions[i - 1].GetLdcI4Value(),
													out fieldName);
											}
										}
									}
					}
					else
					{
						if (method.Body.Instructions[i].Operand is FieldDef)
							if (method.Body.Instructions[i].OpCode == OpCodes.Ldsfld &&
							    method.Body.Instructions[i].Operand.ToString().Contains("System.Int32") && method.Body.Instructions[i]
								    .Operand.ToString().Contains(((FieldDef)method.Body.Instructions[i].Operand).Name))
								if (fieldValue != null)
								{
									method.Body.Instructions[i].OpCode = OpCodes.Ldc_I4;
									method.Body.Instructions[i].Operand = fieldValue;
								}
					}
			}
		}

		public static int? GetFieldValue(MethodDef method, ModuleDefMD module, int argValue, out string fieldName)
		{
			fieldName = null;

			var insemu = new Emulation(method);
			insemu.ValueStack.Parameters[method.Parameters[0]] = argValue;
			insemu.Emulate();
			fieldName = insemu.ValueStack.Fields.First().Key.Name;
			insemu.Emulate();
			var value = (int)insemu.ValueStack.Fields.First().Value;

			
			return value;

		}
	}
}
