using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeGenerationTools.Generator.Base;
using Mono.Cecil;

namespace CodeGenerationTools.Generator
{
    public class HelperGenerator : GeneratorBase<Tuple<Dictionary<string, TypeDefinition>, Dictionary<string, TypeDefinition>, Dictionary<string, TypeReference>, Dictionary<string, TypeDefinition>>>
    {
        private string _filePath;
        private DelegateConveterGenerator _dcg;
        private DelegateRegisterGenerator _drg;
        private AdaptorRegisterGenerator _arg;

        public override bool LoadTemplateFromFile(string filePath)
        {
            _filePath = Path.GetDirectoryName(filePath);
            _dcg = new DelegateConveterGenerator();
            _drg = new DelegateRegisterGenerator();
            _arg = new AdaptorRegisterGenerator();

            return base.LoadTemplateFromFile(filePath);
        }

        public override bool LoadData(Tuple<Dictionary<string, TypeDefinition>, Dictionary<string, TypeDefinition>, Dictionary<string, TypeReference>,Dictionary<string,TypeDefinition>> data)
        {
            if (data?.Item1 == null)
                return false;
            if (data.Item2 == null)
                return false;
            if (data.Item3 == null)
                return false;
            if (data.Item4 == null)
                return false;

            string nsStr = null;

            var adptorStr = "";
            foreach (var type in data.Item1.Values)
            {
                if (nsStr == null)
                    nsStr = type.Namespace;
                adptorStr += CreateAdaptorInit(type);
            }
            SetKeyValue("{$AdaptorInit}", adptorStr);

            var delegateStr = "";
            foreach (var type in data.Item2.Values)
            {
                if (nsStr == null)
                    nsStr = type.Namespace;
                delegateStr += CreateDelegateConvertorInit(type);
            }
            SetKeyValue("{$DelegateInit}", delegateStr);

            var delegateRegStr = "";

            //过滤掉相同类型
            Dictionary<string, TypeReference>  dict = FilterSampleTypes(data.Item3);

            foreach (var val in dict.Values)
            {
                delegateRegStr += CreateDelegateRegisterInit(val);
            }
            SetKeyValue("{$DelegateRegInit}", delegateRegStr);

            var interfaceRegStr = "";
            foreach (var type in data.Item4.Values)
            {
                interfaceRegStr += CreateAdaptorInit(type);
            }
            SetKeyValue("{$InterfaceAdaptorInit}", interfaceRegStr);

            SetKeyValue("{$Namespace}", string.IsNullOrWhiteSpace(nsStr)?"ILRuntime":nsStr);

            return true;
        }

        /// <summary>
        /// 过滤掉相同委托类型
        /// </summary>
        /// <param name="values"></param>
        private Dictionary<string, TypeReference> FilterSampleTypes(Dictionary<string, TypeReference> dict)
        {
            Dictionary<string, TypeReference> filterDic = new Dictionary<string, TypeReference>();

            //用来存储类型
            List<string> temp = new List<string>();

            foreach (var item in dict.Values)
            {
                var gtype = item as GenericInstanceType;
                if (gtype != null)
                {//处理 Action & Func
                    if (gtype.FullName.Contains("Action"))
                    {
                        string argsType = "";
                        if (gtype != null)
                        {
                            foreach (var param in gtype.GenericArguments)
                            {
                                if (param != null)
                                    argsType += param.FullName;
                            }
                        }

                        argsType += "Action";

                        if (!temp.Contains(argsType))
                        {
                            temp.Add(argsType);
                            filterDic.Add(item.FullName, item);
                        }
                    }
                    else if (gtype.FullName.Contains("Func"))
                    {
                        string argsType = "";
                        if (gtype != null)
                        {
                            foreach (var param in gtype.GenericArguments)
                            {
                                if (param != null)
                                    argsType += param.FullName;
                            }
                        }


                        argsType += "Func";

                        if (!temp.Contains(argsType))
                        {
                            temp.Add(argsType);
                            filterDic.Add(item.FullName, item);
                        }
                    }

                }
                else
                {
                    var td = item as TypeDefinition;

                    if (td == null)
                        continue;

                    string argsType = "";
                    foreach (var method in td.Methods)
                    {
                        if (method.FullName.Contains("Invoke") && !method.FullName.Contains("BeginInvoke") && !method.FullName.Contains("EndInvoke"))
                        {//如果是Invoke方法,获取其参数
                            foreach (var parameter in method.Parameters)
                            {
                                if (parameter != null)
                                    argsType += parameter.ParameterType;
                            }

                            //如果返回值不为null,则添加返回值
                            if (!method.ReturnType.FullName.Equals("System.Void"))
                                argsType += method.ReturnType.FullName + "Func";
                            else
                                argsType += "Action";

                            //判断是否有
                            if (!temp.Contains(argsType))
                            {
                                temp.Add(argsType);
                                filterDic.Add(item.FullName, item);
                            }

                            break;
                        }
                    }
                }
                //TypeDefinition td = item.Resolve();
            }

            return filterDic;

        }

        private string CreateAdaptorInit(TypeDefinition type)
        {
            _arg.InitFromFile(_filePath + "/adaptor_register.tmpd", type);
            return _arg.Generate();
        }

        private string CreateDelegateRegisterInit(object type)
        {
            string tmpd = null;
            if (type is Type)
            {
                var t = (Type)type;
                var method = t.GetMethod("Invoke");
                if (method == null)
                    return "";
                tmpd = method.ReturnType == typeof(void) ? "action_register.tmpd" : "function_register.tmpd";
            }
            else if (type is TypeReference)
            {
                var tr = (TypeReference)type;
                if (tr.FullName.Contains("Action") || tr.FullName.Contains("Func"))
                {//如果委托类型是Action或者Func,直接判定采用那个模板
                    tmpd = tr.FullName.Contains("Action") ? "action_register.tmpd" : "function_register.tmpd";
                }
                else
                {
                    var td = tr.Resolve();
                    if (td != null)
                    {
                        foreach (var method in td.Methods)
                        {
                            if (method.FullName.Contains("Invoke") && !method.FullName.Contains("BeginInvoke") && !method.FullName.Contains("EndInvoke"))
                            {//如果是Invoke方法,获取其参数

                                tmpd = method.ReturnType.FullName.Equals("System.Void") ? "action_register.tmpd" : "function_register.tmpd";
                                break;
                            }
                        }
                    }
                }

                
            }
            _drg.InitFromFile(_filePath + Path.AltDirectorySeparatorChar + tmpd, type);

            return _drg.Generate();
        }

        private string CreateDelegateConvertorInit(TypeDefinition type)
        {
            var method = type.Methods.FirstOrDefault(m => m.Name == "Invoke");//GetMethod("Invoke");
            if (method == null)
                return "";
            var tmpd = method.ReturnType.FullName == "System.Void" ? "delegate_void.tmpd" : "delegate_return.tmpd";// == typeof(void) ? "delegate_void.tmpd" : "delegate_return.tmpd";
            _dcg.InitFromFile(_filePath + Path.AltDirectorySeparatorChar + tmpd, type);
            return _dcg.Generate();
        }
    }
}
