﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using RealType = System.Single;

namespace Components
{
    class RealTool
    {
        [DllImport("kernel32.dll")]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, int count);
    }

    [Serializable]
    public struct Real : IComparable<Real>
    {
        public RealType Value;

        public static Int32 Size
        {
            get { return sizeof(RealType); }
        }

        private Real(double value)
        {
            this.Value = (RealType)value;
        }

        public static implicit operator Real(double value)
        {
            return new Real(value);
        }

        public static implicit operator RealType(Real real)
        {
            return real.Value;
        }

        public int CompareTo(Real other)
        {
            return this.Value.CompareTo(other.Value);
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }

        public static Real[] GetArray(Array data)
        {
            Type arrayType = data.GetType().GetElementType();
            Real[] resultData = new Real[data.Length];

            //型の不一致をここで吸収
            if (arrayType != typeof(RealType) && arrayType != typeof(Real))
            {
                //一次元の長さの配列を用意
                Array array = Array.CreateInstance(arrayType, data.Length);
                //一次元化して
                Buffer.BlockCopy(data, 0, array, 0, Marshal.SizeOf(arrayType) * resultData.Length);

                data = new RealType[array.Length];

                //型変換しつつコピー
                Array.Copy(array, data, array.Length);
            }

            //データを叩き込む
            int size = sizeof(RealType) * data.Length;
            GCHandle gchObj = GCHandle.Alloc(data, GCHandleType.Pinned);
            GCHandle gchBytes = GCHandle.Alloc(resultData, GCHandleType.Pinned);
            RealTool.CopyMemory(gchBytes.AddrOfPinnedObject(), gchObj.AddrOfPinnedObject(), size);
            gchObj.Free();
            gchBytes.Free();

            return resultData;
        }
    }
}
