#define Test_NestedArrays

using System;
using System.Collections;
using System.Collections.Generic;
#if Test_NestedArrays
using System.Diagnostics;
#endif

namespace FlattenArbitrarilyNestedIntArrays
{
	class MainClass
	{
		private const int maxArraySize = 5;
		private const int maxArrayDepth = 5;

		private static Random random = new Random ();

		#if Test_NestedArrays
		public static void Main (string[] args)
		{
			object nestedArrays = CreateNestedIntArray (maxArrayDepth);
			List<int> flattenedIntList = new List<int> ();

			var nestedArraysObject = nestedArrays as object[];
			if (nestedArraysObject != null) {
				FlattenNestedArrays (nestedArraysObject, ref flattenedIntList);
			} else {
				throw new Exception ("The initial object is not an array.");
			}

			TestFlatArrayInOrder (flattenedIntList.ToArray ());
			TestNullInput ();
		}
		#else
		public static void Main (string[] args)
		{
			object nestedArrays = CreateNestedIntArray (maxArrayDepth);
			List<int> flattenedIntList = new List<int> ();

			var nestedArraysObject = nestedArrays as object[];
			if (nestedArraysObject != null) {
				FlattenNestedArrays (nestedArraysObject, ref flattenedIntList);
			} else {
				throw new Exception ("The initial object is not an array.");
			}
		}
		#endif

		/// <summary>
		/// Flattens the nested arrays.
		/// </summary>
		/// <param name="nestedIntArray">Nested int array.</param>
		/// <param name="flattenedList">Flattened list.</param>
		private static void FlattenNestedArrays (object[] nestedIntArray, ref List<int> flattenedList)
		{	
			if (nestedIntArray == null) {
				throw new ArgumentNullException ();
			} else {
				foreach (object o in nestedIntArray) {
					var objectArray = o as object[];
					if (objectArray != null) {
						// current object is an array, call this method recursively
						FlattenNestedArrays (objectArray, ref flattenedList);
					} else {
						if (o is int) {
							int intVal = (int)o;
							// current object is an int, find the correct index and insert into flattened list
							int index = flattenedList.BinarySearch (intVal);
							if (index < 0) {
								index = ~index;
							}
							flattenedList.Insert (index, intVal);
						} else {
							// data isnt an int or an array so can't be processed
							throw new Exception ("Array contains a non array/int value.");	
						}
					}
				}
			}
		}

		/// <summary>
		/// Creates a nested int array.
		/// Each array is of max lenght maxArraySize
		/// and the final array is of max depth maxArrayDepth.
		/// </summary>
		/// <returns>The nested int array.</returns>
		/// <param name="maxDepth">Maximum depth of final array.</param>
		private static object CreateNestedIntArray (int maxDepth)
		{
			int arraySize = random.Next (0, maxArraySize);

			object[] objectArray = new object[arraySize];

			for (int i = 0; i < arraySize; i++) {
				int arrayOrInt = random.Next (0, 2);	// upper bound is exclusive. 0 for int, 1 for array
				if (arrayOrInt == 0 || maxDepth == 0) {
					objectArray [i] = random.Next (0, 100);
				} else {
					objectArray [i] = CreateNestedIntArray (maxDepth - 1);
				}
			}
			return objectArray;
		}

		#if Test_NestedArrays
		/// <summary>
		/// Tests that an array of ints is in ascending order.
		/// </summary>
		/// <param name="orderedFlatIntArray">Int array to be checked.</param>
		private static void TestFlatArrayInOrder (int[] orderedFlatIntArray)
		{
			for (int i = 0; i < orderedFlatIntArray.Length - 1; i++) {
				Debug.Assert (orderedFlatIntArray [i] <= orderedFlatIntArray [i + 1], 
					"Element " + i.ToString() + " is greater than element " + (i+1).ToString() + ". The array is not in order");
			}
		}

		/// <summary>
		/// Asserts that a null object will raise an exception.
		/// </summary>
		private static void TestNullInput ()
		{
			object[] nullObject = null;
			List<int> flattenedIntList = new List<int> ();
			try {
				FlattenNestedArrays (nullObject, ref flattenedIntList);
				Debug.Assert (false, "Exception should have been thrown for null object.");
			} catch (ObjectNotArrayException e) {
				// correct exception thrown, do nothing.
				Console.WriteLine("Expected object not array message thrown. " + e.Message);
			} catch (ArgumentNullException e) {
				// expected exception thrown
				Console.WriteLine("Expected null argument exception thrown. " + e.Message);
			} catch (Exception e) {
				Debug.Assert (false, "Unexpected exception was thrown." + e.Message);
			}
		}
		#endif
	}

	public class ObjectNotArrayException : Exception 
	{
		public ObjectNotArrayException () { }

		public ObjectNotArrayException (string message) 
			:base(message)
		{ 
		}
	}
}

/* test array is in order
 * test null value
 * test invalid array
 */
