using System;
using System.Collections.Generic;
using grensesnitt.Framework;
using NUnit.Framework;

namespace grensesnitt.Sample.SimpleKeyValue
{
	[InterfaceSpecification]
	public class SimpleKeyValueTests : AppliesToAll<ISimpleKeyValue> {
		public SimpleKeyValueTests() {

		}
		
		public void WhenAnItemIsInsertedCountIncreases() {
			subject.Insert("Test", "foo");
			Assert.AreEqual(1, subject.Count);
		}
		
		public void CanGetInsertedItem() {
			subject.Insert("Test", "foo");
			Assert.AreEqual("foo", subject.GetByKey("Test"));
		}
		
		public void GetOfNonExistentItemThrowsKeyNotFoundException() {
			Assert.Throws<KeyNotFoundException>(() => subject.GetByKey("not here"));
		}
		
		public void InsertThrowsArgumentNullExceptionWhenValueIsNull() {
			Assert.Throws<ArgumentNullException>(() => subject.Insert("test", null));
		}
		
		public void DoubleInsertThrowsArgumentException() {
			subject.Insert("greg", 1);
			Assert.Throws<ArgumentException>(() => subject.Insert("greg", 2));
		}
		
		public void RemoveLowersCount() {
			subject.Insert("1", "1");
			subject.Insert("2", "2");
			subject.Remove("2");
			Assert.AreEqual(1, subject.Count);
		}
	}
}

