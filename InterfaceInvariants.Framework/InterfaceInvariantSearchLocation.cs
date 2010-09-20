using System;
namespace InterfaceInvariants.Framework
{
	public class InterfaceInvariantSearchLocationAttribute : Attribute
    {
        public string Path { get; set; }
		
		public InterfaceInvariantSearchLocationAttribute(string path) {
			Path = path;
		}
    }
}

