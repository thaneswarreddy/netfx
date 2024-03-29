﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Reflection;
using System.Linq.Expressions;

namespace System.Reflection.UnitTests
{
	[TestFixture]
	public class ReflectorFixture
	{
		[ExpectedException(typeof(ArgumentNullException))]
		[Test]
		public void ShouldThrowIfNullMethodLambda()
		{
			Reflect<Mock>.GetMethod((Expression<Action<Mock>>)null);
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[Test]
		public void ShouldThrowIfNullPropertyLambda()
		{
			Reflect<Mock>.GetProperty((Expression<Func<Mock, object>>)null);
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[Test]
		public void ShouldThrowIfNullFieldLambda()
		{
			Reflect<Mock>.GetField((Expression<Func<Mock, object>>)null);
		}

		[ExpectedException(typeof(ArgumentException))]
		[Test]
		public void ShouldThrowIfNotMethodLambda()
		{
			Reflect<Mock>.GetMethod(x => new object());
		}

		[ExpectedException(typeof(ArgumentException))]
		[Test]
		public void ShouldThrowIfNotPropertyLambda()
		{
			Reflect<Mock>.GetProperty(x => x.PublicField);
		}

		[ExpectedException(typeof(ArgumentException))]
		[Test]
		public void ShouldThrowIfNotFieldLambda()
		{
			Reflect<Mock>.GetField(x => x.PublicProperty);
		}

		[Test]
		public void ShouldGetPublicProperty()
		{
			PropertyInfo info = Reflect<Mock>.GetProperty(x => x.PublicProperty);
			Assert.That(info == typeof(Mock).GetProperty("PublicProperty"));
		}

		[Test]
		public void ShouldGetPublicField()
		{
			FieldInfo info = Reflect<Mock>.GetField(x => x.PublicField);
			Assert.That(info == typeof(Mock).GetField("PublicField"));
		}

		[Test]
		public void ShouldGetPublicVoidMethod()
		{
			MethodInfo info = Reflect<Mock>.GetMethod(x => x.PublicVoidMethod());
			Assert.That(info == typeof(Mock).GetMethod("PublicVoidMethod"));
		}

		[Test]
		public void ShouldGetPublicMethodParameterless()
		{
			MethodInfo info = Reflect<Mock>.GetMethod(x => x.PublicMethodNoParameters());
			Assert.That(info == typeof(Mock).GetMethod("PublicMethodNoParameters"));
		}

		[Test]
		public void ShouldGetPublicMethodParameters()
		{
			MethodInfo info = Reflect<Mock>.GetMethod<string, int>(
				(x, y, z) => x.PublicMethodParameters(y, z));
			Assert.That(info == typeof(Mock).GetMethod("PublicMethodParameters", new Type[] { typeof(string), typeof(int) }));
		}

		[Test]
		public void ShouldGetNonPublicProperty()
		{
			PropertyInfo info = Reflect<ReflectorFixture>.GetProperty(x => x.NonPublicProperty);
			Assert.That(info == typeof(ReflectorFixture).GetProperty("NonPublicProperty", BindingFlags.Instance | BindingFlags.NonPublic));
		}

		[Test]
		public void ShouldGetNonPublicField()
		{
			FieldInfo info = Reflect<ReflectorFixture>.GetField(x => x.NonPublicField);
			Assert.That(info == typeof(ReflectorFixture).GetField("NonPublicField", BindingFlags.Instance | BindingFlags.NonPublic));
		}

		[Test]
		public void ShouldGetNonPublicMethod()
		{
			MethodInfo info = Reflect<ReflectorFixture>.GetMethod(x => x.NonPublicMethod());
			Assert.That(info == typeof(ReflectorFixture).GetMethod("NonPublicMethod", BindingFlags.Instance | BindingFlags.NonPublic));
		}

		[Test]
		public void ShouldPublicStaticMethod()
		{
			MethodInfo info = Reflect.GetMethod(() => Mock.StaticMethod());
			Assert.That(info == typeof(Mock).GetMethod("StaticMethod", BindingFlags.Static | BindingFlags.Public));
		}

		[Test]
		public void ShouldPublicStaticReturnMethod()
		{
			MethodInfo info = Reflect.GetMethod(() => Mock.StaticReturnMethod());
			Assert.That(info == typeof(Mock).GetMethod("StaticReturnMethod", BindingFlags.Static | BindingFlags.Public));
		}

		[Test]
		public void ShouldPublicStaticProperty()
		{
			PropertyInfo info = Reflect.GetProperty(() => Mock.StaticProperty);
			Assert.That(info == typeof(Mock).GetProperty("StaticProperty", BindingFlags.Static | BindingFlags.Public));
		}

		[Test]
		public void ShouldPublicStaticField()
		{
			FieldInfo info = Reflect.GetField(() => Mock.StaticField);
			Assert.That(info == typeof(Mock).GetField("StaticField", BindingFlags.Static | BindingFlags.Public));
		}

		[Test]
		public void ShouldGetPublicConstructor()
		{
			ConstructorInfo info = Reflect.GetConstructor(() => new Mock());
			Assert.That(info == typeof(Mock).GetConstructor(Type.EmptyTypes));
		}

		private int NonPublicField;

		private int NonPublicProperty
		{
			get { return NonPublicField; }
			set { NonPublicField = value; }
		}

		private object NonPublicMethod()
		{
			throw new NotImplementedException();
		}

		public class Mock
		{
			public static void StaticMethod() {}
			public static string StaticReturnMethod() { return ""; }
			public static int StaticProperty { get; set; }
			public static int StaticField;

			public int Value;
			public bool PublicField;
			private int valueProp;

			public Mock()
			{
			}

			public Mock(string foo, int bar)
			{
			}

			public int PublicProperty
			{
				get { return valueProp; }
				set { valueProp = value; }
			}

			public bool PublicMethodNoParameters()
			{
				throw new NotImplementedException();
			}

			public bool PublicMethodParameters(string foo, int bar)
			{
				throw new NotImplementedException();
			}

			public void PublicVoidMethod()
			{
				throw new NotImplementedException();
			}
		}
	}
}
