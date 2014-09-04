using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
#if WINDOWS
[assembly: AssemblyTitle("WaveEngine.Analytics")]
#elif OUYA
[assembly: AssemblyTitle("WaveEngineOUYA.Analytics")]
#elif ANDROID
[assembly: AssemblyTitle("WaveEngineAndroid.Analytics")]
#elif IOS
[assembly: AssemblyTitle("WaveEngineiOS.Analytics")]
#elif METRO
[assembly: AssemblyTitle("WaveEngineMetro.Analytics")]
#elif WINDOWS_PHONE
[assembly: AssemblyTitle("WaveEngineWP.Analytics")]
#elif MAC
[assembly: AssemblyTitle("WaveEngineMac.Analytics")]
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
[assembly: Guid("63b34c51-621b-44a2-a9a2-f15da07281a9")]

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
[assembly: AssemblyVersion("1.4.0.0")]
