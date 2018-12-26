using Editor_Mono.Cecil;
using Editor_Mono.Collections.Generic;
using GameFramework.Taurus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace ILCodeWeaving
{
    public class ILCodeWeaver:IHideObjectMembers
    {
        /// <summary>
        /// assembly pth
        /// </summary>
        private readonly string _assemblyPath;

        private readonly string _saveAssemblyPath;
        /// <summary>
        /// Assembly context
        /// </summary>
        private readonly AssemblyDefinition _assemblyDefinition;

        /// <summary>
        /// all exported types
        /// </summary>
        public Dictionary<string, TypeDefinition> AllTypes = new Dictionary<string, TypeDefinition>();


        #region private methods
        /// <summary>
        /// GetAllTypes
        /// </summary>
        private void LoadAllTypes()
        {
            foreach (var type in _assemblyDefinition.MainModule.Types)
            {
                //UnityEngine.Debug.LogError(type.FullName);
                //AllTypes[type.Name] = type;
                AllTypes[type.FullName] = type;
                //AllTypes.Add(type.Name, type);
                //AllTypes.Add(type.FullName, type);
            }
        }
        #endregion

        #region Common Interfaces

        #region .ctor
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="assemblyPath"></param>
        public ILCodeWeaver(string assemblyPath)
        {
            _assemblyPath = assemblyPath;  
            _saveAssemblyPath = _assemblyPath.Replace("ScriptAssemblies", "ScriptAssemblies");
            _assemblyDefinition = AssemblyDefinition.ReadAssembly(assemblyPath);

            var readerParameters = new ReaderParameters { ReadSymbols = false };
            _assemblyDefinition = AssemblyDefinition.ReadAssembly(assemblyPath, readerParameters);

            var resolver = _assemblyDefinition.MainModule.AssemblyResolver as BaseAssemblyResolver;
            resolver.AddSearchDirectory(Path.GetFullPath("./Library/UnityAssemblies"));
            resolver.AddSearchDirectory(Path.GetFullPath("./UnityEngineLibaray"));
            LoadAllTypes();
        }
        #endregion

        /// <summary>
        /// get typedefine by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TypeDefinition GetTypeDefinitionByName(string name)
        {
            return AllTypes.Keys.Contains(name) ? AllTypes[name] : null;
        }

        /// <summary>
        /// Tuple<string,string,string,string,string>
        /// 1.完整的委托类型名
        /// 2.注入方法的类型名称
        /// 3.注入的方法名
        /// 4.绑定的委托类型名
        /// 5.声明修复委托的类名
        /// </summary>
        /// <returns></returns>
        public List<Tuple<string,string,string,string,string>> GetInjectMessage()
        {
            List<Tuple<string, string, string, string, string>> tuples = new List<Tuple<string, string, string, string, string>>();
            TypeDefinition delgateType = GetTypeDefinitionByName("ILRuntime.BugfixDelegateStatements");
            if (delgateType == null)
            {
                UnityEngine.Debug.LogError("ILRuntime.BugfixDelegateStatements == null");
                return null;
            }
            Collection<FieldDefinition> fields = delgateType.Fields;
            foreach (var field in fields)
            {
                //1.field.FullName Item1
                //...

                //2.split_step_1[0] Item4
                string[] split_step_1 = field.FullName.Split(' ');

                //3.split_step_2[0] Item5
                string[] split_step_2 = Regex.Split(split_step_1[1], "::", RegexOptions.IgnoreCase);

                //4.split_step_3[0].Replace('_', '.'); Item2
                string[] split_step_3 = Regex.Split(split_step_2[1], "_NM_", RegexOptions.IgnoreCase);
                //split_step_3[0].Replace('_', '.');

                //5.split_step_4[0] Item3
                string[] split_step_4 = Regex.Split(split_step_3[1], "_IN_", RegexOptions.IgnoreCase);

                //6.split_step_5[0] params
                string[] split_step_5 = Regex.Split(split_step_4[1], "_OUT_", RegexOptions.IgnoreCase);
                

                //7.split_step_6[0] return
                string[] split_step_6 = Regex.Split(split_step_5[1], "__Delegate", RegexOptions.IgnoreCase);

                string methodSignature = split_step_6[0].Replace('_', '.') + " " + split_step_3[0].Replace('_', '.') + "::" + split_step_4[0] + "("+ split_step_5[0].Replace("_0", ",").Replace("0", "").Replace("_", ".") +")";

                tuples.Add(new Tuple<string, string, string, string, string>(field.FullName,split_step_3[0].Replace('_', '.'), methodSignature, split_step_1[0], split_step_2[0]));
                UnityEngine.Debug.Log(field.FullName);
            }


            return tuples;
        }

        /// <summary>
        /// get all need inject methods
        /// </summary>
        /// <returns></returns>
        public List<MethodDefinition> GetAllInjectedMethods()
        {
            List<MethodDefinition> methods = new List<MethodDefinition>();

            TypeDefinition allhotfixDefinition = GetTypeDefinitionByName("GameFramework.Taurus.AllowHotfixAttribute");
            foreach (var type in AllTypes.Values)
            {
                if (HasCustomAttribute(type, "AllowHotfixAttribute") && type.FullName.Contains("TestMainHotfix"))
                {
                    methods.AddRange(type.Methods);
                }
            }

            return methods;

        }

        /// <summary>
        /// type has constaion attribute
        /// </summary>
        /// <param name="type"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        private bool HasCustomAttribute(TypeDefinition type, string attributeName)
        {
            if (!type.HasCustomAttributes)
                return false;

            for (int i = 0; i < type.CustomAttributes.Count; i++)
                if (type.CustomAttributes[i].AttributeType.Name == attributeName)
                    return true;

            return false;
        }

        /// <summary>
        /// get methoddefinition
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public MethodDefinition GetMethodDefinition(string typeName, string methodName)
        {
            var methids = GetTypeDefinitionByName(typeName).Methods;
            return methids.Single((m) => m.FullName == methodName);
        }
        

        /// <summary>
        /// setup hijack
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public ReweaveHijackMethodContext SetupHijackMethod(MethodDefinition method)
        {
            return new ReweaveHijackMethodContext
            {
                MainModule = _assemblyDefinition.MainModule,
                Method = method,
            };
        }


        /// <summary>
        /// Method
        /// </summary>
        public ReweaveMethodContext SetupMethod(Expression<Action> expression)
        {
            var methodCall = expression.Body as MethodCallExpression;
            var methodDeclaringType = methodCall.Method.DeclaringType;

            var type = _assemblyDefinition.MainModule.Types
                .Single(t => t.Name == methodDeclaringType.Name);
            var method = type.Methods
                .Single(m => m.Name == methodCall.Method.Name);

            return new ReweaveMethodContext
            {
                MainModule = _assemblyDefinition.MainModule,
                Method = method,
            };
        }

        /// <summary>
        /// Prop
        /// </summary>
        public ReweavePropContext SetupProp(Expression<Func<string>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            var declaringType = memberExpression.Member.DeclaringType;
            var propertyType = memberExpression.Member;

            var typeDef = _assemblyDefinition.MainModule.Types
                .Single(t => t.Name == declaringType.Name);
            var propertyDef = typeDef.Properties
                .Single(p => p.Name == propertyType.Name);

            return new ReweavePropContext
            {
                MainModule = _assemblyDefinition.MainModule,
                Property = propertyDef,
            };
        }

        /// <summary>
        /// Reweave
        /// </summary>
        public void Reweave()
        {
            _assemblyDefinition.Write(_saveAssemblyPath);
        }

        #endregion
    }
}
