using System;
using NUnit.Core;
using System.Reflection;
using System.Linq.Expressions;
using grensesnitt.Framework;

namespace grensesnitt.AddIn
{
	public class InstanceTestMethod : TestMethod
	{
		Func<object> factory;
		
		Type fixtureType;
		public void SetTarget(object fixtureInstance, object target) {
			var arguments = new object [] { target };
			fixtureType.InvokeMember ("SetSubjectUnderTest", 
                         BindingFlags.Default | BindingFlags.InvokeMethod,
                         null,
                         fixtureInstance,
                         arguments);
		}
		
		public override object Fixture {
			get {
				var fixture = Reflect.Construct(fixtureType);
				var Target = factory();
				SetTarget(fixture, Target);
            	return fixture;
			}
			set {
				
			}
		}
		
		public InstanceTestMethod(MethodInfo method, Type targetType, Type fixtureType) : base(method)
		{
			this.fixtureType = fixtureType;
			this.factory = GrensesnittObjectLocator.GetHandler(targetType);
			TestName.Name = TestNameBuilder.GetTestName(method, targetType);
			TestName.FullName = TestNameBuilder.GetFullTestName(method, targetType);
		}
	}
}

