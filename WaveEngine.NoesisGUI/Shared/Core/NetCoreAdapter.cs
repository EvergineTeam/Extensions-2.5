// <auto-generated />

#if NETFX_CORE

namespace System.Runtime.InteropServices
{
    using System;

    [System.Runtime.InteropServices.ComVisible(true)]
    public struct HandleRef
    {
        // ! Do not add or rearrange fields as the EE depends on this layout.
        //------------------------------------------------------------------
        internal Object _wrapper;
        internal IntPtr _handle;
        //------------------------------------------------------------------

        public HandleRef(Object wrapper, IntPtr handle)
        {
            _wrapper = wrapper;
            _handle = handle;
        }

        public Object Wrapper
        {
            get
            {
                return _wrapper;
            }
        }

        public IntPtr Handle
        {
            get
            {
                return _handle;
            }
        }
    }
}

namespace System.ComponentModel
{
    // There is no [TypeConverter] attribute in WinRT
    [AttributeUsage(AttributeTargets.All)]
    public sealed class TypeConverterAttribute: Attribute
    {
        private string converter_type;

        public TypeConverterAttribute()
        {
            converter_type = "";
        }

        public TypeConverterAttribute(string typeName)
        {
            converter_type = typeName;
        }

        public TypeConverterAttribute(Type type)
        {
            converter_type = type.AssemblyQualifiedName;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TypeConverterAttribute))
                return false;
            return ((TypeConverterAttribute) obj).ConverterTypeName == converter_type;
        }

        public override int GetHashCode()
        {
            return converter_type.GetHashCode ();
        }
        
        public string ConverterTypeName
        {
            get { return converter_type; }
        }
    }
}

namespace System.Windows.Markup
{
    // There is no [ContentProperty] attribute in WinRT
    [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ContentPropertyAttribute: Attribute
    {
        public ContentPropertyAttribute() {}
        
        public ContentPropertyAttribute(string name)
        {
            Name = name;
        }
        
        public string Name { get; private set; }
    }
}

namespace System.Security
{
    // There is no [SuppressUnmanagedCodeSecurity] attribute in WinRT
    [AttributeUsageAttribute(AttributeTargets.Class|AttributeTargets.Method|AttributeTargets.Interface|
        AttributeTargets.Delegate, AllowMultiple = true, Inherited = false)]
    public sealed class SuppressUnmanagedCodeSecurityAttribute: Attribute {}
}

namespace System
{
    // SWIG uses (incorrectly) System.SystemException instead of System.Exception and there is no
    // SystemException class in WinRT
    public class SystemException: Exception
    {
        public SystemException(string message, Exception innerException) : base(message, innerException) { }
    }

    // Unity already defines this in WinRTLegacy assembly
    #if !UNITY_WSA_10_0 && !UNITY_WSA_8_1
        // SWIG uses System.ApplicationException and there is no such class in .NETCore
        public class ApplicationException : Exception
        {
            public ApplicationException() { }
            public ApplicationException(string message) : base(message) { }
            public ApplicationException(string message, Exception innerException) : base(message, innerException) { }
        }
    #endif
}

namespace Noesis
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Windows.Storage;
    using System.IO;

    sealed class AppDomain
    {
        public static AppDomain CurrentDomain { get; private set; }

        static AppDomain()
        {
            CurrentDomain = new AppDomain();
        }

        public Assembly[] GetAssemblies()
        {
            return GetAssemblyList().ToArray();
        }

        private IEnumerable<Assembly> GetAssemblyList()
        {
            // [WAVEENGINE] Changes in this method were made to avoid hanging on UWP when Noesis loads, be careful when updating!
            var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var assemblies = new List<Assembly>();

            var directoryFiles = Directory.GetFiles(folder.Path);

            foreach (var file in directoryFiles)
            {
                var extension = System.IO.Path.GetExtension(file);
                if (extension == ".dll" || extension == ".exe")
                {
                    try
                    {
                        // Loading UnityPlayer.dll causes an internal error in the .NET Runtime
                        var assemblyName = System.IO.Path.GetFileNameWithoutExtension(file);
                        if (assemblyName != "UnityPlayer" && assemblyName != "BridgeInterface")
                        {
                            var assemblyRef = new AssemblyName() { Name = assemblyName };
                            assemblies.Add(Assembly.Load(assemblyRef));
                        }
                    }
                    catch { }
                }
            }

            // Add extra assemblies that are part of the system
            AddAssemblyFromType(typeof(ObservableCollection<bool>), assemblies);
            return assemblies;
        }

