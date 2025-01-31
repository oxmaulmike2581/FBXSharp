﻿using FBXSharp.Core;
using FBXSharp.ValueTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FBXSharp.Objective
{
	public abstract class Model : FBXObject
	{
		public enum ShadingType
		{
			HardShading,
			WireFrame,
			FlatShading,
			LightShading,
			TextureShading,
			FullShading,
		}

		public enum CullingType
		{
			CullingOff,
			CullingOnCCW,
			CullingOnCW,
		}

		public enum InheritanceType
		{
			InheritRrSs,
			InheritRSrs,
			InheritRrs
		};

		private readonly List<Model> m_children;
		private NodeAttribute m_attribute;
		private Model m_parent;

		public static readonly FBXClassType FClass = FBXClassType.Model;

		public override FBXClassType Class => Model.FClass;

		public abstract bool SupportsAttribute { get; }

		public NodeAttribute Attribute
		{
			get => this.InternalGetNodeAttribute();
			set => this.InternalSetNodeAttribute(value);
		}

		public Model Parent => this.m_parent;

		public IReadOnlyList<Model> Children => this.m_children;

		public ShadingType Shading { get; set; }

		public CullingType Culling { get; set; }
				
		public Enumeration RotationOrder
		{
			get => this.InternalGetEnumeration(nameof(this.RotationOrder));
			set => this.InternalSetEnumeration(nameof(this.RotationOrder), value, "enum", System.String.Empty);
		}

		public Vector3? RotationOffset
		{
			get => this.InternalGetPrimitive<Vector3>(nameof(this.RotationOffset), IElementPropertyType.Double3);
			set => this.InternalSetPrimitive<Vector3>(nameof(this.RotationOffset), IElementPropertyType.Double3, value, "Vector3D", "Vector3D", IElementPropertyFlags.Animatable);
		}

		public Vector3? RotationPivot
		{
			get => this.InternalGetPrimitive<Vector3>(nameof(this.RotationPivot), IElementPropertyType.Double3);
			set => this.InternalSetPrimitive<Vector3>(nameof(this.RotationPivot), IElementPropertyType.Double3, value, "Vector3D", "Vector3D", IElementPropertyFlags.Animatable);
		}

		public Vector3? PreRotation
		{
			get => this.InternalGetPrimitive<Vector3>(nameof(this.PreRotation), IElementPropertyType.Double3);
			set => this.InternalSetPrimitive<Vector3>(nameof(this.PreRotation), IElementPropertyType.Double3, value, "Vector3D", "Vector3D", IElementPropertyFlags.Animatable);
		}

		public Vector3? PostRotation
		{
			get => this.InternalGetPrimitive<Vector3>(nameof(this.PostRotation), IElementPropertyType.Double3);
			set => this.InternalSetPrimitive<Vector3>(nameof(this.PostRotation), IElementPropertyType.Double3, value, "Vector3D", "Vector3D", IElementPropertyFlags.Animatable);
		}

		public Vector3? ScalingOffset
		{
			get => this.InternalGetPrimitive<Vector3>(nameof(this.ScalingOffset), IElementPropertyType.Double3);
			set => this.InternalSetPrimitive<Vector3>(nameof(this.ScalingOffset), IElementPropertyType.Double3, value, "Vector3D", "Vector3D", IElementPropertyFlags.Animatable);
		}

		public Vector3? ScalingPivot
		{
			get => this.InternalGetPrimitive<Vector3>(nameof(this.ScalingPivot), IElementPropertyType.Double3);
			set => this.InternalSetPrimitive<Vector3>(nameof(this.ScalingPivot), IElementPropertyType.Double3, value, "Vector3D", "Vector3D", IElementPropertyFlags.Animatable);
		}

		public Vector3? LocalTranslation
		{
			get => this.InternalGetPrimitive<Vector3>("Lcl Translation", IElementPropertyType.Double3);
			set => this.InternalSetPrimitive<Vector3>("Lcl Translation", IElementPropertyType.Double3, value, "Lcl Translation", "Lcl Translation", IElementPropertyFlags.Animatable);
		}

		public Vector3? LocalRotation
		{
			get => this.InternalGetPrimitive<Vector3>("Lcl Rotation", IElementPropertyType.Double3);
			set => this.InternalSetPrimitive<Vector3>("Lcl Rotation", IElementPropertyType.Double3, value, "Lcl Rotation", "Lcl Rotation", IElementPropertyFlags.Animatable);
		}

		public Vector3? LocalScale
		{
			get => this.InternalGetPrimitive<Vector3>("Lcl Scaling", IElementPropertyType.Double3);
			set => this.InternalSetPrimitive<Vector3>("Lcl Scaling", IElementPropertyType.Double3, value, "Lcl Scaling", "Lcl Scaling", IElementPropertyFlags.Animatable);
		}

		public double? Visibility
		{
			get => this.InternalGetPrimitive<double>(nameof(this.Visibility), IElementPropertyType.Double);
			set => this.InternalSetPrimitive<double>(nameof(this.Visibility), IElementPropertyType.Double, value, "Visibility", String.Empty, IElementPropertyFlags.Animatable);
		}

		public bool? VisibilityInheritance
		{
			get => this.InternalGetPrimitive<bool>("Visibility Inheritance", IElementPropertyType.Bool);
			set => this.InternalSetPrimitive<bool>("Visibility Inheritance", IElementPropertyType.Bool, value, "Visibility Inheritance", String.Empty);
		}

		public int? DefaultAttributeIndex
		{
			get => this.InternalGetPrimitive<int>(nameof(this.DefaultAttributeIndex), IElementPropertyType.Int);
			set => this.InternalSetPrimitive<int>(nameof(this.DefaultAttributeIndex), IElementPropertyType.Int, value, "int", "Integer");
		}

		public InheritanceType? InheritType
		{
			get => this.InternalGetEnumType(nameof(this.InheritType), out InheritanceType type) ? type : (InheritanceType?)null;
			set => this.InternalSetEnumType(nameof(this.InheritType), value.HasValue, (int)(value ?? 0), "enum", String.Empty);
		}

		internal Model(IElement element, IScene scene) : base(element, scene)
		{
			this.m_children = new List<Model>();
			this.ParseDepthFields(element);
		}

		private void ParseDepthFields(IElement element)
		{
			if (element is null)
			{
				return;
			}

			var shading = element.FindChild(nameof(this.Shading));
			var culling = element.FindChild(nameof(this.Culling));

			if (!(shading is null) && shading.Attributes.Length > 0)
			{
				var type = (char)Convert.ToByte(shading.Attributes[0].GetElementValue());

				switch (type)
				{
					case 'W': this.Shading = ShadingType.WireFrame; break;
					case 'F': this.Shading = ShadingType.FlatShading; break;
					case 'Y': this.Shading = ShadingType.LightShading; break;
					case 'T': this.Shading = ShadingType.TextureShading; break;
					case 'U': this.Shading = ShadingType.FullShading; break;
					default: this.Shading = ShadingType.HardShading; break;
				}
			}

			if (!(culling is null) && culling.Attributes.Length > 0)
			{
				if (Enum.TryParse(culling.Attributes[0].GetElementValue().ToString(), out CullingType type))
				{
					this.Culling = type;
				}
			}
		}

		private NodeAttribute InternalGetNodeAttribute()
		{
			if (!this.SupportsAttribute)
			{
				throw new NotSupportedException("Model does not support node attributes");
			}

			return this.m_attribute;
		}
		
		private void InternalSetNodeAttribute(NodeAttribute attribute)
		{
			if (!this.SupportsAttribute)
			{
				throw new NotSupportedException("Model does not support node attributes");
			}

			if (attribute.Type != this.Type)
			{
				throw new Exception("Node attribute should have same type as the Model");
			}

			this.m_attribute = attribute;
		}

		internal void InternalSetChild(Model child)
		{
			this.m_children.Add(child);
			child.InternalSetParent(this);
		}
		
		internal void InternalSetParent(Model parent)
		{
			this.m_parent = parent;
		}

		private Matrix4x4 EvaluateLocal(in Vector3 position, in Vector3 rotation)
		{
			var scale = this.LocalScale;

			return this.EvaluateLocal(position, rotation, scale.HasValue ? scale.Value : Vector3.One);
		}

		private Matrix4x4 EvaluateLocal(in Vector3 position, in Vector3 rotation, in Vector3 scale)
		{
			var rotationPivot = this.RotationPivot.GetValueOrDefault();
			var scalingPivot = this.ScalingPivot.GetValueOrDefault();
			var rotationOrder = (RotationOrder)(this.RotationOrder?.Value ?? 0);

			var t = Matrix4x4.CreateTranslation(position);
			var s = Matrix4x4.CreateScale(scale);
			var r = Matrix4x4.CreateFromEuler(rotation, rotationOrder);

			var rpre = Matrix4x4.CreateFromEuler(this.PreRotation.GetValueOrDefault(), ValueTypes.RotationOrder.XYZ);
			var post = Matrix4x4.CreateFromEuler(-this.PostRotation.GetValueOrDefault(), ValueTypes.RotationOrder.ZYX);

			var roff = Matrix4x4.CreateTranslation(this.RotationOffset.GetValueOrDefault());
			var rpip = Matrix4x4.CreateTranslation(rotationPivot);
			var rpii = Matrix4x4.CreateTranslation(-rotationPivot);

			var soff = Matrix4x4.CreateTranslation(this.ScalingOffset.GetValueOrDefault());
			var spip = Matrix4x4.CreateTranslation(scalingPivot);
			var spii = Matrix4x4.CreateTranslation(-scalingPivot);

			return t * roff * rpip * rpre * r * post * rpii * soff * spip * s * spii;
		}

		protected IElement MakeElement(string className, bool binary)
		{
			var elements = new IElement[6];

			byte shading = 0;

			switch (this.Shading)
			{
				case ShadingType.WireFrame: shading = (byte)'W'; break;
				case ShadingType.FlatShading: shading = (byte)'F'; break;
				case ShadingType.LightShading: shading = (byte)'Y'; break;
				case ShadingType.TextureShading: shading = (byte)'T'; break;
				case ShadingType.FullShading: shading = (byte)'U'; break;
			}

			elements[0] = Element.WithAttribute("Version", ElementaryFactory.GetElementAttribute(232));
			elements[1] = this.BuildProperties70();
			elements[2] = Element.WithAttribute("MultiLayer", ElementaryFactory.GetElementAttribute(false));
			elements[3] = Element.WithAttribute("MultiTake", ElementaryFactory.GetElementAttribute(0));
			elements[4] = Element.WithAttribute("Shading", ElementaryFactory.GetElementAttribute(shading));
			elements[5] = Element.WithAttribute("Culling", ElementaryFactory.GetElementAttribute(this.Culling.ToString()));

			return new Element(this.Class.ToString(), elements, this.BuildAttributes(className, this.Type.ToString(), binary));
		}

		public Matrix4x4 GetLocalTransform()
		{
			var translation = this.LocalTranslation;
			var rotation = this.LocalRotation;
			var scale = this.LocalScale;

			return this.EvaluateLocal(translation.GetValueOrDefault(), rotation.GetValueOrDefault(), scale.HasValue ? scale.Value : Vector3.One);
		}

		public Matrix4x4 GetGlobalTransform()
		{
			if (this.Parent is null)
			{
				return this.EvaluateLocal(this.LocalTranslation.GetValueOrDefault(), this.LocalRotation.GetValueOrDefault());
			}
			else
			{
				return this.Parent.GetGlobalTransform() * this.EvaluateLocal(this.LocalTranslation.GetValueOrDefault(), this.LocalRotation.GetValueOrDefault());
			}
		}

		public void AddChild(Model model)
		{
			if (model is null)
			{
				return;
			}

			if (model.Scene != this.Scene)
			{
				throw new ArgumentException("Model passed should share same scene with the current model");
			}

			if (Object.ReferenceEquals(this, model))
			{
				throw new Exception("Cannot add itself as a child");
			}

			if (model.Parent is null)
			{
				this.InternalSetChild(model);
			}
			else if (model.Parent != this)
			{
				model.DetachFromParent();
				this.InternalSetChild(model);
			}
		}
		public void RemoveChild(Model model)
		{
			if (model is null || model.Scene != this.Scene || model.Parent != this)
			{
				return;
			}

			_ = this.m_children.Remove(model);
			model.m_parent = null;
		}
		public void AddChildAt(Model model, int index)
		{
			if (model is null)
			{
				return;
			}

			if (model.Scene != this.Scene)
			{
				throw new ArgumentException("Model passed should share same scene with the current model");
			}

			if (Object.ReferenceEquals(this, model))
			{
				throw new Exception("Cannot add itself as a child");
			}

			if (index < 0 || index > this.m_children.Count)
			{
				throw new ArgumentOutOfRangeException("Index should be in range 0 to children count inclusively");
			}

			if (model.Parent is null)
			{
				this.m_children.Insert(index, model);
				model.m_parent = this;
			}
			else if (model.Parent != this)
			{
				model.DetachFromParent();
				this.m_children.Insert(index, model);
				model.m_parent = this;
			}
		}
		public void RemoveChildAt(int index)
		{
			if (index < 0 || index >= this.m_children.Count)
			{
				throw new ArgumentOutOfRangeException("Index should be in 0 to children count range");
			}

			var model = this.m_children[index];
			this.m_children.RemoveAt(index);
			model.m_parent = null;
		}
		public void DetachFromParent()
		{
			if (this.m_parent is null)
			{
				return;
			}

			_ = this.m_parent.m_children.Remove(this);
			this.m_parent = null;
		}
		public void DetachAllChildren()
		{
			foreach (var child in this.m_children)
			{
				child.m_parent = null;
			}

			this.m_children.Clear();
		}

		public override Connection[] GetConnections()
		{
			if (this.m_children.Count == 0)
			{
				if (!this.SupportsAttribute || this.Attribute is null)
				{
					return Array.Empty<Connection>();
				}
				else
				{
					return new Connection[1]
					{
						new Connection(Connection.ConnectionType.Object, this.Attribute.GetHashCode(), this.GetHashCode()),
					};
				}
			}

			var attributeOn = this.SupportsAttribute && !(this.Attribute is null);
			var connections = new Connection[this.m_children.Count + (attributeOn ? 1 : 0)];

			for (int i = 0; i < this.m_children.Count; ++i)
			{
				connections[i] = new Connection(Connection.ConnectionType.Object, this.m_children[i].GetHashCode(), this.GetHashCode());
			}

			if (attributeOn)
			{
				connections[this.m_children.Count] = new Connection(Connection.ConnectionType.Object, this.Attribute.GetHashCode(), this.GetHashCode());
			}

			return connections;
		}

		public override void ResolveLink(FBXObject linker, IElementAttribute attribute)
		{
			if (linker.Class == FBXClassType.Model)
			{
				this.AddChild(linker as Model);

				return;
			}

			if (linker.Class == FBXClassType.NodeAttribute)
			{
				this.InternalSetNodeAttribute(linker as NodeAttribute);

				return;
			}
		}

		public override void Destroy()
		{
			this.DetachAllChildren();
			base.Destroy();
		}
	}

	public abstract class NodeAttribute : FBXObject
	{
		public static readonly FBXClassType FClass = FBXClassType.NodeAttribute;

		public override FBXClassType Class => NodeAttribute.FClass;

		internal NodeAttribute(IElement element, IScene scene) : base(element, scene)
		{
		}
	}
}
