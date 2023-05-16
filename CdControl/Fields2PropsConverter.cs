using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Linq;

namespace CdControl {
	internal class Fields2PropsConverter : CustomTypeDescriptor {
		private object realObj;

		public Fields2PropsConverter(object realObj) {
			this.realObj = realObj;
		}

		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes) {
			var realType = realObj.GetType();

			List<PropertyDescriptor> props = new List<PropertyDescriptor>();


			foreach(var fieldInfo in realType.GetFields()) {
				props.Add(new DummyPropDescriptor(fieldInfo));
			}

			return new PropertyDescriptorCollection(props.ToArray());
		}

		public override object GetPropertyOwner(PropertyDescriptor pd) {
			return realObj;
		}

		private class DummyPropDescriptor : PropertyDescriptor {
			private FieldInfo fieldInfo;

			public DummyPropDescriptor(FieldInfo fieldInfo) : base(fieldInfo.Name, MakeAtts(fieldInfo)) {
				this.fieldInfo = fieldInfo;
			}

			private static Attribute[] MakeAtts(FieldInfo fieldInfo) {
				return null;
				foreach(var att in fieldInfo.CustomAttributes) {
					//att.Constructor.Invoke(att.ConstructorArguments.);
				}
			}

			public override Type ComponentType => throw new NotImplementedException();

			public override bool IsReadOnly => fieldInfo.IsInitOnly;

			public override Type PropertyType => fieldInfo.FieldType;

			public override bool CanResetValue(object component) {
				throw new NotImplementedException();
			}

			public override object GetValue(object component) {
				return fieldInfo.GetValue(component);
			}

			public override void ResetValue(object component) {
				throw new NotImplementedException();
			}

			public override void SetValue(object component, object value) {
				fieldInfo.SetValue(component, value);
			}

			public override bool ShouldSerializeValue(object component) {
				DefaultValueAttribute defValAtt = (DefaultValueAttribute)fieldInfo.GetCustomAttribute(typeof(DefaultValueAttribute));
				if(defValAtt == null) return true;
				return defValAtt.Value == GetValue(component);
			}
		}
	}
}