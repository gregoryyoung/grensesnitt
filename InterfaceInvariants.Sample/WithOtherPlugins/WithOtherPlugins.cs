using System;
using NUnit.Framework;
using InterfaceInvariants.Framework;
namespace InterfaceInvariants.Sample
{
	
	public interface ICanAdd {
		int Add(int i, int j); //dont ask me why you want different adders
	}
	
	public class Adder1 : ICanAdd {
		public int Add(int i, int j) {
			return i + j;
		}
	}
	
	public class Adder2 : ICanAdd {
		public int Add(int i, int j) {
			return (i + 12) + (j - 12); //yeeeeeaaaah
		}
	}
	
/*	[InterfaceSpecification]
	public class WithOtherPlugins : AppliesToAll<ICanAdd>
	{
		[TestCase(1,2, Result=3)]
		[TestCase(-1,2, Result=1)]
		[TestCase(0,0, Result=0)]
		public int CanAddOrSomething(int x, int y) {
			return subject.Add(x, y);
		}
	}
	not working yet gosh darn it! MUST HAVE CHAINING!
	*/
}

