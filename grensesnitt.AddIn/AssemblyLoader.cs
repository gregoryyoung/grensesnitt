using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

namespace grensesnitt.AddIn
{
	public class AssemblyLoader
	{
		private static IEnumerable<string> GetFiles(IEnumerable<string> paths) {
			List<string> files = new List<string>();
			foreach(string path in paths){
				try {
				files.AddRange(Directory.GetFiles(path, "*.dll"));
				files.AddRange(Directory.GetFiles(path, "*.exe"));
				}
				catch {}
			}
			return files;
		}
		public static void LoadAssembliesInPaths(IEnumerable<string> paths) {
			foreach(string file in GetFiles(paths)) {
				Assembly.LoadFrom(file);
			}
		}
	}
}

