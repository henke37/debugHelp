﻿using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Reflection;

namespace CdControl {
	internal class Fields2PropsConverter : ICustomTypeDescriptor {
		private object realObj;

		public Fields2PropsConverter(object realObj) {
			this.realObj = realObj;
		}

		public PropertyDescriptorCollection GetProperties() {
			var realType = realObj.GetType();

			List<PropertyDescriptor> props = new List<PropertyDescriptor>();

			foreach(var propInfo in realType.GetProperties()) {
				props.Add(TypeDescriptor.GetProperties(realObj)[propInfo.Name]);
			}

			foreach(var fieldInfo in realType.GetFields()) {
				props.Add(new DummyPropDescriptor(fieldInfo));
			}

			return new PropertyDescriptorCollection(props.ToArray());
		}
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes) => GetProperties();

		public EventDescriptor GetDefaultEvent() => TypeDescriptor.GetDefaultEvent(realObj);
		public TypeConverter GetConverter() => TypeDescriptor.GetConverter(realObj);
		public AttributeCollection GetAttributes() => TypeDescriptor.GetAttributes(realObj);
		public EventDescriptorCollection GetEvents() => TypeDescriptor.GetEvents(realObj);
		public EventDescriptorCollection GetEvents(Attribute[] attributes) => TypeDescriptor.GetEvents(realObj, attributes);
		public object GetEditor(Type editorBaseType) => TypeDescriptor.GetEditor(realObj, editorBaseType);
		public string GetComponentName() => TypeDescriptor.GetComponentName(realObj);
		public PropertyDescriptor GetDefaultProperty() => TypeDescriptor.GetDefaultProperty(realObj);
		public string GetClassName() => TypeDescriptor.GetClassName(realObj);

		public object GetPropertyOwner(PropertyDescriptor pd) => realObj;

		private class DummyPropDescriptor : PropertyDescriptor {
			private FieldInfo fieldInfo;

			public DummyPropDescriptor(FieldInfo fieldInfo) : base(fieldInfo.Name, fieldInfo.GetCustomAttributes() as Attribute[]) {
				this.fieldInfo = fieldInfo;
			}

			public override Type ComponentType => fieldInfo.DeclaringType;

			public override bool IsReadOnly => fieldInfo.IsInitOnly;

			public override Type PropertyType => fieldInfo.FieldType;

			public override bool CanResetValue(object component) => false;

			public override object GetValue(object component) => fieldInfo.GetValue(component);

			public override void ResetValue(object component) => throw new NotSupportedException();

			public override void SetValue(object component, object value) => fieldInfo.SetValue(component, value);

			public override bool ShouldSerializeValue(object component) => false;
		}
	}
}