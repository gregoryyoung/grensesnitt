using System;
using System.Collections.Generic;
namespace InterfaceInvariants.Sample.SimpleKeyValue
{
	public interface shitbird : ISimpleKeyValue {} //supports inheritance etc
	
	public class ListSimpleKeyValue : shitbird
	{
		private SortedList<string, object> list = new SortedList<string, object>();
		
		public void Insert(string key, object val){
            if (val == null) throw new ArgumentNullException("val");
			list.Add(key, val);
		}
		public object GetByKey(string key) {
			return list[key];
		}
		public void Remove(string key) {
			list.Remove(key);
		}
		
		public int Count { 
			get { return list.Count; } 
		}
	}
}