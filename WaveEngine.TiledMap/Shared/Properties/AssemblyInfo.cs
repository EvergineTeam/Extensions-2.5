using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
#if WINDOWS
[assembly: AssemblyTitle("WaveEngine.TiledMap")]
#elif OUYA
[assembly: AssemblyTitle("WaveEngineOUYA.TiledMap")]
#elif ANDROID
[assembly: AssemblyTitle("WaveEngineAndroid.TiledMap")]
#elif IOS
[assembly: AssemblyTitle("WaveEngineiOS.TiledMap")]
#elif METRO
[assembly: AssemblyTitle("WaveEngineMetro.TiledMap")]
#elif WINDOWS_PHONE
[assembly: AssemblyTitle("WaveEngineWP.TiledMap")]
#elif MAC
[assembly: AssemblyTitle("WaveEngineMac.TiledMap")]
#endif

[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Wave Corporation")]
[assembly: AssemblyCopyright("Copyright © Wave Corporation 2014")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("a142bf2e-e6db-41ec-b84d-88fe9b566493")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.4.2.0")]
