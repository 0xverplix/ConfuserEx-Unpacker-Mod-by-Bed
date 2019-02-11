using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;
using SRE = System.Reflection.Emit;
namespace ConfuserEx_Unpacker.Protections.Mutations.SizeOf
{
	class Remover:Base
	{
		public override void Deobfuscate()
		{
			
			sizeofCleaner(ModuleDef);
		
		}

		public static void sizeofCleaner(ModuleDefMD module)
		{
			MemoryStream stream = new MemoryStream();
			ModuleWriterOptions moduleWriterOptions1 = new ModuleWriterOptions(module)
			{
				Logger = DummyLogger.NoThrowInstance,
				MetadataOptions = {Flags = MetadataFlags.PreserveAll},
				Cor20HeaderOptions = {Flags = dnlib.DotNet.MD.ComImageFlags.ILOnly}
			};
			module.Write(stream, moduleWriterOptions1);
        
			foreach (TypeDef types in module.GetTypes())
			{
				foreach (MethodDef methods in types.Methods)
				{
					if (!methods.HasBody) continue;
					foreach (Instruction t in methods.Body.Instructions)
					{
						if (t.OpCode != OpCodes.Sizeof) continue;
						ITypeDefOrRef type = (ITypeDefOrRef)t.Operand;
						if (type.FullName == "-") continue;
						try
						{


							var val = GetSize(type);
							Console.WriteLine("[!] SizeOf {0} is {1}",type,val);
							t.OpCode = OpCodes.Ldc_I4;
							t.Operand = val;



						}
						catch
						{
							// ignored
						}
					}
				}
			}
		}
		private static int GetSize(ITypeDefOrRef refOrDef, bool topmost = true)
		{

			int ret = 0;
			TypeDef target = refOrDef.ResolveTypeDef();

			if (target.Module.FullName.Contains("CommonLanguageRuntimeLibrary"))
			{
				return GetSize(refOrDef.ReflectionFullName);
			}

			if (!topmost && target.BaseType != null && target.BaseType.Name == "ValueType")
			{
				//ret += 1;
			}

			foreach (FieldDef fd in target.Fields)
			{
				if (fd.FieldType.TryGetTypeDef() != null)
				{
					int size = GetSize(fd.FieldType.ToTypeDefOrRef(), false);
					ret += size;
				}
				else
				{
					int size = GetSize(fd.FieldType.ReflectionFullName);
					ret += size;
				}
			}

			if (ret % 4 == 0) return ret;
			int rem = ret % 4;
			ret -= rem;
			ret += 4;

			return ret;
		}
		private static int GetSize(string target)
		{
			Type targetType = Type.GetType(target, false);
			if (targetType == null)
			{
				return -1;
			}

			SRE.DynamicMethod dm = new SRE.DynamicMethod("", typeof(int), null);

			SRE.ILGenerator gen = dm.GetILGenerator();

			gen.Emit(SRE.OpCodes.Sizeof, targetType);
			gen.Emit(SRE.OpCodes.Ret);

			SizeDM del = (SizeDM)dm.CreateDelegate(typeof(SizeDM));

			return del();
		}
		private delegate int SizeDM();

	}
}
