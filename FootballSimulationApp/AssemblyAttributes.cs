using System;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace FootballSimulationApp
{
    internal class AssemblyAttributes
    {
        public readonly string Company;
        public readonly string Copyright;
        public readonly string Description;
        public readonly string Product;
        public readonly string Title;
        public readonly string Version;

        public AssemblyAttributes(Assembly assembly)
        {
            Contract.Requires<ArgumentNullException>(assembly != null);

            Title = GetAttribute<AssemblyTitleAttribute>(assembly, a => a.Title);
            Version = assembly.GetName().Version.ToString();
            Description = GetAttribute<AssemblyDescriptionAttribute>(assembly, a => a.Description);
            Product = GetAttribute<AssemblyProductAttribute>(assembly, a => a.Product);
            Copyright = GetAttribute<AssemblyCopyrightAttribute>(assembly, a => a.Copyright);
            Company = GetAttribute<AssemblyCompanyAttribute>(assembly, a => a.Company);
        }

        private static string GetAttribute<T>(Assembly assembly, Func<T, string> func) where T : Attribute
        {
            var attributes = Attribute.GetCustomAttributes(assembly, typeof(T));
            return attributes.Length == 0 ? string.Empty : func.Invoke((T)attributes[0]);
        }
    }
}