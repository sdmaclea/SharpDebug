﻿using CsScriptManaged;
using DbgEngManaged;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CsScripts
{
    public class Variable : DynamicObject
    {
        public const string ComputedName = "<computed>";
        private DEBUG_TYPED_DATA typedData;
        private string name;

        internal Variable(string name, _DEBUG_SYMBOL_ENTRY entry)
        {
            this.name = name;
            if (entry.Offset > 0)
            {
                typedData = Context.Advanced.Request(DebugRequest.ExtTypedDataAnsi, new EXT_TYPED_DATA()
                {
                    Operation = ExtTdop.SetFromTypeIdAndU64,
                    InData = new DEBUG_TYPED_DATA()
                    {
                        ModBase = entry.ModuleBase,
                        Offset = entry.Offset,
                        TypeId = entry.TypeId,
                    },
                }).OutData;
            }
            else
            {
                typedData.Size = entry.Size;
                typedData.ModBase = entry.ModuleBase;
                typedData.Offset = entry.Offset;
                typedData.TypeId = entry.TypeId;
                typedData.Tag = (SymTag)entry.Tag;
            }
        }

        private static DEBUG_TYPED_DATA GetTypedData(ulong moduleId, uint typeId, ulong offset)
        {
            return Context.Advanced.Request(DebugRequest.ExtTypedDataAnsi, new EXT_TYPED_DATA()
            {
                Operation = ExtTdop.SetFromTypeIdAndU64,
                InData = new DEBUG_TYPED_DATA()
                {
                    ModBase = moduleId,
                    Offset = offset,
                    TypeId = typeId,
                },
            }).OutData;
        }

        internal Variable(DEBUG_TYPED_DATA typedData, string name = ComputedName)
        {
            this.name = name;
            this.typedData = typedData;
        }

        public static explicit operator byte (Variable v)
        {
            return (byte)(short)v;
        }

        public static explicit operator short (Variable v)
        {
            return (short)(int)v;
        }

        public static explicit operator int (Variable v)
        {
            return (int)(long)v;
        }

        public static explicit operator long (Variable v)
        {
            // TODO: Check if it is base type and if we can read v.typedData.Data
            uint read;
            uint size = v.typedData.Size;
            IntPtr pointer = Marshal.AllocHGlobal((int)size);

            try
            {
                Context.Symbols.ReadTypedDataVirtual(v.typedData.Offset, v.typedData.ModBase, v.typedData.TypeId, pointer, size, out read);

                switch (size)
                {
                    case 1:
                        return Marshal.ReadByte(pointer);
                    case 2:
                        return Marshal.ReadInt16(pointer);
                    case 4:
                        return Marshal.ReadInt32(pointer);
                    case 8:
                        return Marshal.ReadInt64(pointer);
                    default:
                        throw new Exception("Unexpected variable size");
                }
            }
            finally
            {
                Marshal.FreeHGlobal(pointer);
            }
        }

        public override string ToString()
        {
            var type = GetCodeType();
            return "";
        }

        public string GetName()
        {
            return name;
        }

        public DType GetCodeType()
        {
            return new DType(typedData);
        }

        public string GetRuntimeType()
        {
            // TODO: See if it is complex type and try to get VTable
            return "";
        }

        public string[] GetFieldNames()
        {
            List<string> fields = new List<string>();
            uint nameSize;

            try
            {
                for (uint fieldIndex = 0; ; fieldIndex++)
                {
                    StringBuilder sb = new StringBuilder(Constants.MaxSymbolName);

                    Context.Symbols.GetFieldName(typedData.ModBase, typedData.TypeId, fieldIndex, sb, (uint)sb.Capacity, out nameSize);
                    fields.Add(sb.ToString());
                }
            }
            catch (Exception)
            {
            }

            return fields.ToArray();
        }

        public Variable AdjustPointer(int offset)
        {
            if (typedData.Tag != SymTag.PointerType)
            {
                throw new ArgumentException("Variable is not a pointer type, but " + typedData.Tag);
            }

            DEBUG_TYPED_DATA newTypedData = typedData;

            newTypedData.Data += (ulong)offset;
            return new Variable(newTypedData);
        }

        public Variable CastAs(DType type)
        {
            return new Variable(GetTypedData(type.ModuleId, type.TypeId, typedData.Offset));
        }

        public Variable CastAs(string newType)
        {
            uint newTypeId = Context.Symbols.GetTypeIdWide(typedData.ModBase, newType);
            int moduleIndex = newType.IndexOf('!');
            ulong moduleId = typedData.ModBase;

            if (moduleIndex > 0)
            {
                string moduleName = newType.Substring(moduleIndex);
                uint index;

                Context.Symbols.GetModuleByModuleName(moduleName, 0, out index, out moduleId);
            }

            return new Variable(GetTypedData(moduleId, newTypeId, typedData.Offset));
        }

        public Variable[] GetFields()
        {
            string[] fieldNames = GetFieldNames();
            Variable[] fields = new Variable[fieldNames.Length];

            for (int i = 0; i < fieldNames.Length; i++)
            {
                fields[i] = GetField(fieldNames[i]);
            }

            return fields;
        }

        public Variable GetField(string name)
        {
            var response = Context.Advanced.RequestExtended(DebugRequest.ExtTypedDataAnsi, new EXT_TYPED_DATA()
            {
                Operation = ExtTdop.GetField,
                InData = typedData,
                InStrIndex = (uint)Marshal.SizeOf<EXT_TYPED_DATA>(),
            }, name);

            return new Variable(response.OutData, name);
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            // TODO: Implement
            return base.TryConvert(binder, out result);
        }

        public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result)
        {
            // TODO: Implement
            return base.TryBinaryOperation(binder, arg, out result);
        }

        public override bool TryUnaryOperation(UnaryOperationBinder binder, out object result)
        {
            // TODO: Implement
            return base.TryUnaryOperation(binder, out result);
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return GetFieldNames();
        }

        private bool TryGetMember(string name, out object result)
        {
            try
            {
                result = GetField(name);
                return true;
            }
            catch (Exception)
            {
                result = null;
                return false;
            }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return TryGetMember(binder.Name, out result);
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if (indexes.Length > 1)
            {
                throw new ArgumentException("Multidimensional arrays are not supported");
            }

            try
            {
                int index = Convert.ToInt32(indexes[0]);

                if (typedData.Tag == SymTag.PointerType || typedData.Tag == SymTag.ArrayType)
                {
                    var response = Context.Advanced.Request(DebugRequest.ExtTypedDataAnsi, new EXT_TYPED_DATA()
                    {
                        Operation = ExtTdop.GetArrayElement,
                        InData = typedData,
                        In64 = (ulong)index,
                    }).OutData;

                    result = new Variable(response);
                    return true;
                }
            }
            catch (Exception)
            {
                // Index is not a number, fall back to getting member
            }

            return TryGetMember(indexes[0].ToString(), out result);
        }

        #region Not allowed setters/deleters
        public override bool TryDeleteMember(DeleteMemberBinder binder)
        {
            throw new UnauthorizedAccessException();
        }

        public override bool TryDeleteIndex(DeleteIndexBinder binder, object[] indexes)
        {
            throw new UnauthorizedAccessException();
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            throw new UnauthorizedAccessException();
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            throw new UnauthorizedAccessException();
        }
        #endregion
    }
}