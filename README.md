# How to elegantly handle c#'s CreateInstance() with default parameter constructor

## The Problem

Sometimes, it is useful to be able to create an object from an arbitrary Type variable using this built-in c# function:

```c#
Activator.CreateInstance(classType);
```

This can be quite useful in the case where you want to instantiate a type from a string during runtime.  (My particular use-case is for creating a inherited sub type like this:)

```c#
SuperType newlyCreatedObject = (SuperType) Activator.CreateInstance(subType);
```

However, this only works with types that have parameterless constructors.  So a class like this will work:

```c#
public class WorkingTest
{
	public WorkingTest()
	{
		// Do stuff.
	}
}
```

But a class like this would fail:

```c#
public class BrokenTest
{
	public BrokenTest(string parameter = "Default Parameter")
	{
		// Do stuff.
	}
}
```

You will probably get a `No parameterless constructor defined for type` even though you have defaults defined.

This is somewhat vexing since it seems like CreateInstance should be smart enough to fill in the default parameter automatically.  (At compiletime, I don't know what parameters a given subtype will need, though the class itself knows).  This only becomes more messy the more default parameters your class has.

*With that in mind, what strategies can you use to work around this limitation?*

## Bad Workaround - Duplicate first parameter

You can create a new default parameterless constructor that calls the existing default constructor:

```c#
public class WorkingTest
{
	public WorkingTest() : this("Default Parameter") // Only the first parameter has to be duplicated
	{ }

	public WorkingTest(string parameter = "Default Parameter", int secondParameter = 0)
	{
		// Do stuff.
	}
}
```

Downsides:  
1) Requires changes to every instantiable class
2) Duplicates the first parameter in a second location

## Good workaround?

Use a class like `Instantiator.cs` (attached) to intelligently call either the parameterless or default parameter constructor.

You may notice this function also helps ensure you're creating the type you intend to (just in case someone passes you a weird type to init that you don't intend to.)

Then, simply do this in your calling code:

```c#
SuperType newlyCreatedObject = Instantiator.Instantiate<SuperType>(subType);
```

It will work regardless of how many default parameters are required.

---

This was non-obvious to me, so hope this helps! :)
