using System;
using NUnit.Core;
using NUnit.Core.Extensibility;

namespace grensesnitt.AddIn
{
	[NUnitAddin(Name = "Grensesnitt Addin", Description = "Tests defined for interfaces run accross all classes that meet that interface")]
    public class GrensesnittAddin : IAddin, ISuiteBuilder
    {
        public bool Install(IExtensionHost host)
        {
            IExtensionPoint builders = host.GetExtensionPoint("SuiteBuilders");
            if (builders == null)
                return false;
            builders.Install(this);
            return true;
        }

        public bool CanBuildFrom(Type type)
        {
			bool canBuild = Reflect.HasAttribute(type, "grensesnitt.Framework.InterfaceSpecificationAttribute", false);
			
            return canBuild;
        }

        public Test BuildFrom(Type type)
		{
			AssemblyLoader.LoadAssembliesInPaths(SearchPaths.GetSearchPaths(type.Assembly));
			return new InterfaceSuiteBuilder(type);
		}
    }
}

