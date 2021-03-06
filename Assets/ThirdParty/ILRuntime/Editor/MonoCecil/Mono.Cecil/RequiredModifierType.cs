using Editor_Mono.Cecil.Metadata;
using System;
namespace Editor_Mono.Cecil
{
	public sealed class RequiredModifierType : TypeSpecification, IModifierType
	{
		private TypeReference modifier_type;
		public TypeReference ModifierType
		{
			get
			{
				return this.modifier_type;
			}
			set
			{
				this.modifier_type = value;
			}
		}
		public override string Name
		{
			get
			{
				return base.Name + this.Suffix;
			}
		}
		public override string FullName
		{
			get
			{
				return base.FullName + this.Suffix;
			}
		}
		private string Suffix
		{
			get
			{
				return " modreq(" + this.modifier_type + ")";
			}
		}
		public override bool IsValueType
		{
			get
			{
				return false;
			}
			set
			{
				throw new InvalidOperationException();
			}
		}
		public override bool IsRequiredModifier
		{
			get
			{
				return true;
			}
		}
		public override bool ContainsGenericParameter
		{
			get
			{
				return this.modifier_type.ContainsGenericParameter || base.ContainsGenericParameter;
			}
		}
		public RequiredModifierType(TypeReference modifierType, TypeReference type) : base(type)
		{
			Mixin.CheckModifier(modifierType, type);
			this.modifier_type = modifierType;
			this.etype = Editor_Mono.Cecil.Metadata.ElementType.CModReqD;
		}
	}
}
