﻿namespace NOpenCL
{
    using System;
    using System.Collections.Generic;

    public sealed class Program : IDisposable
    {
        private readonly Context _context;
        private readonly ProgramSafeHandle _handle;
        private bool _disposed;

        internal Program(Context context, ProgramSafeHandle handle)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (handle == null)
                throw new ArgumentNullException("handle");

            _context = context;
            _handle = handle;
        }

        public uint ReferenceCount
        {
            get
            {
                return UnsafeNativeMethods.GetProgramInfo(Handle, UnsafeNativeMethods.ProgramInfo.ReferenceCount);
            }
        }

        public Context Context
        {
            get
            {
                ThrowIfDisposed();
                return _context;
            }
        }

        public uint NumDevices
        {
            get
            {
                return UnsafeNativeMethods.GetProgramInfo(Handle, UnsafeNativeMethods.ProgramInfo.NumDevices);
            }
        }

        public IReadOnlyList<Device> Devices
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Source
        {
            get
            {
                return UnsafeNativeMethods.GetProgramInfo(Handle, UnsafeNativeMethods.ProgramInfo.Source);
            }
        }

        public IReadOnlyList<UIntPtr> BinarySizes
        {
            get
            {
                return UnsafeNativeMethods.GetProgramInfo(Handle, UnsafeNativeMethods.ProgramInfo.BinarySizes);
            }
        }

        public IReadOnlyList<IntPtr> Binaries
        {
            get
            {
                return UnsafeNativeMethods.GetProgramInfo(Handle, UnsafeNativeMethods.ProgramInfo.Binaries);
            }
        }

        public IntPtr NumKernels
        {
            get
            {
                return UnsafeNativeMethods.GetProgramInfo(Handle, UnsafeNativeMethods.ProgramInfo.NumKernels);
            }
        }

        public IReadOnlyList<string> KernelNames
        {
            get
            {
                return UnsafeNativeMethods.GetProgramInfo(Handle, UnsafeNativeMethods.ProgramInfo.KernelNames).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        internal ProgramSafeHandle Handle
        {
            get
            {
                ThrowIfDisposed();
                return _handle;
            }
        }

        public BuildStatus GetBuildStatus(Device device)
        {
            if (device == null)
                throw new ArgumentNullException("device");

            return (BuildStatus)UnsafeNativeMethods.GetProgramBuildInfo(Handle, device.ID, UnsafeNativeMethods.ProgramBuildInfo.BuildStatus);
        }

        public string GetBuildOptions(Device device)
        {
            if (device == null)
                throw new ArgumentNullException("device");

            return UnsafeNativeMethods.GetProgramBuildInfo(Handle, device.ID, UnsafeNativeMethods.ProgramBuildInfo.BuildOptions);
        }

        public string GetBuildLog(Device device)
        {
            if (device == null)
                throw new ArgumentNullException("device");

            return UnsafeNativeMethods.GetProgramBuildInfo(Handle, device.ID, UnsafeNativeMethods.ProgramBuildInfo.BuildLog);
        }

        public BinaryType GetBinaryType(Device device)
        {
            if (device == null)
                throw new ArgumentNullException("device");

            return (BinaryType)UnsafeNativeMethods.GetProgramBuildInfo(Handle, device.ID, UnsafeNativeMethods.ProgramBuildInfo.BinaryType);
        }

        public void Build(string options)
        {
            UnsafeNativeMethods.BuildProgram(Handle, null, options, null, IntPtr.Zero);
        }

        public void Build(Device[] devices, string options)
        {
            UnsafeNativeMethods.ClDeviceID[] deviceIDs = null;
            if (devices != null)
                deviceIDs = Array.ConvertAll(devices, device => device.ID);

            UnsafeNativeMethods.BuildProgram(Handle, deviceIDs, options, null, IntPtr.Zero);
        }

        public void Dispose()
        {
            _handle.Dispose();
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }
    }
}
