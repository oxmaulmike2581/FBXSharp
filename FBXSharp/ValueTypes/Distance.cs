﻿namespace FBXSharp.ValueTypes
{
	public struct Distance
	{
		public float Value;
		public string Unit;

		public Distance(float value, string unit)
		{
			this.Value = value;
			this.Unit = unit;
		}
	}
}
