using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DisplayMagicianShared.NVIDIA
{



    internal class NativeArrayHelper
    {
        public static T GetArrayItemData<T>(IntPtr sourcePointer)
        {
            return (T)Marshal.PtrToStructure(sourcePointer, typeof(T));
        }

        public static T[] GetArrayData<T>(IntPtr sourcePointer, int itemCount)
        {
            var lstResult = new List<T>();
            if (sourcePointer != IntPtr.Zero && itemCount > 0)
            {
                var sizeOfItem = Marshal.SizeOf(typeof(T));
                for (int i = 0; i < itemCount; i++)
                {
                    lstResult.Add(GetArrayItemData<T>(sourcePointer + (sizeOfItem * i)));
                }
            }
            return lstResult.ToArray();
        }

        public static void SetArrayData<T>(T[] items, out IntPtr targetPointer)
        {
            if (items != null && items.Length > 0)
            {
                var sizeOfItem = Marshal.SizeOf(typeof(T));
                targetPointer = Marshal.AllocHGlobal(sizeOfItem * items.Length);
                for (int i = 0; i < items.Length; i++)
                {
                    Marshal.StructureToPtr(items[i], targetPointer + (sizeOfItem * i), true);
                }
            }
            else
            {
                targetPointer = IntPtr.Zero;
            }

        }

        public static void SetArrayItemData<T>(T item, out IntPtr targetPointer)
        {
            var sizeOfItem = Marshal.SizeOf(typeof(T));
            targetPointer = Marshal.AllocHGlobal(sizeOfItem);
            Marshal.StructureToPtr(item, targetPointer, true);
        }

    }


    /// <summary>
    ///     Interface for all pointer based handles
    /// </summary>
    public interface IHandle
    {
        /// <summary>
        ///     Returns true if the handle is null and not pointing to a valid location in the memory
        /// </summary>
        bool IsNull { get; }

        /// <summary>
        ///     Gets the address of the handle in the memory
        /// </summary>
        IntPtr MemoryAddress { get; }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ValueTypeReference : IDisposable, IHandle, IEquatable<ValueTypeReference>
    {
        // ReSharper disable once ConvertToAutoProperty
        public IntPtr MemoryAddress { get; }

        public static ValueTypeReference Null
        {
            get => new ValueTypeReference();
        }

        public bool IsNull
        {
            get => MemoryAddress == IntPtr.Zero;
        }

        public ValueTypeReference(IntPtr memoryAddress)
        {
            MemoryAddress = memoryAddress;
        }

        public static ValueTypeReference FromValueType<T>(T valueType) where T : struct
        {
            return FromValueType(valueType, typeof(T));
        }

        public static ValueTypeReference FromValueType(object valueType, Type type)
        {
            if (!type.IsValueType)
            {
                throw new ArgumentException("Only Value Types are acceptable.", nameof(type));
            }

            var memoryAddress = Marshal.AllocHGlobal(Marshal.SizeOf(type));

            if (memoryAddress != IntPtr.Zero)
            {
                var result = new ValueTypeReference(memoryAddress);
                Marshal.StructureToPtr(valueType, memoryAddress, false);

                return result;
            }

            return Null;
        }

        public bool Equals(ValueTypeReference other)
        {
            return MemoryAddress.Equals(other.MemoryAddress);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is ValueTypeReference reference && Equals(reference);
        }

        public override int GetHashCode()
        {
            return MemoryAddress.GetHashCode();
        }

        public static bool operator ==(ValueTypeReference left, ValueTypeReference right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ValueTypeReference left, ValueTypeReference right)
        {
            return !left.Equals(right);
        }

        public T ToValueType<T>(Type type)
        {
            if (MemoryAddress == IntPtr.Zero)
            {
                return default(T);
            }

            if (!type.IsValueType)
            {
                throw new ArgumentException("Only Value Types are acceptable.", nameof(type));
            }

            return (T)Marshal.PtrToStructure(MemoryAddress, type);
        }

        public T? ToValueType<T>() where T : struct
        {
            if (IsNull)
            {
                return null;
            }

            return ToValueType<T>(typeof(T));
        }

        public void Dispose()
        {
            if (!IsNull)
            {
                Marshal.FreeHGlobal(MemoryAddress);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ValueTypeArray : IDisposable, IHandle, IEquatable<ValueTypeArray>
    {
        // ReSharper disable once ConvertToAutoProperty
        public IntPtr MemoryAddress { get; }

        public static ValueTypeArray Null
        {
            get => new ValueTypeArray();
        }

        public bool IsNull
        {
            get => MemoryAddress == IntPtr.Zero;
        }

        public ValueTypeArray(IntPtr memoryAddress)
        {
            MemoryAddress = memoryAddress;
        }

        public static ValueTypeArray FromArray(IEnumerable<object> list)
        {
            var array = list.ToArray();

            if (array.Length > 0)
            {
                if (array[0] == null || !array[0].GetType().IsValueType)
                {
                    throw new ArgumentException("Only Value Types are acceptable.", nameof(list));
                }

                var type = array[0].GetType();

                if (array.Any(item => item.GetType() != type))
                {
                    throw new ArgumentException("Array should not hold objects of multiple types.", nameof(list));
                }

                return FromArray(array, type);
            }

            return Null;
        }


        // ReSharper disable once ExcessiveIndentation
        // ReSharper disable once MethodTooLong
        public static ValueTypeArray FromArray(IEnumerable<object> list, Type type)
        {
            var array = list.ToArray();

            if (array.Length > 0)
            {
                var typeSize = Marshal.SizeOf(type);
                var memoryAddress = Marshal.AllocHGlobal(array.Length * typeSize);

                if (memoryAddress != IntPtr.Zero)
                {
                    var result = new ValueTypeArray(memoryAddress);

                    foreach (var item in array)
                    {
                        if (type == typeof(int))
                        {
                            Marshal.WriteInt32(memoryAddress, (int)item);
                        }
                        else if (type == typeof(uint))
                        {
                            Marshal.WriteInt32(memoryAddress, (int)(uint)item);
                        }
                        else if (type == typeof(short))
                        {
                            Marshal.WriteInt16(memoryAddress, (short)item);
                        }
                        else if (type == typeof(ushort))
                        {
                            Marshal.WriteInt16(memoryAddress, (short)(ushort)item);
                        }
                        else if (type == typeof(long))
                        {
                            Marshal.WriteInt64(memoryAddress, (long)item);
                        }
                        else if (type == typeof(ulong))
                        {
                            Marshal.WriteInt64(memoryAddress, (long)(ulong)item);
                        }
                        else if (type == typeof(byte))
                        {
                            Marshal.WriteByte(memoryAddress, (byte)item);
                        }
                        else if (type == typeof(IntPtr))
                        {
                            Marshal.WriteIntPtr(memoryAddress, (IntPtr)item);
                        }
                        else
                        {
                            Marshal.StructureToPtr(item, memoryAddress, false);
                        }

                        memoryAddress += typeSize;
                    }

                    return result;
                }
            }

            return Null;
        }

        public bool Equals(ValueTypeArray other)
        {
            return MemoryAddress.Equals(other.MemoryAddress);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is ValueTypeArray array && Equals(array);
        }

        public override int GetHashCode()
        {
            return MemoryAddress.GetHashCode();
        }

        public static bool operator ==(ValueTypeArray left, ValueTypeArray right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ValueTypeArray left, ValueTypeArray right)
        {
            return !left.Equals(right);
        }

        public static ValueTypeArray FromArray<T>(T[] array) where T : struct
        {
            return FromArray(array.Cast<object>());
        }

        public T[] ToArray<T>(int count) where T : struct
        {
            return ToArray<T>(count, typeof(T));
        }

        public T[] ToArray<T>(int count, Type type)
        {
            return ToArray<T>(0, count, type);
        }

        public T[] ToArray<T>(int start, int count) where T : struct
        {
            return ToArray<T>(start, count, typeof(T)).ToArray();
        }

        public T[] ToArray<T>(int start, int count, Type type)
        {
            if (IsNull)
            {
                return null;
            }

            return AsEnumerable<T>(start, count, type).ToArray();
        }

        public IEnumerable<T> AsEnumerable<T>(int count) where T : struct
        {
            return AsEnumerable<T>(count, typeof(T));
        }


        public IEnumerable<T> AsEnumerable<T>(int count, Type type)
        {
            return AsEnumerable<T>(0, count, type);
        }

        public IEnumerable<T> AsEnumerable<T>(int start, int count) where T : struct
        {
            return AsEnumerable<T>(start, count, typeof(T));
        }

        // ReSharper disable once ExcessiveIndentation
        // ReSharper disable once MethodTooLong
        public IEnumerable<T> AsEnumerable<T>(int start, int count, Type type)
        {
            if (!IsNull)
            {
                if (!type.IsValueType)
                {
                    throw new ArgumentException("Only Value Types are acceptable.", nameof(type));
                }

                var typeSize = Marshal.SizeOf(type);
                var address = MemoryAddress + start * typeSize;

                for (var i = 0; i < count; i++)
                {
                    if (type == typeof(int))
                    {
                        yield return (T)(object)Marshal.ReadInt32(address);
                    }
                    else if (type == typeof(uint))
                    {
                        yield return (T)(object)(uint)Marshal.ReadInt32(address);
                    }
                    else if (type == typeof(short))
                    {
                        yield return (T)(object)Marshal.ReadInt16(address);
                    }
                    else if (type == typeof(ushort))
                    {
                        yield return (T)(object)(ushort)Marshal.ReadInt16(address);
                    }
                    else if (type == typeof(long))
                    {
                        yield return (T)(object)Marshal.ReadInt64(address);
                    }
                    else if (type == typeof(ulong))
                    {
                        yield return (T)(object)(ulong)Marshal.ReadInt64(address);
                    }
                    else if (type == typeof(byte))
                    {
                        yield return (T)(object)Marshal.ReadByte(address);
                    }
                    else if (type == typeof(IntPtr))
                    {
                        yield return (T)(object)Marshal.ReadIntPtr(address);
                    }
                    else
                    {
                        yield return (T)Marshal.PtrToStructure(address, type);
                    }

                    address += typeSize;
                }
            }
        }


        public void Dispose()
        {
            if (!IsNull)
            {
                Marshal.FreeHGlobal(MemoryAddress);
            }
        }
    }
}
