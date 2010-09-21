using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using grensesnitt.Framework;
using NUnit.Core;
using NUnit.Core.Builders;
using NUnit.Core.Extensibility;
using NUnit.Framework;

namespace grensesnitt.AddIn
{
    public class InterfaceSuiteBuilder : TestSuite
    {
        public InterfaceSuiteBuilder(Type fixtureType)
            : base(fixtureType)
        {
            IEnumerable<Type> types = GetInterfaceTypes(fixtureType);
            foreach (Type t in types)
            {                
                IEnumerable<Type> matchingTypes = TypeFinder.GetTypesMatchingInterface(t.GetGenericArguments()[0]);
				
		        NUnit.Core.TestSuite current = null;
				foreach (Type toTest in matchingTypes) {
	   	             current = new NUnit.Core.TestSuite(this.TestName.Name,toTest.Name);
    				    foreach (
                        MethodInfo method in
                            fixtureType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                    {
		                    if (!method.IsConstructor)
                        {
                            if (method.GetCustomAttributes(typeof (TestCaseAttribute), true).Count() > 0)
                            {
                                current.Add(BuildParameterizedMethodSuite(method, this, toTest, fixtureType));
                            }
                            else
                            {
                                current.Add(new InstanceTestMethod(method, toTest, fixtureType));
                            }
                        }
					}
					Add(current);
                }
            
			}
        }

        public static Test BuildParameterizedMethodSuite(MethodInfo method, Test parentSuite, Type targetType,
                                                         Type fixtureType)
        {
            var methodSuite = new ParameterizedMethodSuite(method);
            NUnitFramework.ApplyCommonAttributes(method, methodSuite);
            object meh = CoreExtensions.Host.GetType().GetProperty("TestCaseProviders",
                                                                   BindingFlags.Instance | BindingFlags.NonPublic).
                GetValue
                (CoreExtensions.Host, null);

            foreach (object source in meh.Call<IEnumerable>("GetTestCasesFor", method, parentSuite))
                //CoreExtensions.Host.TestCaseProviders.GetTestCasesFor(method, parentSuite))
            {
                ParameterSet parms;

                if (source == null)
                {
                    parms = new ParameterSet();
                    parms.Arguments = new object[] {null};
                }
                else
                    parms = source as ParameterSet;

                if (parms == null)
                {
                    if (source.GetType().GetInterface("NUnit.Framework.ITestCaseData") != null)
                        parms = ParameterSet.FromDataSource(source);
                    else
                    {
                        parms = new ParameterSet();

                        ParameterInfo[] parameters = method.GetParameters();
                        Type sourceType = source.GetType();

                        if (parameters.Length == 1 && parameters[0].ParameterType.IsAssignableFrom(sourceType))
                            parms.Arguments = new[] {source};
                        else if (source is object[])
                            parms.Arguments = (object[]) source;
                        else if (source is Array)
                        {
                            var array = (Array) source;
                            if (array.Rank == 1)
                            {
                                parms.Arguments = new object[array.Length];
                                for (int i = 0; i < array.Length; i++)
                                    parms.Arguments[i] = array.GetValue(i);
                            }
                        }
                        else
                            parms.Arguments = new[] {source};
                    }
                }

                //TestMethod test = new InstanceTestMethod(method, targetType, fixtureType, parms);
                TestMethod test = BuildSingleTestMethod(new InstanceTestMethod(method, targetType, fixtureType),
                                                        method, parentSuite, parms, targetType);

                methodSuite.Add(test);
            }

            return methodSuite;
        }

        public static TestMethod BuildSingleTestMethod(TestMethod testMethod, MethodInfo method, Test parentSuite,
                                                       ParameterSet parms, Type targetType)
        {

            string prefix = TestNameBuilder.GetFullTestName(method, targetType);

            if (CheckTestMethodSignatureAndDoSomePrettyFuckedUpStuffIfItHappensToMatchSomehow(testMethod, parms))
            {
                if (parms == null)
                    NUnitFramework.ApplyCommonAttributes(method, testMethod);
                NUnitFramework.ApplyExpectedExceptionAttribute(method, testMethod);
            }

            if (parms != null)
            {
                // NOTE: After the call to CheckTestMethodSignature, the Method
                // property of testMethod may no longer be the same as the
                // original MethodInfo, so we reassign it here.
                method = testMethod.Method;

                if (parms.TestName != null)
                {
                    testMethod.TestName.Name = parms.TestName;
                    testMethod.TestName.FullName = prefix + "." + parms.TestName;
                }
                else if (parms.OriginalArguments != null)
                {
                    string name = prefix + GetNameFor(parms.OriginalArguments);
                    testMethod.TestName.Name = name;
                    testMethod.TestName.FullName = name;
                }

                if (parms.Ignored)
                {
                    testMethod.RunState = RunState.Ignored;
                    testMethod.IgnoreReason = parms.IgnoreReason;
                }

                //if (parms.ExpectedExceptionName != null)
                //    testMethod.exceptionProcessor = new ExpectedExceptionProcessor(testMethod, parms);

                foreach (string key in parms.Properties.Keys)
                    testMethod.Properties[key] = parms.Properties[key];

                // Description is stored in parms.Properties
                if (parms.Description != null)
                    testMethod.Description = parms.Description;
            }

            if (testMethod.BuilderException != null)
                testMethod.RunState = RunState.NotRunnable;

            return testMethod;
        }
		
		private static string GetNameFor(object []ps) {
			string ret = "(";
			for(int i=0;i<ps.Length;i++) {
				ret += ps[i].ToString();
			    if(i<ps.Length - 1) ret += ", ";
			}
			ret += ")";
			return ret;
		}


        private static bool CheckTestMethodSignatureAndDoSomePrettyFuckedUpStuffIfItHappensToMatchSomehow(
            TestMethod testMethod, ParameterSet parms)
        {
            if (testMethod.Method.IsAbstract)
            {
                testMethod.RunState = RunState.NotRunnable;
                testMethod.IgnoreReason = "Method is abstract";
                return false;
            }

            if (!testMethod.Method.IsPublic)
            {
                testMethod.RunState = RunState.NotRunnable;
                testMethod.IgnoreReason = "Method is not public";
                return false;
            }

            ParameterInfo[] parameters = testMethod.Method.GetParameters();
            int argsNeeded = parameters.Length;

            object[] arglist = null;
            int argsProvided = 0;

            if (parms != null)
            {
                testMethod.SetProperty("arguments", parms.Arguments);
                testMethod.SetProperty("expectedResult", parms.Result);
                testMethod.SetProperty("hasExpectedResult", parms.HasExpectedResult);
                testMethod.RunState = parms.RunState;
                testMethod.IgnoreReason = parms.NotRunReason;
                testMethod.BuilderException = parms.ProviderException;

                arglist = parms.Arguments;

                if (arglist != null)
                    argsProvided = arglist.Length;

                if (testMethod.RunState != RunState.Runnable)
                    return false;
            }

            if (!testMethod.Method.ReturnType.Equals(typeof (void)) &&
                (parms == null || !parms.HasExpectedResult && parms.ExpectedExceptionName == null))
            {
                testMethod.RunState = RunState.NotRunnable;
                testMethod.IgnoreReason = "Method has non-void return value";
                return false;
            }

            if (argsProvided > 0 && argsNeeded == 0)
            {
                testMethod.RunState = RunState.NotRunnable;
                testMethod.IgnoreReason = "Arguments provided for method not taking any";
                return false;
            }

            if (argsProvided == 0 && argsNeeded > 0)
            {
                testMethod.RunState = RunState.NotRunnable;
                testMethod.IgnoreReason = "No arguments were provided";
                return false;
            }

            //if (argsProvided > argsNeeded)
            //{
            //    ParameterInfo lastParameter = parameters[argsNeeded - 1];
            //    Type lastParameterType = lastParameter.ParameterType;

            //    if (lastParameterType.IsArray && lastParameter.IsDefined(typeof(ParamArrayAttribute), false))
            //    {
            //        object[] newArglist = new object[argsNeeded];
            //        for (int i = 0; i < argsNeeded; i++)
            //            newArglist[i] = arglist[i];

            //        int length = argsProvided - argsNeeded + 1;
            //        Array array = Array.CreateInstance(lastParameterType.GetElementType(), length);
            //        for (int i = 0; i < length; i++)
            //            array.SetValue(arglist[argsNeeded + i - 1], i);

            //        newArglist[argsNeeded - 1] = array;
            //        testMethod.arguments = arglist = newArglist;
            //        argsProvided = argsNeeded;
            //    }
            //}

            if (argsProvided != argsNeeded)
            {
                testMethod.RunState = RunState.NotRunnable;
                testMethod.IgnoreReason = "Wrong number of arguments provided";
                return false;
            }

#if NET_2_0
            if (testMethod.Method.IsGenericMethodDefinition)
            {
                Type[] typeArguments = GetTypeArgumentsForMethod(testMethod.Method, arglist);
                foreach (object o in typeArguments)
                    if (o == null)
                    {
                        testMethod.RunState = RunState.NotRunnable;
                        testMethod.IgnoreReason = "Unable to determine type arguments for fixture";
                        return false;
                    }

                testMethod.method = testMethod.Method.MakeGenericMethod(typeArguments);
                parameters = testMethod.Method.GetParameters();

                for (int i = 0; i < parameters.Length; i++)
                {
                    if (arglist[i].GetType() != parameters[i].ParameterType && arglist[i] is IConvertible)
                    {
                        try
                        {
                            arglist[i] = Convert.ChangeType(arglist[i], parameters[i].ParameterType);
                        }
                        catch (Exception)
                        {
                            // Do nothing - the incompatible argument will be reported below
                        }
                    }
                }
            }
#endif

            return true;
        }

        private IEnumerable<Type> GetInterfaceTypes(Type t)
        {
            Type[] ifaces = t.GetInterfaces();
            foreach (Type cur in ifaces)
            {
                if (cur.IsGenericType && cur.GetGenericTypeDefinition() == typeof (AppliesTo<>))
                {
                    yield return cur;
                }
            }
        }

    }

