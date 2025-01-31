﻿using FBXSharp.Core;
using FBXSharp.ValueTypes;
using System;
using System.Collections.Generic;

namespace FBXSharp.Objective
{
	public class BindPose : FBXObject
	{
		public interface IBinding
		{
			FBXObject Target { get; set; }
			Matrix4x4 Matrix { get; set; }
		}

		public struct Binding : IBinding
		{
			public FBXObject Target { get; set; }
			public Matrix4x4 Matrix { get; set; }
		}

		private struct TempBind : IBinding
		{
			public long NodeID { get; set; }
			public FBXObject Target { get; set; }
			public Matrix4x4 Matrix { get; set; }
		}

		private readonly List<IBinding> m_bindings;

		public static readonly FBXObjectType FType = FBXObjectType.BindPose;

		public static readonly FBXClassType FClass = FBXClassType.Pose;

		public override FBXObjectType Type => BindPose.FType;

		public override FBXClassType Class => BindPose.FClass;

		public IReadOnlyList<IBinding> Bindings => this.m_bindings;

		internal BindPose(IElement element, IScene scene) : base(element, scene)
		{
			this.m_bindings = new List<IBinding>();

			if (element is null)
			{
				return;
			}

			var nbPoseNodes = element.FindChild("NbPoseNodes");

			if (nbPoseNodes is null || nbPoseNodes.Attributes.Length == 0)
			{
				return;
			}

			if (Convert.ToInt32(nbPoseNodes.Attributes[0].GetElementValue()) == 0)
			{
				return;
			}

			for (int i = 0; i < element.Children.Length; ++i)
			{
				var child = element.Children[i];

				if (child.Name != "PoseNode")
				{
					continue;
				}

				var nodeID = child.FindChild("Node");
				var matrix = child.FindChild("Matrix");

				if (nodeID is null || nodeID.Attributes.Length == 0 || nodeID.Attributes[0].Type != IElementAttributeType.Int64)
				{
					continue;
				}

				if (matrix is null || matrix.Attributes.Length == 0 || matrix.Attributes[0].Type != IElementAttributeType.ArrayDouble)
				{
					this.m_bindings.Add(new TempBind()
					{
						NodeID = Convert.ToInt64(nodeID.Attributes[0].GetElementValue()),
						Matrix = Matrix4x4.Identity,
					});
				}
				else
				{
					if (ElementaryFactory.ToMatrix4x4(matrix.Attributes[0], out var transform))
					{
						this.m_bindings.Add(new TempBind()
						{
							NodeID = Convert.ToInt64(nodeID.Attributes[0].GetElementValue()),
							Matrix = transform,
						});
					}
					else
					{
						this.m_bindings.Add(new TempBind()
						{
							NodeID = Convert.ToInt64(nodeID.Attributes[0].GetElementValue()),
							Matrix = Matrix4x4.Identity,
						});
					}
				}
			}
		}

		internal void InternalResolveAllBinds(IReadOnlyDictionary<long, FBXObject> objectMap)
		{
			for (int i = 0; i < this.m_bindings.Count; ++i)
			{
				if (this.m_bindings[i] is TempBind temp)
				{
					if (objectMap.TryGetValue(temp.NodeID, out var target))
					{
						this.m_bindings[i] = new Binding()
						{
							Target = target,
							Matrix = temp.Matrix,
						};
					}
					else
					{
						this.m_bindings.RemoveAt(i--);
					}
				}
			}
		}

		public void AddBinding(IBinding binding)
		{
			this.AddBindingAt(binding, this.m_bindings.Count);
		}
		public void RemoveBinding(IBinding binding)
		{
			for (int i = 0; i < this.m_bindings.Count; ++i)
			{
				var current = this.m_bindings[i];

				if (current.Target == binding.Target && current.Matrix == binding.Matrix)
				{
					this.m_bindings.RemoveAt(i);

					return;
				}
			}
		}
		public void AddBindingAt(IBinding binding, int index)
		{
			if (binding.Target is null)
			{
				return;
			}

			if (binding.Target.Scene != this.Scene)
			{
				throw new Exception("Binding target should share same scene with bind pose");
			}

			if (index < 0 || index > this.m_bindings.Count)
			{
				throw new ArgumentOutOfRangeException("Index should be in range 0 to binding count inclusively");
			}

			for (int i = 0; i < this.m_bindings.Count; ++i)
			{
				if (this.m_bindings[i].Target == binding.Target)
				{
					this.m_bindings[i] = binding;

					return;
				}
			}

			this.m_bindings.Insert(index, binding);
		}
		public void RemoveBindingAt(int index)
		{
			if (index < 0 || index >= this.m_bindings.Count)
			{
				throw new ArgumentOutOfRangeException("Index should be in 0 to binding count range");
			}

			this.m_bindings.RemoveAt(index);
		}

		public override IElement AsElement(bool binary)
		{
			var elements = new IElement[3 + this.m_bindings.Count];

			elements[0] = Element.WithAttribute("Type", ElementaryFactory.GetElementAttribute("BindPose"));
			elements[1] = Element.WithAttribute("Version", ElementaryFactory.GetElementAttribute(100));
			elements[2] = Element.WithAttribute("NbPoseNodes", ElementaryFactory.GetElementAttribute(this.m_bindings.Count));

			for (int i = 0; i < this.m_bindings.Count; ++i)
			{
				var binding = this.m_bindings[i];

				elements[3 + i] = new Element("PoseNode", new IElement[]
				{
					Element.WithAttribute("Node", ElementaryFactory.GetElementAttribute((long)binding.Target.GetHashCode())),
					Element.WithAttribute("Matrix", ElementaryFactory.GetElementAttribute(binding.Matrix)),
				}, null);
			}

			return new Element(this.Class.ToString(), elements, this.BuildAttributes("Pose", this.Type.ToString(), binary));
		}
	}
}
