﻿using FBXSharp.Core;

namespace FBXSharp.Objective
{
	public class NullNode : Model
	{
		public static readonly FBXObjectType FType = FBXObjectType.Null;

		public override FBXObjectType Type => NullNode.FType;

		public override bool SupportsAttribute => true;

		internal NullNode(IElement element, IScene scene) : base(element, scene)
		{
		}

		public override IElement AsElement(bool binary)
		{
			return this.MakeElement("Model", binary);
		}
	}

	public class NullAttribute : NodeAttribute
	{
		public static readonly FBXObjectType FType = FBXObjectType.Null;

		public override FBXObjectType Type => NullAttribute.FType;

		internal NullAttribute(IElement element, IScene scene) : base(element, scene)
		{
		}

		public override IElement AsElement(bool binary)
		{
			var elements = new IElement[2];

			elements[0] = Element.WithAttribute("TypeFlags", ElementaryFactory.GetElementAttribute("Null"));
			elements[1] = this.BuildProperties70();

			return new Element(this.Class.ToString(), elements, this.BuildAttributes("NodeAttribute", this.Type.ToString(), binary));
		}
	}
}
