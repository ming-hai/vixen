﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.Property {
	abstract public class PropertyModuleInstanceBase : ModuleInstanceBase, IPropertyModuleInstance, IEqualityComparer<IPropertyModuleInstance>, IEquatable<IPropertyModuleInstance>, IEqualityComparer<PropertyModuleInstanceBase>, IEquatable<PropertyModuleInstanceBase> {
		virtual public ChannelNode Owner { get; set; }

		abstract public void Setup();

		/// <summary>
		/// Set or reset the property's values to a property-specific default.
		/// This should only be called after the property has been added to a node.
		/// </summary>
		public abstract void SetDefaultValues();

		/// <summary>
		/// Clones the property-specific values of sourceProperty so that they're appropriate for the local node.
		/// </summary>
		public virtual void CloneValues(IProperty sourceProperty) {
			SetDefaultValues();
		}

		public bool Equals(IPropertyModuleInstance x, IPropertyModuleInstance y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IPropertyModuleInstance obj) {
			return base.GetHashCode(obj);
		}

		public bool Equals(IPropertyModuleInstance other) {
			return base.Equals(other);
		}

		public bool Equals(PropertyModuleInstanceBase x, PropertyModuleInstanceBase y) {
			return Equals(x as IPropertyModuleInstance, y as IPropertyModuleInstance);
		}

		public int GetHashCode(PropertyModuleInstanceBase obj) {
			return GetHashCode(obj as IPropertyModuleInstance);
		}

		public bool Equals(PropertyModuleInstanceBase other) {
			return Equals(other as IPropertyModuleInstance);
		}
	}
}
