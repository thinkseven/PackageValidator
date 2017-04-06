using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PackageValidator
{
    class Program
    {
        public static void ResolveDependency(AssemblyName _givenAssemblyName, Assembly _parentAssembly, ref List<Exception> _errors)
        {
            try
            {
                Assembly _assembly = Assembly.Load(_givenAssemblyName);
                AssemblyName[] _assemblyNames = _assembly.GetReferencedAssemblies();
                foreach (AssemblyName _assemblyName in _assemblyNames)
                {
                    if (AppDomain.CurrentDomain.GetAssemblies().Where<Assembly>(x => x.FullName.Equals(_assemblyName.FullName)).SingleOrDefault() == null)
                    {
                        ResolveDependency(_assemblyName, _assembly, ref _errors);
                    }
                }
            }
            catch (Exception ex)
            {
                _errors.Add(new Exception(String.Format(@"Failed to load assembly ""{0}"" referenced by ""{1}"" due to error of {2}", _givenAssemblyName, _parentAssembly.FullName, ex.ToString())));
            }
        }

        static void Main(string[] args)
        {
            if(args != null && args.Length == 3)
            {
                // -output:""; args[0]
                // -package:""; args[1]
                // -enrty:""; args[2]
                String output = String.Empty;
                if(!String.IsNullOrEmpty(args[0]) && args[0].Length>0)
                {
                    output = args[0].Substring("-output:".Length);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error!! Invalid parameters. Please run the utility in given format and parameters: ");
                    Console.WriteLine("");
                    Console.Write("\t");
                    Console.WriteLine(@"PackageValidator.exe -output:""<OUTPUT_PATH>"" -package:""<PACKAGE_PATH>"" -entry:""<Entry DLL>""");
                    Console.ResetColor();
                }

                String package = String.Empty;
                if (!String.IsNullOrEmpty(args[1]) && args[1].Length > 0)
                {
                    package = args[1].Substring("-package:".Length);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error!! Invalid parameters. Please run the utility in given format and parameters: ");
                    Console.WriteLine("");
                    Console.Write("\t");
                    Console.WriteLine(@"PackageValidator.exe -output:""<OUTPUT_PATH>"" -package:""<PACKAGE_PATH>"" -entry:""<Entry DLL>""");
                    Console.ResetColor();
                }


                String entry = String.Empty;
                if (!String.IsNullOrEmpty(args[2]) && args[2].Length > 0)
                {
                    entry = args[2].Substring("-entry:".Length);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error!! Invalid parameters. Please run the utility in given format and parameters: ");
                    Console.WriteLine("");
                    Console.Write("\t");
                    Console.WriteLine(@"PackageValidator.exe -output:""<OUTPUT_PATH>"" -package:""<PACKAGE_PATH>"" -entry:""<Entry DLL>""");
                    Console.ResetColor();
                }


                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(@"Validating package ""{0}"" for entry dll ""{1}""...", package, entry);
                Console.ResetColor();

                // Shell AppDomain Setup
                //AppDomainSetup _shellDomainSetup = new AppDomainSetup();
                //_shellDomainSetup.ApplicationBase = package;
                //AppDomain _shellDomain = AppDomain.CreateDomain("ShellDomain", null, _shellDomainSetup);
                
                List<Exception> _errors = new List<Exception>();
                try
                {
                    Assembly _entryAssembly = Assembly.LoadFile(String.Format("{0}\\{1}", package, entry));
                    AssemblyName[] _assemblyNames = _entryAssembly.GetReferencedAssemblies();
                    foreach (AssemblyName _assemblyName in _assemblyNames)
                    {
                        if (AppDomain.CurrentDomain.GetAssemblies().Where<Assembly>(x => x.FullName.Equals(_assemblyName.FullName)).SingleOrDefault() == null)
                        {
                            ResolveDependency(_assemblyName, _entryAssembly, ref _errors);
                        }
                    }
                    //_shellDomain.Load(_entryAssembly.FullName);
                }
                catch (Exception ex)
                {
                    _errors.Add(ex);
                }

                if(_errors.Count > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("\t");
                    Console.WriteLine("----------------------------------------------------------------");
                    Console.Write("\t");
                    Console.WriteLine("Failed to resolve package dependency for below dlls");
                    Console.WriteLine("");
                    foreach (Exception _ex in _errors)
                    {
                        Console.Write("\t");
                        Console.WriteLine(_ex.ToString());
                    }
                    Console.ResetColor();
                }

                //Assembly[] _binAssemblies = _shellDomain.GetAssemblies().Where<Assembly>(x => x.Location.ToLower().Contains("bin")).ToArray<Assembly>();
                Assembly[] _binAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where<Assembly>(x => x.Location.ToLower().Contains("bin")).ToArray<Assembly>();

                if (_binAssemblies.Length > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("\t");
                    Console.WriteLine("----------------------------------------------------------------");
                    Console.Write("\t");
                    Console.WriteLine("Successfully resolve package dependency from local bin");
                    Console.WriteLine("");
                    foreach (Assembly _assembly in _binAssemblies)
                    {
                        Console.Write("\t");
                        Console.WriteLine(_assembly.FullName);
                        Console.Write("\t\t");
                        Console.WriteLine(_assembly.Location);
                    }
                    Console.ResetColor();
                }

                //Assembly[] _systemAssemblies = _shellDomain.GetAssemblies().Where<Assembly>(x => x.Location.ToLower().Contains("gac") || x.Location.ToLower().Contains("windows")).ToArray<Assembly>();
                Assembly[] _systemAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where<Assembly>(x => x.Location.ToLower().Contains("gac") || x.Location.ToLower().Contains("windows")).ToArray<Assembly>();
                if (_systemAssemblies.Length>0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write("\t");
                    Console.WriteLine("----------------------------------------------------------------");
                    Console.Write("\t");
                    Console.WriteLine("Successfully resolve package dependency from GAC/Default .NET Setup");
                    Console.WriteLine("");
                    foreach (Assembly _assembly in _systemAssemblies)
                    {
                        Console.Write("\t");
                        Console.WriteLine(_assembly.FullName);
                        Console.Write("\t\t");
                        Console.WriteLine(_assembly.Location);
                    }
                    Console.ResetColor();
                }

                //AppDomain.Unload(_shellDomain);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error!! Invalid parameters. Please run the utility in given format and parameters: ");
                Console.WriteLine("");
                Console.Write("\t");
                Console.WriteLine(@"PackageValidator.exe -output:""<OUTPUT_PATH>"" -package:""<PACKAGE_PATH>"" -entry:""<Entry DLL>""");
                Console.ResetColor();
            }
            Console.ReadLine();
        }
    }
}