        private void AddAssemblyFromType(Type type, List<Assembly> assemblies)
        {
            Assembly assembly = type.GetTypeInfo().Assembly;
            if (assemblies.IndexOf(assembly) == -1)
            {
                assemblies.Add(assembly);
            }
        }
    }

    public static class ExtensionMethods
    {
        public static Type[] GetGenericArguments(this Type type)
        {
            return type.GetTypeInfo().GenericTypeArguments;
        }

        public static MethodInfo GetGetMethod(this PropertyInfo property)
        {
            return property.GetMethod;
        }

        public static MethodInfo GetSetMethod(this PropertyInfo property)
        {
            return property.SetMethod;
        }
    }
}

#else

namespace System.Windows.Markup
{
    /// <summary>Indicates which property of a type is the XAML content property. A XAML processor
    /// uses this information when processing XAML child elements of XAML representations of the
    /// attributed type.</summary>
    [AttributeUsageAttribute(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ContentPropertyAttribute : Attribute
    {
        private string _name;

        /// <summary>Gets the name of the property that is the content property.</summary>
        /// <returns>The name of the property that is the content property.</returns>
        public string Name { get{ return this._name; }
        }

        /// <summary>Initializes a new instance of the ContentPropertyAttribute class.</summary>
        public ContentPropertyAttribute() { }

        /// <summary>Initializes a new instance of the ContentPropertyAttribute class, by using the
        /// specified name.</summary>
        /// <param name="name">The property name for the property that is the content
        /// property.</param>
        public ContentPropertyAttribute(string name) { this._name = name; }
    }
}

namespace Noesis
{
    using System;
    using System.Reflection;
    using System.Runtime;
    using System.Collections.Generic;

    public static class ExtensionMethods
    {
        // In .NET 4.5 Type and TypeInfo are different classes. This extension method adapts the
        // version of Mono used in Unity to that API. That way we can mantain the same code without
        // using preprocessor
        public static Type GetTypeInfo(this Type type)
        {
            return type;
        }

        public static T GetCustomAttribute<T>(this MemberInfo member) where T: Attribute
        {
            return (T)Attribute.GetCustomAttribute(member, typeof(T));
        }

        public static Delegate CreateDelegate(this MethodInfo method, Type type)
        {
            return Delegate.CreateDelegate(type, method);
        }
    }

    public static class Marshal
    {
        public static void StructureToPtr(object structure, IntPtr ptr, bool fDeleteOld)
        {
            System.Runtime.InteropServices.Marshal.StructureToPtr(structure, ptr, fDeleteOld);
        }

        public static T PtrToStructure<T>(IntPtr ptr)
        {
            return (T)System.Runtime.InteropServices.Marshal.PtrToStructure(ptr, typeof(T));
        }

        public static IntPtr StringToHGlobalAnsi(string s)
        {
            return System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(s);
        }

        public static string PtrToStringAnsi(IntPtr ptr)
        {
            return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(ptr);
        }

        public static IntPtr StringToHGlobalUni(string s)
        {
            return System.Runtime.InteropServices.Marshal.StringToHGlobalUni(s);
        }

        public static int SizeOf<T>()
        {
            return System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));
        }

        public static IntPtr AllocHGlobal(int numBytes)
        {
            return System.Runtime.InteropServices.Marshal.AllocHGlobal(numBytes);
        }

        public static void FreeHGlobal(IntPtr hglobal)
        {
            System.Runtime.InteropServices.Marshal.FreeHGlobal(hglobal);
        }

        public static void Copy(IntPtr source, byte[] destination, int startIndex, int length)
        {
            System.Runtime.InteropServices.Marshal.Copy(source, destination, startIndex, length);
        }

        public static byte ReadByte(IntPtr ptr, int offset)
        {
            return System.Runtime.InteropServices.Marshal.ReadByte(ptr, offset);
        }

        public static void WriteInt32(IntPtr ptr, int ofs, int val)
        {
            System.Runtime.InteropServices.Marshal.WriteInt32(ptr, ofs, val);
        }

        public static void WriteInt64(IntPtr ptr, int ofs, long val)
        {
            System.Runtime.InteropServices.Marshal.WriteInt64(ptr, ofs, val);
        }

        public static Delegate GetDelegateForFunctionPointer(IntPtr ptr, Type t)
        {
            return System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer(ptr, t);
        }

        public static int GetLastWin32Error()
        {
            return System.Runtime.InteropServices.Marshal.GetLastWin32Error();
        }
    }
}

#endif
