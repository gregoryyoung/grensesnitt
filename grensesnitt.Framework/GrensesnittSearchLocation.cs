using System;
namespace grensesnitt.Framework
{
	public class GrensesnittSearchLocationAttribute : Attribute
    {
        public string Path { get; set; }
		
		public GrensesnittSearchLocationAttribute(string path) {
			Path = path;
		}
    }
}

