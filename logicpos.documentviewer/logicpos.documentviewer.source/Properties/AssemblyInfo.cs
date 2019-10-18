using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Version information for an assembly consists of the following four values:
//      Major Version
//      Minor Version - Improvments
//      Build Number - BugFixes
//      Revision -.Net framework

//---------------------------- WinForms Viewer -------------------------------------------------
[assembly: AssemblyInformationalVersion("4.12.2704")]  //Should be equal to the same property of Patagames.Pdf assembly
[assembly: AssemblyVersion("4.4.2." +
#if DOTNET20
"20"
#elif DOTNET30
"30"
#elif DOTNET35
"35"
#elif DOTNET40
"40"
#elif DOTNET45
"45"
#elif DOTNET451
"451"
#elif DOTNET452
"452"
#elif DOTNET46
"46"
#elif DOTNET461
"461"
#elif DOTNET462
"462"
#elif DOTNET47
"47"
#elif DOTNET471
"471"
#elif DOTNET472
"472"
#elif DOTNET48
"48"
#else
"0"
#endif
)]

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
#if DOTNET20
[assembly: AssemblyTitle("Patagames Pdf.Net SDK - WinForms controls (.net 2.0)")]
#elif DOTNET30
[assembly: AssemblyTitle("Patagames Pdf.Net SDK - WinForms controls (.net 3.0)")]
#elif DOTNET35
[assembly: AssemblyTitle("Patagames Pdf.Net SDK - WinForms controls (.net 3.5)")]
#elif DOTNET40
[assembly: AssemblyTitle("Patagames Pdf.Net SDK - WinForms controls (.net 4.0)")]
#elif DOTNET45
[assembly: AssemblyTitle("Patagames Pdf.Net SDK - WinForms controls (.net 4.5)")]
#elif DOTNET451
[assembly: AssemblyTitle("Patagames Pdf.Net SDK - WinForms controls (.net 4.5.1)")]
#elif DOTNET452
[assembly: AssemblyTitle("Patagames Pdf.Net SDK - WinForms controls (.net 4.5.2)")]
#elif DOTNET46
[assembly: AssemblyTitle("Patagames Pdf.Net SDK - WinForms controls (.net 4.6)")]
#elif DOTNET461
[assembly: AssemblyTitle("Patagames Pdf.Net SDK - WinForms controls (.net 4.6.1)")]
#elif DOTNET462
[assembly: AssemblyTitle("Patagames Pdf.Net SDK - WinForms controls (.net 4.6.2)")]
#elif DOTNET47
[assembly: AssemblyTitle("Patagames Pdf.Net SDK - WinForms controls (.net 4.7)")]
#elif DOTNET471
[assembly: AssemblyTitle("Patagames Pdf.Net SDK - WinForms controls (.net 4.7.1)")]
#elif DOTNET472
[assembly: AssemblyTitle("Patagames Pdf.Net SDK - WinForms controls (.net 4.7.2)")]
#elif DOTNET48
[assembly: AssemblyTitle("Patagames Pdf.Net SDK - WinForms controls (.net 4.8)")]
#else
[assembly: AssemblyTitle("Patagames Pdf.Net SDK - WinForms controls")]
#endif
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Patagames Software")]
[assembly: AssemblyProduct("Patagames Pdf.Net SDK")]
[assembly: AssemblyCopyright("Copyright ©  2019")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("3ee5b54a-d560-4066-8882-939b9cbf611d")]
