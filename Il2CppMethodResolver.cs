using System;
using System.IO;
using System.Reflection;
using Iced.Intel;
using UnhollowerBaseLib.Runtime;
using UnhollowerBaseLib.Runtime.VersionSpecific.MethodInfo;

namespace ModernCamera
{

    internal class Il2CppMethodResolver
    {
        private static ulong ExtractTargetAddress(in Instruction instruction)
        {
            return instruction.Op0Kind switch
            {
                OpKind.NearBranch16 => instruction.NearBranch16,
                OpKind.NearBranch32 => instruction.NearBranch32,
                OpKind.NearBranch64 => instruction.NearBranch64,
                OpKind.FarBranch16 => instruction.FarBranch16,
                OpKind.FarBranch32 => instruction.FarBranch32,
                _ => 0,
            };
        }
        private static unsafe IntPtr ResolveMethodPointer(IntPtr methodPointer)
        {
            var stream = new UnmanagedMemoryStream((byte*)methodPointer, 1024, 1024, FileAccess.Read);
            var codeReader = new StreamCodeReader(stream);

            Decoder decoder = Decoder.Create(64, codeReader);
            decoder.IP = (ulong)methodPointer.ToInt64();

            Instruction instr = default;
            while (instr.Mnemonic != Mnemonic.Int3)
            {
                decoder.Decode(out instr);

                if (instr.Mnemonic != Mnemonic.Jmp && instr.Mnemonic != Mnemonic.Add)
                {
                    Plugin.Logger.LogDebug($"Encountered mnemonic {instr.Mnemonic}. Treating as normal method");
                    return methodPointer;
                }

                if (instr.Mnemonic == Mnemonic.Add)
                {
                    if (instr.Immediate32 == 0x10)
                    {
                        Plugin.Logger.LogDebug($"Unboxing {instr.Op0Register}");
                    }
                    else
                    {
                        Plugin.Logger.LogDebug($"Encountered non-unboxing add {instr.Immediate32}. Treating as normal method");
                        return methodPointer;
                    }
                }

                if (instr.Mnemonic == Mnemonic.Jmp)
                {
                    return new IntPtr((long)ExtractTargetAddress(instr));
                }
            }
            return methodPointer;
        }

        public static unsafe IntPtr ResolveFromMethodInfo(INativeMethodInfoStruct methodInfo)
        {
            return ResolveMethodPointer(methodInfo.MethodPointer);
        }

        public static unsafe IntPtr ResolveFromMethodInfo(MethodInfo method)
        {
            var methodInfoField = UnhollowerBaseLib.UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(method);
            if (methodInfoField == null)
            {
                throw new Exception($"Couldn't obtain method info for {method}");
            }

            var il2CppMethod = UnityVersionHandler.Wrap((Il2CppMethodInfo*)(IntPtr)(methodInfoField.GetValue(null) ?? IntPtr.Zero));
            if (il2CppMethod == null)
            {
                throw new Exception($"Method info for {method} is invalid");
            }

            return ResolveFromMethodInfo(il2CppMethod);
        }
    }
}
