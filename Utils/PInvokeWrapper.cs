using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

public static class PInvokeWrapper
{
    [StructLayout(LayoutKind.Sequential)]
    private struct KeyValue
    {
        public IntPtr Key;
        public IntPtr Values;
        public int ValueCount;
    }

    [DllImport("GerberToolsWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void processPCBFiles(KeyValue[] data, int dataSize, out IntPtr frontSvg, out IntPtr backSvg);

    [DllImport("GerberToolsWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void generateMTLAndOBJFiles(KeyValue[] data, int dataSize, out IntPtr mtl, out IntPtr obj);

    public static (string frontSvg, string backSvg) CallProcessPCBFiles(Dictionary<string, List<string>> files)
    {
        var data = ConvertDictionaryToKeyValueArray(files);
        IntPtr frontSvgPtr = IntPtr.Zero;
        IntPtr backSvgPtr = IntPtr.Zero;
        string frontSvg = string.Empty;
        string backSvg = string.Empty;

        try
        {
            processPCBFiles(data, data.Length, out frontSvgPtr, out backSvgPtr);

            frontSvg = Marshal.PtrToStringAnsi(frontSvgPtr);
            backSvg = Marshal.PtrToStringAnsi(backSvgPtr);

            return (frontSvg, backSvg);
        }
        finally
        {
            Cleanup(data, frontSvgPtr, backSvgPtr);
        }
    }

    public static (string mtl, string obj) CallGenerateMTLAndOBJFiles(Dictionary<string, List<string>> files)
    {
        var data = ConvertDictionaryToKeyValueArray(files);
        IntPtr mtlPtr = IntPtr.Zero;
        IntPtr objPtr = IntPtr.Zero;
        string mtl = string.Empty;
        string obj = string.Empty;

        try
        {
            generateMTLAndOBJFiles(data, data.Length, out mtlPtr, out objPtr);

            mtl = Marshal.PtrToStringAnsi(mtlPtr);
            obj = Marshal.PtrToStringAnsi(objPtr);

            return (mtl, obj);
        }
        finally
        {
            Cleanup(data, mtlPtr, objPtr);
            mtl = string.Empty;
            obj = string.Empty;
        }
    }

    private static KeyValue[] ConvertDictionaryToKeyValueArray(Dictionary<string, List<string>> files)
    {
        var data = new KeyValue[files.Count];
        int index = 0;

        foreach (var kvp in files)
        {
            var keyPtr = Marshal.StringToHGlobalAnsi(kvp.Key);
            var values = kvp.Value;
            var valuesPtrs = new IntPtr[values.Count];

            for (int i = 0; i < values.Count; i++)
            {
                valuesPtrs[i] = Marshal.StringToHGlobalAnsi(values[i]);
            }

            var valuesPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)) * values.Count);
            Marshal.Copy(valuesPtrs, 0, valuesPtr, values.Count);

            data[index] = new KeyValue
            {
                Key = keyPtr,
                Values = valuesPtr,
                ValueCount = values.Count
            };

            index++;
        }

        return data;
    }

    private static void Cleanup(KeyValue[] data, params IntPtr[] pointers)
    {
        foreach (var kvp in data)
        {
            Marshal.FreeHGlobal(kvp.Key);
            var values = new IntPtr[kvp.ValueCount];
            Marshal.Copy(kvp.Values, values, 0, kvp.ValueCount);
            foreach (var value in values)
            {
                Marshal.FreeHGlobal(value);
            }
            Marshal.FreeHGlobal(kvp.Values);
        }

        foreach (var ptr in pointers)
        {
            if (ptr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(ptr);
            }
        }
    }
}