    internal static class AssemblyExtension
    {

        public static object CreateInstance(this Assembly self, string typeName, params object[] args)
        {
            try
            {
                return Activator.CreateInstance(self.GetType(typeName, true), args, null);
            }
            catch (Exception e)
            {
                throw e.InnerException;
            }
        }

    }

    internal static class ObjectExtension
    {

        public static void Call(this object self, string methodName, params object[] args)
        {
            try
            {
                MethodInfo method = self.GetType().GetMethod(methodName,
                                                             BindingFlags.Instance | BindingFlags.Public |
                                                             BindingFlags.NonPublic, null,
                                                             args.Select(obj => obj.GetType()).ToArray(), null);
                method.Invoke(self, args);
            }
            catch (Exception e)
            {
                throw e.InnerException;
            }
        }

        public static T Call<T>(this object self, string methodName, params object[] args)
        {
            try
            {
                MethodInfo method = self.GetType().GetMethod(methodName,
                                                             BindingFlags.Instance | BindingFlags.Public |
                                                             BindingFlags.NonPublic, null,
                                                             args.Select(obj => obj.GetType()).ToArray(), null);
                return (T) method.Invoke(self, args);
            }
            catch (Exception e)
            {
                throw e.InnerException;
            }
        }

        public static T CallStatic<T>(Type type, string methodName, params object[] args)
        {
            try
            {
                MethodInfo method = type.GetMethod(methodName,
                                                   BindingFlags.Public |
                                                   BindingFlags.NonPublic | BindingFlags.Static, null,
                                                   args.Select(obj => obj.GetType()).ToArray(), null);
                return (T) method.Invoke(null, args);
            }
            catch (Exception e)
            {
                throw e.InnerException;
            }
        }

        public static void SetProperty(this object self, string propertyName, object value)
        {
            try
            {
                BindingFlags bindingFlags = BindingFlags.Instance |
                                            BindingFlags.Public |
                                            BindingFlags.NonPublic;
                var property = self.GetType().GetProperty(propertyName, bindingFlags);
                if (property != null)
                {
                    property.SetValue(self, value, null);
                }
                else
                {
                    var field = self.GetType().GetField(propertyName, bindingFlags);
                    if (field != null)
                    {
                        field.SetValue(self, value);
                    }
                    else
                    {
                        throw new ArgumentException("Field or property " + propertyName + " not found");
                    }
                }
            }
            catch (Exception e)
            {
                throw e.InnerException;
            }
        }
    }
}