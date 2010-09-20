using System;
using System.Collections.Generic;

namespace InterfaceInvariants.Sample.SimpleKeyValue
{
	public class DictionarySimpleKeyValue : ISimpleKeyValue {
		private Dictionary<string, object> dictionary = new Dictionary<string, object>();
		
		public void Insert(string key, object val){
			if(val == null) throw new ArgumentNullException();
			dictionary.Add(key, val);
		}
		public object GetByKey(string key) {
			return dictionary[key];
		}
		public void Remove(string key) {
			dictionary.Remove(key);
		}
		public int Count { 
			get { return dictionary.Count; } 
		}
	}
}

