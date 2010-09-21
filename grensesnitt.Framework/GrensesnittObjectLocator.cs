using System;
using System.Collections.Generic;

namespace grensesnitt.Framework
{
	public static class GrensesnittObjectLocator
	{
		private static Func<Type, object> handleAll;
		
		private static Dictionary<Type, Func<object>> map = new Dictionary<Type, Func<object>>();
		
		public static object GetInstance(Type t) {
			return GetHandler(t)();
		}
		
		public static void HookAllTo(Func<Type, object> handler) {
			handleAll = handler;
		}
		
		public static void Register<T>(Func<object> handler) where T:class {
			map.Add(typeof(T), handler);
		}
		
		public static Func<object> GetHandler(Type T) {
			Func<object> handler;
			if(handleAll != null) return () => {return handleAll(T); };
			if(map.TryGetValue(T, out handler)) {
				return (Func<object>) handler;
			}
			return () => { return TryReflections(T);} ;
		}		
		
		public static object TryReflections(Type t) {
			return Activator.CreateInstance(t);
		}
	}
}