using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Editor_Mono.Cecil;
using Editor_Mono.Cecil.Cil;

namespace ILCodeWeaving
{
    /// <summary>
    /// reweave method
    /// </summary>
    public class ReweaveMethodContext
    {
        #region Properties
        /// <summary>
        /// method
        /// </summary>
        public MethodDefinition Method { get; set; }
        /// <summary>
        /// module of method
        /// </summary>
        public ModuleDefinition MainModule { get; set; }
        #endregion

        #region Public interface

        public void Returns(object returnValue)
        {
            var returnString = returnValue as string;

            //Get the site of code injection
            var ilProcessor = Method.Body.GetILProcessor();
            var firstInstruction = ilProcessor.Body.Instructions.First();

            ilProcessor.InsertBefore(firstInstruction, ilProcessor.Create(OpCodes.Ldstr, returnString));
            ilProcessor.InsertBefore(firstInstruction, ilProcessor.Create(OpCodes.Ret));
        }

        public void Returns<TInstance>(params object[] returnObject) where TInstance : class, new()
        {

        }

        #endregion
    }
}
