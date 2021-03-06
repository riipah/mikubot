﻿using System;
using System.Linq;

namespace MikuBot
{
	/// <summary>
	/// Type-safe enum
	/// </summary>
	/// <typeparam name="T">Enum type</typeparam>
	public struct EnumVal<T> where T : struct, IConvertible
	{
		private T val;
		private readonly int valInt;

		/// <summary>
		/// Checks whether a flag has been set.
		/// </summary>
		/// <param name="flags">Flag array.</param>
		/// <param name="flag">Flag to check.</param>
		/// <returns>True if the flag is set.</returns>
		public static bool FlagIsSet(T flags, T flag)
		{
			return (Convert.ToInt32(flags) & Convert.ToInt32(flag)) == Convert.ToInt32(flag);
		}

		/// <summary>
		/// Gets individual values from a bitfield.
		/// </summary>
		/// <param name="flags">Bitfield to be parsed.</param>
		/// <returns>Individual set values.</returns>
		public static T[] GetIndividualValues(T flags)
		{
			return Values.Where(val => FlagIsSet(flags, val)).ToArray();
		}

		/// <summary>
		/// List of possible values for this enum.
		/// </summary>
		public static T[] Values
		{
			get
			{
				return (
					from T value in Enum.GetValues(typeof(T))
					select value
					).ToArray();
			}
		}

		/// <summary>
		/// Parses a valid enum value from a string.
		/// </summary>
		/// <param name="value">String representation of the enum value.</param>
		/// <returns>Enum value matching the string.</returns>
		public static T Parse(string value)
		{
			return (T)Enum.Parse(typeof(T), value);
		}

		/// <summary>
		/// Initializes a new instance of enum
		/// </summary>
		/// <param name="flags">Enum flags to set.</param>
		public EnumVal(T flags)
		{
			this.val = flags;
			this.valInt = Convert.ToInt32(val);
		}

		/// <summary>
		/// Gets or sets the current value
		/// </summary>
		public T Value
		{
			get { return val; }
			set { val = value; }
		}

		/// <summary>
		/// Checks whether a flag has been set.
		/// </summary>
		/// <param name="flag">Flag to check.</param>
		/// <returns>True if the flag is set.</returns>
		public bool FlagIsSet(T flag)
		{
			return (valInt & Convert.ToInt32(flag)) == Convert.ToInt32(flag);
		}
	}
}
