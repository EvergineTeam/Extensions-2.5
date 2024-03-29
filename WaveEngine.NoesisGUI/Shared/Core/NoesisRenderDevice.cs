﻿// <auto-generated />
using System;
using System.Runtime.InteropServices;

namespace Noesis
{
    /// <summary>
    /// Abstraction of a graphics rendering device.
    /// </summary>
    public class RenderDevice
    {
        /// <summary>
        /// Width of offscreen textures (0 = automatic). Default is automatic.
        /// </summary>
        public uint OffscreenWidth
        {
            get { return Noesis_RenderDevice_GetOffscreenWidth(CPtr); }
            set { Noesis_RenderDevice_SetOffscreenWidth(CPtr, value); }
        }

        /// <summary>
        /// Height of offscreen textures (0 = automatic). Default is automatic.
        /// </summary>
        public uint OffscreenHeight
        {
            get { return Noesis_RenderDevice_GetOffscreenHeight(CPtr); }
            set { Noesis_RenderDevice_SetOffscreenHeight(CPtr, value); }
        }

        /// <summary>
        /// Multisampling of offscreen textures. Default is 1x.
        /// </summary>
        public uint OffscreenSampleCount
        {
            get { return Noesis_RenderDevice_GetOffscreenSampleCount(CPtr); }
            set { Noesis_RenderDevice_SetOffscreenSampleCount(CPtr, value); }
        }

        /// <summary>
        /// Number of offscreen textures created at startup. Default is 0.
        /// </summary>
        public uint OffscreenDefaultNumSurfaces
        {
            get { return Noesis_RenderDevice_GetOffscreenDefaultNumSurfaces(CPtr); }
            set { Noesis_RenderDevice_SetOffscreenDefaultNumSurfaces(CPtr, value); }
        }

        /// <summary>
        /// Maximum number of offscreen textures (0 = unlimited). Default is unlimited.
        /// </summary>
        public uint OffscreenMaxNumSurfaces
        {
            get { return Noesis_RenderDevice_GetOffscreenMaxNumSurfaces(CPtr); }
            set { Noesis_RenderDevice_SetOffscreenMaxNumSurfaces(CPtr, value); }
        }

        /// <summary>
        /// Width of texture used to cache glyphs. Default is 1024.
        /// </summary>
        public uint GlyphCacheWidth
        {
            get { return Noesis_RenderDevice_GetGlyphCacheWidth(CPtr); }
            set { Noesis_RenderDevice_SetGlyphCacheWidth(CPtr, value); }
        }

        /// <summary>
        /// Height of texture used to cache glyphs. Default is 1024.
        /// </summary>
        public uint GlyphCacheHeight
        {
            get { return Noesis_RenderDevice_GetGlyphCacheHeight(CPtr); }
            set { Noesis_RenderDevice_SetGlyphCacheHeight(CPtr, value); }
        }

        /// <summary>
        /// Glyphs with size above this are rendered using triangle meshes. Default is 96.
        /// </summary>
        public uint GlyphCacheMeshThreshold
        {
            get { return Noesis_RenderDevice_GetGlyphCacheMeshThreshold(CPtr); }
            set { Noesis_RenderDevice_SetGlyphCacheMeshThreshold(CPtr, value); }
        }

        #region Private members
        public RenderDevice(IntPtr cPtr, bool memoryOwn)
        {
            _renderDevice = new BaseComponent(cPtr, memoryOwn);
        }

        internal HandleRef CPtr { get { return BaseComponent.getCPtr(_renderDevice); } }

        private BaseComponent _renderDevice;
        #endregion

        #region Imports
        [DllImport(Library.Name)]
        static extern uint Noesis_RenderDevice_GetOffscreenWidth(HandleRef device);

        [DllImport(Library.Name)]
        static extern void Noesis_RenderDevice_SetOffscreenWidth(HandleRef device, uint w);

        [DllImport(Library.Name)]
        static extern uint Noesis_RenderDevice_GetOffscreenHeight(HandleRef device);

        [DllImport(Library.Name)]
        static extern void Noesis_RenderDevice_SetOffscreenHeight(HandleRef device, uint h);

        [DllImport(Library.Name)]
        static extern uint Noesis_RenderDevice_GetOffscreenSampleCount(HandleRef device);

        [DllImport(Library.Name)]
        static extern void Noesis_RenderDevice_SetOffscreenSampleCount(HandleRef device, uint c);

        [DllImport(Library.Name)]
        static extern uint Noesis_RenderDevice_GetOffscreenDefaultNumSurfaces(HandleRef device);

        [DllImport(Library.Name)]
        static extern void Noesis_RenderDevice_SetOffscreenDefaultNumSurfaces(HandleRef device,
            uint n);

        [DllImport(Library.Name)]
        static extern uint Noesis_RenderDevice_GetOffscreenMaxNumSurfaces(HandleRef device);

        [DllImport(Library.Name)]
        static extern void Noesis_RenderDevice_SetOffscreenMaxNumSurfaces(HandleRef device, uint n);

        [DllImport(Library.Name)]
        static extern uint Noesis_RenderDevice_GetGlyphCacheWidth(HandleRef device);

        [DllImport(Library.Name)]
        static extern void Noesis_RenderDevice_SetGlyphCacheWidth(HandleRef device, uint w);

        [DllImport(Library.Name)]
        static extern uint Noesis_RenderDevice_GetGlyphCacheHeight(HandleRef device);

        [DllImport(Library.Name)]
        static extern void Noesis_RenderDevice_SetGlyphCacheHeight(HandleRef device, uint w);

        [DllImport(Library.Name)]
        static extern uint Noesis_RenderDevice_GetGlyphCacheMeshThreshold(HandleRef device);

        [DllImport(Library.Name)]
        static extern void Noesis_RenderDevice_SetGlyphCacheMeshThreshold(HandleRef device, uint t);
        #endregion
    }

    /// <summary>
    ///  Creates an OpenGL RenderDevice.
    /// </summary>
    public class RenderDeviceGL : RenderDevice
    {
        public RenderDeviceGL() :
            base(Noesis_RenderDevice_CreateGL_(), true)
        {
        }

        #region Imports
        private static IntPtr Noesis_RenderDevice_CreateGL_()
        {
            IntPtr cPtr = Noesis_RenderDevice_CreateGL();
            Error.Check();
            return cPtr;
        }

        [DllImport(Library.Name)]
        static extern IntPtr Noesis_RenderDevice_CreateGL();
        #endregion
    }

    /// <summary>
    ///  Creates a D3D11 RenderDevice.
    /// </summary>
    public class RenderDeviceD3D11 : RenderDevice
    {
        public RenderDeviceD3D11(IntPtr deviceContext) : this(deviceContext, false)
        {
        }

        public RenderDeviceD3D11(IntPtr deviceContext, bool sRGB) :
            base(Noesis_RenderDevice_CreateD3D11_(deviceContext, sRGB), true)
        {
        }

        #region Imports
        private static IntPtr Noesis_RenderDevice_CreateD3D11_(IntPtr deviceContext, bool sRGB)
        {
            IntPtr cPtr = Noesis_RenderDevice_CreateD3D11(deviceContext, sRGB);
            Error.Check();
            return cPtr;
        }

        [DllImport(Library.Name)]
        static extern IntPtr Noesis_RenderDevice_CreateD3D11(IntPtr deviceContext, bool sRGB);
        #endregion
    }
}
