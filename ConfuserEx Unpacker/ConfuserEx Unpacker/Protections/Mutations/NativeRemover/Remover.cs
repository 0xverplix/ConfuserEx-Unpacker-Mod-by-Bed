using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace ConfuserEx_Unpacker.Protections.Mutations.NativeRemover
{
	class Remover:Base
	{
		public override void Deobfuscate()
		{
			Console.WriteLine("[!] Cleaning Native Conversions");
			NativeRemoverMethod(ModuleDef);
			
		}

		public static void NativeRemoverMethod(ModuleDefMD modulee)
		{
			foreach (var types in modulee.GetTypes())
			foreach (var methods in types.Methods)
			{
				//       if (!(methods.FullName.Contains("buttonConnect_Click"))) continue;
				if (!methods.HasBody) continue;
				foreach (Instruction t in methods.Body.Instructions)
					if (t.OpCode == OpCodes.Call)
						if (t.Operand.ToString().Contains("System.IntPtr::op_Explicit(System.Int32)"))
							t.OpCode = OpCodes.Nop;
						else if (t.Operand.ToString().Contains("System.IntPtr::op_Explicit(System.IntPtr)"))
							t.OpCode = OpCodes.Nop;
			}
		}
	}
}
