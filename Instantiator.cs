using System;
using System.Reflection;

namespace Skyebold
{
	public static class Instantiator
	{
		/// <summary>
		/// Attempts to instantiate an object from a given type.  This utility function will check that the given type
		/// passed is the same or a subtype of the templated type.  It can handle types that have both parameterless
		/// constructors, as well as default parameter constructors.
		/// </summary>
		/// <param name="desiredType">Desired type to create</param>
		/// <typeparam name="MustBeATypeOrSubTypeOf"></typeparam>
		/// <returns></returns>
		public static MustBeATypeOrSubTypeOf Instantiate<MustBeATypeOrSubTypeOf> (Type desiredType)
		{
			if (desiredType == null)
			{
				throw new ArgumentNullException();
			}
			else if (desiredType.IsSubclassOf(typeof(MustBeATypeOrSubTypeOf)) || desiredType == typeof(MustBeATypeOrSubTypeOf)) // safety check to make sure is a subtype or type
			{
				// Object is of the desired subtype, safe to proceed

				MustBeATypeOrSubTypeOf createdObject = default(MustBeATypeOrSubTypeOf);

				// Does the type have a parameterless constructor that can be used?
				var parameterlessConstructor = desiredType.GetConstructor(Type.EmptyTypes);
				if (parameterlessConstructor != null)
				{
					// Attempt a parameterless construction
					createdObject = (MustBeATypeOrSubTypeOf) Activator.CreateInstance(desiredType);
				}
				else
				{
					// No parameterless constructor, attempt to find a default constructor instead
					// (Based on https://stackoverflow.com/questions/11002523/activator-createinstance-with-optional-parameters )
					createdObject = (MustBeATypeOrSubTypeOf) Activator.CreateInstance(
						desiredType,
						BindingFlags.CreateInstance |
						BindingFlags.Public |
						BindingFlags.Instance |
						BindingFlags.OptionalParamBinding,
						null, new object[] { Type.Missing }, System.Globalization.CultureInfo.CurrentCulture);
				}

				return createdObject;
			} else {
				// Attempted to instantiate an object of an unexpected type (Not a subtype)

				throw new TypeAccessException(desiredType.ToString() + " is not a type or subtype of " + 
					typeof(MustBeATypeOrSubTypeOf).ToString());
			}
		}
	}
}
