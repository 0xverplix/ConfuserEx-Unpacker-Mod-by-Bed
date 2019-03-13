using de4dot.blocks;
using de4dot.blocks.cflow;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfuserEx_Unpacker.Protections.Control_Flow
{
    class ExpersionRemover : Base
    {
        public override void Deobfuscate()
        {
            unpack(ModuleDef);
        }





        private void unpack(ModuleDef module)
        {
            this.methods = new List<MethodDef>();
            if (module.HasTypes)
            {
                foreach (TypeDef type in module.Types)
                {
                    this.AddMethods(type);
                }
            }
            BlocksCflowDeobfuscator blocksCflowDeobfuscator = new BlocksCflowDeobfuscator();
            for (int i = 0; i < this.methods.Count; i++)
            {
                Blocks blocks = new Blocks(this.methods[i]);
                blocksCflowDeobfuscator.Initialize(blocks);
                blocksCflowDeobfuscator.Deobfuscate();
                blocks.RepartitionBlocks();
                IList<Instruction> list;
                IList<ExceptionHandler> exceptionHandlers;
                blocks.GetCode(out list, out exceptionHandlers);
                DotNetUtils.RestoreBody(this.methods[i], list, exceptionHandlers);
            }
            for (int i = 0; i < this.methods.Count; i++)
            {
                for (int j = 0; j < this.methods[i].Body.Instructions.Count; j++)
                {
                    if (this.methods[i].Body.Instructions[j].IsLdcI4() && j + 1 < this.methods[i].Body.Instructions.Count && this.methods[i].Body.Instructions[j + 1].OpCode == OpCodes.Pop)
                    {
                        this.methods[i].Body.Instructions[j].OpCode = OpCodes.Nop;
                        this.methods[i].Body.Instructions[j + 1].OpCode = OpCodes.Nop;
                        for (int k = 0; k < this.methods[i].Body.Instructions.Count; k++)
                        {
                            if (this.methods[i].Body.Instructions[k].OpCode == OpCodes.Br || this.methods[i].Body.Instructions[k].OpCode == OpCodes.Br_S)
                            {
                                if (this.methods[i].Body.Instructions[k].Operand is Instruction)
                                {
                                    Instruction instruction = this.methods[i].Body.Instructions[k].Operand as Instruction;
                                    if (instruction == this.methods[i].Body.Instructions[j + 1])
                                    {
                                        if (k - 1 >= 0 && this.methods[i].Body.Instructions[k - 1].IsLdcI4())
                                        {
                                            this.methods[i].Body.Instructions[k - 1].OpCode = OpCodes.Nop;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (this.methods[i].Body.Instructions[j].OpCode == OpCodes.Dup && j + 1 < this.methods[i].Body.Instructions.Count && this.methods[i].Body.Instructions[j + 1].OpCode == OpCodes.Pop)
                    {
                        this.methods[i].Body.Instructions[j].OpCode = OpCodes.Nop;
                        this.methods[i].Body.Instructions[j + 1].OpCode = OpCodes.Nop;
                        for (int k = 0; k < this.methods[i].Body.Instructions.Count; k++)
                        {
                            if (this.methods[i].Body.Instructions[k].OpCode == OpCodes.Br || this.methods[i].Body.Instructions[k].OpCode == OpCodes.Br_S)
                            {
                                if (this.methods[i].Body.Instructions[k].Operand is Instruction)
                                {
                                    Instruction instruction = this.methods[i].Body.Instructions[k].Operand as Instruction;
                                    if (instruction == this.methods[i].Body.Instructions[j + 1])
                                    {
                                        if (k - 1 >= 0 && this.methods[i].Body.Instructions[k - 1].OpCode == OpCodes.Dup)
                                        {
                                            this.methods[i].Body.Instructions[k - 1].OpCode = OpCodes.Nop;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < this.methods.Count; i++)
            {
                Blocks blocks = new Blocks(this.methods[i]);
                blocksCflowDeobfuscator.Initialize(blocks);
                blocksCflowDeobfuscator.Deobfuscate();
                blocks.RepartitionBlocks();
                IList<Instruction> list;
                IList<ExceptionHandler> exceptionHandlers;
                blocks.GetCode(out list, out exceptionHandlers);
                DotNetUtils.RestoreBody(this.methods[i], list, exceptionHandlers);
            }
            for (int i = 0; i < this.methods.Count; i++)
            {
                Dictionary<Instruction, Instruction> dictionary = new Dictionary<Instruction, Instruction>();
                for (int j = 0; j < this.methods[i].Body.Instructions.Count; j++)
                {
                    if (this.methods[i].Body.Instructions[j].IsConditionalBranch())
                    {
                        Instruction instruction2 = this.methods[i].Body.Instructions[j];
                        for (int k = 0; k < this.methods[i].Body.Instructions.Count; k++)
                        {
                            if (this.methods[i].Body.Instructions[k].IsBr())
                            {
                                Instruction instruction3 = this.methods[i].Body.Instructions[k];
                                Instruction instruction4 = this.methods[i].Body.Instructions[k].Operand as Instruction;
                                if (instruction4 == instruction2)
                                {
                                    if (!dictionary.ContainsKey(instruction4))
                                    {
                                        this.methods[i].Body.Instructions[k].OpCode = instruction2.GetOpCode();
                                        this.methods[i].Body.Instructions[k].Operand = instruction2.GetOperand();
                                        this.methods[i].Body.Instructions.Insert(k + 1, OpCodes.Br.ToInstruction(this.methods[i].Body.Instructions[j + 1]));
                                        k++;
                                        dictionary.Add(instruction4, this.methods[i].Body.Instructions[k]);
                                    }
                                }
                            }
                        }
                    }
                }
                this.methods[i].Body.SimplifyBranches();
                this.methods[i].Body.OptimizeBranches();
            }
            for (int i = 0; i < this.methods.Count; i++)
            {
                Blocks blocks = new Blocks(this.methods[i]);
                blocksCflowDeobfuscator.Initialize(blocks);
                blocksCflowDeobfuscator.Deobfuscate();
                blocks.RepartitionBlocks();
                IList<Instruction> list;
                IList<ExceptionHandler> exceptionHandlers;
                blocks.GetCode(out list, out exceptionHandlers);
                DotNetUtils.RestoreBody(this.methods[i], list, exceptionHandlers);
            }
            int num = 0;
            for (int i = 0; i < this.methods.Count; i++)
            {
                this.toberemoved = new List<int>();
                this.integer_values_1 = new List<int>();
                this.tobenooped_start = new List<int>();
                this.tobenooped_len = new List<int>();
                this.for_rem = new List<int>();
                this.switchinstructions = new List<Instruction>();
                this.wheretojump = new List<Instruction>();
                for (int j = 0; j < this.methods[i].Body.Instructions.Count; j++)
                {
                    if (j + 2 < this.methods[i].Body.Instructions.Count && this.methods[i].Body.Instructions[j].IsStloc() && (this.methods[i].Body.Instructions[j + 1].IsLdloc() || this.methods[i].Body.Instructions[j + 1].IsLdcI4()) && (this.methods[i].Body.Instructions[j + 2].IsLdcI4() || this.methods[i].Body.Instructions[j + 2].IsLdloc() || this.methods[i].Body.Instructions[j + 2].OpCode == OpCodes.Neg || this.methods[i].Body.Instructions[j + 2].OpCode == OpCodes.Not))
                    {
                        int stlocInsIndex = GetStlocInsIndex(this.methods[i].Body.Instructions, j);
                        if (stlocInsIndex != -1)
                        {
                            Instruction instruction5 = this.methods[i].Body.Instructions[stlocInsIndex - 1];
                            if ((stlocInsIndex - 1 >= 0 && this.methods[i].Body.Instructions[stlocInsIndex - 1].IsBr()) || this.methods[i].Body.Instructions[stlocInsIndex - 1].IsLdloc() || this.methods[i].Body.Instructions[stlocInsIndex - 1].IsLdcI4() || this.methods[i].Body.Instructions[stlocInsIndex - 1].OpCode == OpCodes.Xor)
                            {
                                int stlocInsIndex2 = GetStlocInsIndex(this.methods[i].Body.Instructions, stlocInsIndex + 1);
                                if (stlocInsIndex2 != -1)
                                {
                                    int switchInsIndex = GetSwitchInsIndex(this.methods[i].Body.Instructions, stlocInsIndex2 + 1);
                                    if (switchInsIndex != -1)
                                    {
                                        this.local_variable1 = this.methods[i].Body.Instructions[stlocInsIndex].GetLocal(this.methods[i].Body.Variables);
                                        this.local_variable2 = this.methods[i].Body.Instructions[stlocInsIndex2].GetLocal(this.methods[i].Body.Variables);
                                        if (this.local_variable1.Type != null && (this.local_variable1.Type.FullName == "System.Int32" || this.local_variable1.Type.FullName == "System.UInt32") && this.local_variable2.Type != null && (this.local_variable2.Type.FullName == "System.Int32" || this.local_variable2.Type.FullName == "System.UInt32"))
                                        {
                                            int num2 = switchInsIndex - j;
                                            this.wheretojump.Add(this.methods[i].Body.Instructions[j]);
                                            this.switchinstructions.Add(this.methods[i].Body.Instructions[switchInsIndex]);
                                            this.tobenooped_start.Add(j);
                                            this.tobenooped_len.Add(num2);
                                            j += num2;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (this.switchinstructions.Count > 0)
                {
                    this.placeintindexes = new List<int>();
                    this.intvalues = new List<int>();
                    this.conditionalinstructions = new List<Instruction>();
                    this.brinstructions = new List<Instruction>();
                    this.realbrinstructions = new List<Instruction>();
                    this.instructions = this.methods[i].Body.Instructions;
                    this.method = this.methods[i];
                    this.InstructionParse2(0, 0, 0);
                    num += this.placeintindexes.Count;
                    for (int k = 0; k < this.placeintindexes.Count; k++)
                    {
                        this.methods[i].Body.Instructions[this.placeintindexes[k]].OpCode = OpCodes.Ldc_I4;
                        this.methods[i].Body.Instructions[this.placeintindexes[k]].Operand = this.intvalues[k];
                    }
                    for (int k = 0; k < this.tobenooped_start.Count; k++)
                    {
                        for (int l = 0; l < this.tobenooped_len[k]; l++)
                        {
                            this.methods[i].Body.Instructions[this.tobenooped_start[k] + l].OpCode = OpCodes.Nop;
                        }
                    }
                }
                this.toberemoved = new List<int>();
                this.integer_values_1 = new List<int>();
                this.for_rem = new List<int>();
                this.switchinstructions = new List<Instruction>();
                for (int j = 0; j < this.methods[i].Body.Instructions.Count; j++)
                {
                    if (j + 6 < this.methods[i].Body.Instructions.Count && this.methods[i].Body.Instructions[j].IsLdcI4())
                    {
                        if (this.methods[i].Body.Instructions[j + 1].OpCode == OpCodes.Xor)
                        {
                            if (this.methods[i].Body.Instructions[j + 2].IsLdcI4())
                            {
                                if (this.methods[i].Body.Instructions[j + 3].OpCode == OpCodes.Rem_Un)
                                {
                                    if (this.methods[i].Body.Instructions[j + 4].OpCode == OpCodes.Switch)
                                    {
                                        this.toberemoved.Add(j);
                                        this.integer_values_1.Add(this.methods[i].Body.Instructions[j].GetLdcI4Value());
                                        this.for_rem.Add(this.methods[i].Body.Instructions[j + 2].GetLdcI4Value());
                                        this.switchinstructions.Add(this.methods[i].Body.Instructions[j + 4]);
                                    }
                                }
                            }
                        }
                    }
                }
                if (this.switchinstructions.Count > 0)
                {
                    this.toberemovedindex = new List<int>();
                    this.toberemovedvalues = new List<int>();
                    this.conditionalinstructions = new List<Instruction>();
                    this.brinstructions = new List<Instruction>();
                    this.realbrinstructions = new List<Instruction>();
                    this.instructions = this.methods[i].Body.Instructions;
                    this.method = this.methods[i];
                    this.InstructionParseNoLocal(0);
                    num += this.toberemovedindex.Count;
                    if (this.toberemovedindex.Count > 0)
                    {
                        for (int m = 0; m < this.toberemoved.Count; m++)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                this.methods[i].Body.Instructions[j + this.toberemoved[m]].OpCode = OpCodes.Nop;
                                this.methods[i].Body.Instructions[j + this.toberemoved[m]].Operand = null;
                            }
                        }
                        for (int j = 0; j < this.toberemovedindex.Count; j++)
                        {
                            this.methods[i].Body.Instructions[this.toberemovedindex[j]].OpCode = OpCodes.Ldc_I4;
                            this.methods[i].Body.Instructions[this.toberemovedindex[j]].Operand = this.toberemovedvalues[j];
                            if (!this.methods[i].Body.Instructions[this.toberemovedindex[j] + 1].IsBr())
                            {
                                for (int k = 0; k < 4; k++)
                                {
                                    this.methods[i].Body.Instructions[this.toberemovedindex[j] + k + 1].OpCode = OpCodes.Nop;
                                    this.methods[i].Body.Instructions[this.toberemovedindex[j] + k + 1].Operand = null;
                                }
                            }
                        }
                    }
                }
                Blocks blocks = new Blocks(this.methods[i]);
                blocksCflowDeobfuscator.Initialize(blocks);
                blocksCflowDeobfuscator.Deobfuscate();
                blocks.RepartitionBlocks();
                IList<Instruction> list;
                IList<ExceptionHandler> exceptionHandlers;
                blocks.GetCode(out list, out exceptionHandlers);
                DotNetUtils.RestoreBody(this.methods[i], list, exceptionHandlers);
                this.methods[i].Body.SimplifyBranches();
                this.methods[i].Body.OptimizeBranches();
            }
            for (int i = 0; i < this.methods.Count; i++)
            {
                Blocks blocks = new Blocks(this.methods[i]);
                blocksCflowDeobfuscator.Initialize(blocks);
                blocksCflowDeobfuscator.Deobfuscate();
                blocks.RepartitionBlocks();
                IList<Instruction> list;
                IList<ExceptionHandler> exceptionHandlers;
                blocks.GetCode(out list, out exceptionHandlers);
                DotNetUtils.RestoreBody(this.methods[i], list, exceptionHandlers);
            }
         
          
        
    }


























        /// <summary>
        /// ////////////////////////////////////
        /// </summary>
        /// <param name="type"></param>
        public void AddMethods(TypeDef type)
        {
            if (type.HasMethods)
            {
                foreach (MethodDef methodDef in type.Methods)
                {
                    if (methodDef.HasBody)
                    {
                        this.methods.Add(methodDef);
                    }
                }
            }
            if (type.HasNestedTypes)
            {
                foreach (TypeDef type2 in type.NestedTypes)
                {
                    this.AddMethods(type2);
                }
            }
        }
        public bool IsArithmetic(Instruction ins)
        {
            return ins.OpCode == OpCodes.Add || ins.OpCode == OpCodes.Add_Ovf || ins.OpCode == OpCodes.Add_Ovf_Un || (ins.OpCode == OpCodes.Sub || ins.OpCode == OpCodes.Sub_Ovf || ins.OpCode == OpCodes.Sub_Ovf_Un) || (ins.OpCode == OpCodes.Mul || ins.OpCode == OpCodes.Mul_Ovf || ins.OpCode == OpCodes.Mul_Ovf_Un) || (ins.OpCode == OpCodes.Div || ins.OpCode == OpCodes.Div_Un) || ins.OpCode == OpCodes.Shl || (ins.OpCode == OpCodes.Shr || ins.OpCode == OpCodes.Shr_Un) || (ins.OpCode == OpCodes.Rem || ins.OpCode == OpCodes.Rem_Un) || (ins.OpCode == OpCodes.And || ins.OpCode == OpCodes.Or || ins.OpCode == OpCodes.Xor);
        }
        public int CalculateArithmetic(int value1, int value2, Instruction ins)
        {
            int result;
            if (ins.OpCode == OpCodes.Add || ins.OpCode == OpCodes.Add_Ovf)
            {
                result = value1 + value2;
            }
            else if (ins.OpCode == OpCodes.Add_Ovf_Un)
            {
                result = value1 + value2;
            }
            else if (ins.OpCode == OpCodes.Sub || ins.OpCode == OpCodes.Sub_Ovf)
            {
                result = value1 - value2;
            }
            else if (ins.OpCode == OpCodes.Sub_Ovf_Un)
            {
                result = value1 - value2;
            }
            else if (ins.OpCode == OpCodes.Mul || ins.OpCode == OpCodes.Mul_Ovf)
            {
                result = value1 * value2;
            }
            else if (ins.OpCode == OpCodes.Mul_Ovf_Un)
            {
                result = value1 * value2;
            }
            else if (ins.OpCode == OpCodes.Div)
            {
                result = value1 / value2;
            }
            else if (ins.OpCode == OpCodes.Div_Un)
            {
                result = value1 / value2;
            }
            else if (ins.OpCode == OpCodes.Shl)
            {
                result = value1 << value2;
            }
            else if (ins.OpCode == OpCodes.Shr)
            {
                result = value1 >> value2;
            }
            else if (ins.OpCode == OpCodes.Shr_Un)
            {
                result = (int)((uint)value1 >> value2);
            }
            else if (ins.OpCode == OpCodes.Rem)
            {
                result = value1 % value2;
            }
            else if (ins.OpCode == OpCodes.Rem_Un)
            {
                result = value1 % value2;
            }
            else if (ins.OpCode == OpCodes.And)
            {
                result = (value1 & value2);
            }
            else if (ins.OpCode == OpCodes.Or)
            {
                result = (value1 | value2);
            }
            else if (ins.OpCode == OpCodes.Xor)
            {
                result = (value1 ^ value2);
            }
            else
            {
                result = -1;
            }
            return result;
        }
        public int EmulateExpression(int ins_index, ref int local_value1, ref int local_value2)
        {
            int result = -1;
            for (int i = ins_index; i < this.instructions.Count; i++)
            {
                if (this.instructions[i].IsStloc())
                {
                    Local local = this.instructions[i].GetLocal(this.method.Body.Variables);
                    if (local == this.local_variable1)
                    {
                        local_value1 = StackEmulator.PopValue();
                    }
                    if (local == this.local_variable2)
                    {
                        local_value2 = StackEmulator.PopValue();
                    }
                }
                if (this.instructions[i].IsLdloc())
                {
                    Local local = this.instructions[i].GetLocal(this.method.Body.Variables);
                    if (local == this.local_variable1)
                    {
                        StackEmulator.PushValue(local_value1);
                    }
                    if (local == this.local_variable2)
                    {
                        StackEmulator.PushValue(local_value2);
                    }
                }
                if (this.instructions[i].IsLdcI4())
                {
                    int num = this.instructions[i].GetLdcI4Value();
                    StackEmulator.PushValue(num);
                }
                if (this.IsArithmetic(this.instructions[i]))
                {
                    int value = StackEmulator.PopValue();
                    int value2 = StackEmulator.PopValue();
                    int value3 = this.CalculateArithmetic(value2, value, this.instructions[i]);
                    StackEmulator.PushValue(value3);
                }
                if (this.instructions[i].OpCode == OpCodes.Neg)
                {
                    int num = StackEmulator.PopValue();
                    num = -num;
                    StackEmulator.PushValue(num);
                }
                if (this.instructions[i].OpCode == OpCodes.Not)
                {
                    int num = StackEmulator.PopValue();
                    num = ~num;
                    StackEmulator.PushValue(num);
                }
                if (this.instructions[i].OpCode == OpCodes.Dup)
                {
                    StackEmulator.Dup();
                }
                if (this.instructions[i].OpCode == OpCodes.Switch)
                {
                    result = StackEmulator.PopValue();
                    break;
                }
            }
            return result;
        }
        public bool IsConfuserExpression(Instruction ins)
        {
            return ins.IsLdcI4() || ins.IsStloc() || ins.IsLdloc() || this.IsArithmetic(ins) || ins.OpCode == OpCodes.Neg || ins.OpCode == OpCodes.Not || ins.OpCode == OpCodes.Dup || ins.OpCode == OpCodes.Switch;
        }
        public void InstructionParse2(int ins_index, int local_value1, int local_value2)
        {
            for (int i = ins_index; i < this.instructions.Count; i++)
            {
                if (!this.placeintindexes.Contains(i))
                {
                    if (this.instructions[i].IsBr())
                    {
                        Instruction item = this.instructions[i].Operand as Instruction;
                        if (!this.brinstructions.Contains(item) && !this.realbrinstructions.Contains(item))
                        {
                            this.realbrinstructions.Add(item);
                            int ins_index2 = this.instructions.IndexOf(item);
                            this.InstructionParse2(ins_index2, local_value1, local_value2);
                        }
                        break;
                    }
                    if (this.instructions[i].IsConditionalBranch() || this.instructions[i].IsLeave())
                    {
                        Instruction item = this.instructions[i].Operand as Instruction;
                        if (!this.conditionalinstructions.Contains(item))
                        {
                            this.conditionalinstructions.Add(item);
                            int ins_index3 = this.instructions.IndexOf(item);
                            this.InstructionParse2(ins_index3, local_value1, local_value2);
                            if (i + 1 < this.instructions.Count)
                            {
                                int ins_index4 = i + 1;
                                this.InstructionParse2(ins_index4, local_value1, local_value2);
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (this.instructions[i].OpCode == OpCodes.Ret)
                        {
                            break;
                        }
                        if (i + 1 < this.instructions.Count && (this.instructions[i].IsLdcI4() || this.instructions[i].IsLdloc()) && (this.instructions[i + 1].IsStloc() || this.instructions[i + 1].IsLdcI4() || this.instructions[i + 1].IsLdloc() || this.instructions[i + 1].IsBr()))
                        {
                            int num = 0;
                            if (this.instructions[i].IsLdcI4())
                            {
                                num = this.instructions[i].GetLdcI4Value();
                            }
                            if (this.instructions[i].IsLdloc())
                            {
                                Local local = this.instructions[i].GetLocal(this.method.Body.Variables);
                                if (local == this.local_variable1)
                                {
                                    num = local_value1;
                                }
                                if (local == this.local_variable2)
                                {
                                    num = local_value2;
                                }
                            }
                            int num2 = i + 1;
                            if (this.instructions[i + 1].IsBr() && this.instructions[i + 1].Operand is Instruction)
                            {
                                num2 = this.instructions.IndexOf(this.instructions[i + 1].Operand as Instruction);
                            }
                            int value;
                            int num5;
                            if (num2 + 3 < this.instructions.Count && (this.instructions[num2].IsLdcI4() || this.instructions[num2].IsLdloc()) && (this.instructions[num2 + 1].IsLdcI4() || this.instructions[num2 + 1].IsLdloc() || this.instructions[num2 + 1].OpCode == OpCodes.Mul) && (this.instructions[num2 + 2].IsLdcI4() || this.instructions[num2 + 2].IsLdloc() || this.instructions[num2 + 2].OpCode == OpCodes.Mul) && this.instructions[num2 + 3].OpCode == OpCodes.Xor)
                            {
                                int num3 = 0;
                                if (this.instructions[num2].IsLdcI4())
                                {
                                    num3 = this.instructions[num2].GetLdcI4Value();
                                }
                                if (this.instructions[num2].IsLdloc())
                                {
                                    Local local = this.instructions[num2].GetLocal(this.method.Body.Variables);
                                    if (local == this.local_variable1)
                                    {
                                        num3 = local_value1;
                                    }
                                    if (local == this.local_variable2)
                                    {
                                        num3 = local_value2;
                                    }
                                }
                                int num4 = 0;
                                if (this.instructions[num2 + 1].OpCode == OpCodes.Mul)
                                {
                                    if (this.instructions[num2 + 2].IsLdcI4())
                                    {
                                        num4 = this.instructions[num2 + 2].GetLdcI4Value();
                                    }
                                    if (this.instructions[num2 + 2].IsLdloc())
                                    {
                                        Local local = this.instructions[num2 + 2].GetLocal(this.method.Body.Variables);
                                        if (local == this.local_variable1)
                                        {
                                            num4 = local_value1;
                                        }
                                        if (local == this.local_variable2)
                                        {
                                            num4 = local_value2;
                                        }
                                    }
                                }
                                else
                                {
                                    if (this.instructions[num2 + 1].IsLdcI4())
                                    {
                                        num4 = this.instructions[num2 + 1].GetLdcI4Value();
                                    }
                                    if (this.instructions[num2 + 1].IsLdloc())
                                    {
                                        Local local = this.instructions[num2 + 1].GetLocal(this.method.Body.Variables);
                                        if (local == this.local_variable1)
                                        {
                                            num4 = local_value1;
                                        }
                                        if (local == this.local_variable2)
                                        {
                                            num4 = local_value2;
                                        }
                                    }
                                }
                                if (this.instructions[num2 + 1].OpCode == OpCodes.Mul)
                                {
                                    value = (num * num3 ^ num4);
                                }
                                else
                                {
                                    value = (num4 * num3 ^ num);
                                }
                                num5 = num2 + 4;
                            }
                            else
                            {
                                value = num;
                                num5 = i + 1;
                            }
                            Instruction instruction;
                            if (this.instructions[num5].IsBr())
                            {
                                instruction = (this.instructions[num5].Operand as Instruction);
                            }
                            else
                            {
                                instruction = this.instructions[num5];
                            }
                            int num6 = -1;
                            for (int j = 0; j < this.wheretojump.Count; j++)
                            {
                                if (this.wheretojump[j] == instruction)
                                {
                                    num6 = j;
                                    break;
                                }
                            }
                            if (num6 != -1)
                            {
                                StackEmulator.InitStack();
                                StackEmulator.PushValue(value);
                                int num7 = this.EmulateExpression(this.instructions.IndexOf(instruction), ref local_value1, ref local_value2);
                                Instruction[] array = this.switchinstructions[num6].Operand as Instruction[];
                                Instruction item2 = array[num7];
                                this.placeintindexes.Add(i);
                                this.intvalues.Add(num7);
                                if (!this.instructions[i + 1].IsBr())
                                {
                                    int num8 = i + 1;
                                    int item3 = num5 - num8;
                                    this.tobenooped_start.Add(num8);
                                    this.tobenooped_len.Add(item3);
                                }
                                if (!this.brinstructions.Contains(item2))
                                {
                                    this.brinstructions.Add(item2);
                                    this.InstructionParse2(this.instructions.IndexOf(item2), local_value1, local_value2);
                                    break;
                                }
                            }
                        }
                        else if (this.instructions[i].OpCode == OpCodes.Switch)
                        {
                            Instruction instruction2 = this.instructions[i];
                            bool flag = false;
                            for (int j = 0; j < this.switchinstructions.Count; j++)
                            {
                                if (this.switchinstructions[j] == this.instructions[i])
                                {
                                    flag = true;
                                    break;
                                }
                            }
                            if (!flag)
                            {
                                foreach (Instruction item4 in this.instructions[i].Operand as Instruction[])
                                {
                                    this.InstructionParse2(this.instructions.IndexOf(item4), local_value1, local_value2);
                                }
                            }
                        }
                    }
                }
            }
        }
        public void InstructionParseNoLocal(int ins_index)
        {
            for (int i = ins_index; i < this.instructions.Count; i++)
            {
                Instruction instruction = this.instructions[i];
                MethodDef methodDef = this.method;
                if (!this.toberemovedindex.Contains(i))
                {
                    if (this.instructions[i].IsBr())
                    {
                        Instruction item = this.instructions[i].Operand as Instruction;
                        if (!this.brinstructions.Contains(item) && !this.realbrinstructions.Contains(item))
                        {
                            this.realbrinstructions.Add(item);
                            int ins_index2 = this.instructions.IndexOf(item);
                            this.InstructionParseNoLocal(ins_index2);
                        }
                        break;
                    }
                    if (this.instructions[i].IsConditionalBranch() || this.instructions[i].IsLeave())
                    {
                        Instruction item = this.instructions[i].Operand as Instruction;
                        if (!this.conditionalinstructions.Contains(item))
                        {
                            this.conditionalinstructions.Add(item);
                            int ins_index3 = this.instructions.IndexOf(item);
                            this.InstructionParseNoLocal(ins_index3);
                            if (i + 1 < this.instructions.Count)
                            {
                                int ins_index4 = i + 1;
                                this.InstructionParseNoLocal(ins_index4);
                            }
                        }
                    }
                    else
                    {
                        if (this.instructions[i].OpCode == OpCodes.Ret)
                        {
                            break;
                        }
                        if (this.instructions[i].IsLdcI4())
                        {
                            uint num = 0u;
                            if (this.instructions[i].IsLdcI4())
                            {
                                num = (uint)this.instructions[i].GetLdcI4Value();
                            }
                            int num2 = i + 1;
                            if (this.instructions[i + 1].IsBr())
                            {
                                Instruction item2 = this.instructions[i + 1].Operand as Instruction;
                                num2 = this.instructions.IndexOf(item2);
                            }
                            if (this.instructions[num2].IsLdcI4())
                            {
                                uint num3 = 0u;
                                if (this.instructions[num2].IsLdcI4())
                                {
                                    num3 = (uint)this.instructions[num2].GetLdcI4Value();
                                }
                                uint num4 = 0u;
                                if ((this.instructions[num2 + 1].OpCode == OpCodes.Mul && this.instructions[num2 + 2].IsLdcI4()) || (this.instructions[num2 + 1].IsLdcI4() && this.instructions[num2 + 2].OpCode == OpCodes.Mul) || this.instructions[num2 + 1].OpCode == OpCodes.Xor)
                                {
                                    if (this.instructions[num2 + 1].OpCode != OpCodes.Xor)
                                    {
                                        if (this.instructions[num2 + 1].OpCode == OpCodes.Mul && this.instructions[num2 + 2].IsLdcI4())
                                        {
                                            num4 = (uint)this.instructions[num2 + 2].GetLdcI4Value();
                                        }
                                        if (this.instructions[num2 + 1].IsLdcI4() && this.instructions[num2 + 2].OpCode == OpCodes.Mul)
                                        {
                                            num4 = (uint)this.instructions[num2 + 1].GetLdcI4Value();
                                        }
                                    }
                                    if (this.instructions[num2 + 3].OpCode == OpCodes.Xor || this.instructions[num2 + 1].OpCode == OpCodes.Xor)
                                    {
                                        for (int j = 0; j < this.toberemoved.Count; j++)
                                        {
                                            if ((this.instructions[num2 + 4].IsBr() && this.instructions[num2 + 4].Operand as Instruction == this.instructions[this.toberemoved[j]]) || num2 + 4 == this.toberemoved[j] || (this.instructions[num2 + 1].OpCode == OpCodes.Xor && num2 == this.toberemoved[j]))
                                            {
                                                uint num5;
                                                if (this.instructions[num2 + 1].OpCode == OpCodes.Xor)
                                                {
                                                    num5 = (num ^ num3);
                                                }
                                                else if (this.instructions[num2 + 1].IsLdcI4() || this.instructions[num2 + 1].IsLdloc())
                                                {
                                                    num5 = (num4 * num3 ^ num);
                                                }
                                                else
                                                {
                                                    num5 = (num * num3 ^ num4);
                                                }
                                                uint num6;
                                                if (this.instructions[num2 + 1].OpCode != OpCodes.Xor)
                                                {
                                                    num6 = (num5 ^ (uint)this.integer_values_1[j]);
                                                }
                                                else
                                                {
                                                    num6 = num5;
                                                }
                                                uint num7 = num6 % (uint)this.for_rem[j];
                                                Instruction[] array = this.switchinstructions[j].Operand as Instruction[];
                                                Instruction item3 = array[(int)((UIntPtr)num7)];
                                                if (this.toberemovedindex.Contains(i))
                                                {
                                                }
                                                this.toberemovedindex.Add(i);
                                                this.toberemovedvalues.Add((int)num7);
                                                bool flag = false;
                                                if (this.brinstructions.IndexOf(item3) != -1)
                                                {
                                                    flag = true;
                                                }
                                                if (flag)
                                                {
                                                    this.brinstructions.Add(item3);
                                                    this.InstructionParseNoLocal(this.instructions.IndexOf(item3));
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (this.instructions[i].OpCode == OpCodes.Switch)
                        {
                            bool flag2;
                            if (i - 4 < 0)
                            {
                                flag2 = false;
                            }
                            else
                            {
                                flag2 = false;
                                for (int j = 0; j < this.toberemoved.Count; j++)
                                {
                                    int num8 = this.toberemoved[j];
                                    if (i - 4 == this.toberemoved[j])
                                    {
                                        flag2 = true;
                                        break;
                                    }
                                }
                            }
                            if (!flag2)
                            {
                                foreach (Instruction item4 in this.instructions[i].Operand as Instruction[])
                                {
                                    this.InstructionParseNoLocal(this.instructions.IndexOf(item4));
                                }
                            }
                        }
                    }
                }
            }
        }

        public static int GetStlocInsIndex(IList<Instruction> m_ins, int ins_index)
        {
            int i = ins_index;
            while (i < m_ins.Count)
            {
                int result;
                if (m_ins[i].IsBr())
                {
                    result = -1;
                }
                else
                {
                    if (!m_ins[i].IsStloc())
                    {
                        i++;
                        continue;
                    }
                    result = i;
                }
                return result;
            }
            return -1;
        }

        public static int GetSwitchInsIndex(IList<Instruction> m_ins, int ins_index)
        {
            int i = ins_index;
            while (i < m_ins.Count)
            {
                int result;
                if (m_ins[i].IsBr())
                {
                    result = -1;
                }
                else
                {
                    if (i + 1 >= m_ins.Count || m_ins[i].OpCode != OpCodes.Rem_Un || m_ins[i + 1].OpCode != OpCodes.Switch)
                    {
                        i++;
                        continue;
                    }
                    result = i + 1;
                }
                return result;
            }
            return -1;
        }
        public string DirectoryName = "";

        private List<MethodDef> methods = new List<MethodDef>();

        private Local local_variable1 = null;

        private Local local_variable2 = null;

        private List<int> toberemoved;

        private List<int> integer_values_1;

        private List<int> for_rem;

        private List<int> tobenooped_start;

        private List<int> tobenooped_len;

        private List<Instruction> switchinstructions;

        private List<Instruction> wheretojump;

        private IList<Instruction> instructions;

        private List<int> placeintindexes;

        private List<int> intvalues;

        private List<int> toberemovedindex;

        private List<int> toberemovedvalues;

        private MethodDef method;

        private List<Instruction> conditionalinstructions;

        private List<Instruction> brinstructions;

        private List<Instruction> realbrinstructions;

    }
}
