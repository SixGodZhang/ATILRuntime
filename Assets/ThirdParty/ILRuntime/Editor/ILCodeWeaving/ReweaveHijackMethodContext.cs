using Editor_Mono.Cecil;
using Editor_Mono.Cecil.Cil;
using System.Linq;

namespace ILCodeWeaving
{
    /// <summary>
    /// hijack method
    /// </summary>
    public class ReweaveHijackMethodContext
    {
        #region Properties
        /// <summary>
        /// Method
        /// </summary>
        public MethodDefinition Method { get; set; }
        /// <summary>
        /// MainModule
        /// </summary>
        public ModuleDefinition MainModule { get; set; }
        #endregion

        #region Method
        /// <summary>
        /// Hijack Method
        /// </summary>
        /// <param name="delegateFieldName">委托实例全名</param>
        /// <param name="delegateTypeRef">委托类型</param>
        /// <param name="CommonDelegateSetType">定义委托实例的类型</param>
        public void HijackMethod(string delegateFieldName, TypeDefinition delegateTypeRef, TypeDefinition CommonDelegateSetType)
        {
            //find delegate
            FieldDefinition FieldDelegate = CommonDelegateSetType.Fields.ToList().Find((@delegate) => { return @delegate.FullName == delegateFieldName; });
            if (FieldDelegate == null)
                return;

            //Get the site of code injection
            var ilProcessor = Method.Body.GetILProcessor();
            var hijackPoint = ilProcessor.Body.Instructions.First();

            //import current delegate
            var invokeDeclare = MainModule.ImportReference(delegateTypeRef.Methods.Single(x => x.Name == "Invoke"));

            ilProcessor.InsertBefore(hijackPoint, ilProcessor.Create(OpCodes.Ldsfld, FieldDelegate));
            ilProcessor.InsertBefore(hijackPoint, ilProcessor.Create(OpCodes.Brfalse, hijackPoint));

            ilProcessor.InsertBefore(hijackPoint, ilProcessor.Create(OpCodes.Ldsfld, FieldDelegate));

            if (!Method.IsStatic)
                //load this pointer
                ilProcessor.InsertBefore(hijackPoint, LoadArgs(ilProcessor, 0));

            //load all parameters
            for (int i = 0; i < Method.Parameters.Count; i++)
            {
                ilProcessor.InsertBefore(hijackPoint, LoadArgs(ilProcessor, Method.IsStatic ? i : i + 1));
            }

            //call delegate
            ilProcessor.InsertBefore(hijackPoint, ilProcessor.Create(OpCodes.Call, invokeDeclare));

            ilProcessor.InsertBefore(hijackPoint, ilProcessor.Create(OpCodes.Ret));
        }

        /// <summary>
        /// load arg to evalucation stack 
        /// </summary>
        private static Instruction LoadArgs(ILProcessor iLProcessor, int c)
        {
            switch (c)
            {
                case 0:
                    return iLProcessor.Create(OpCodes.Ldarg_0);
                case 1:
                    return iLProcessor.Create(OpCodes.Ldarg_1);
                case 2:
                    return iLProcessor.Create(OpCodes.Ldarg_2);
                case 3:
                    return iLProcessor.Create(OpCodes.Ldarg_3);
            }

            if (c > 0 && c < byte.MaxValue)
                return iLProcessor.Create(OpCodes.Ldarga_S, (byte)c);

            return iLProcessor.Create(OpCodes.Ldarg, c);
        }
        
        #endregion
    }
}
