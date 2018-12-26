using System;
using CodeGenerationTools.Generator.Base;
using Mono.Cecil;

namespace CodeGenerationTools.Generator
{
    public class DelegateRegisterGenerator : GeneratorBase<object>
    {
        public override bool LoadData(object data)
        {
            if (data == null)
                return false;

            if (data is Type)
            {
                var type = (Type)data;
                var method = type.GetMethod("Invoke");
                if (method == null)
                    return false;

                //var tmpd = method.ReturnType == typeof(void) ? _actionRegisterTmpd : _functionRegisterTmpd;
                var argsType = "";
                var returnType = method.ReturnType == typeof(void) ? "" : method.ReturnType.Name;
                foreach (var param in method.GetParameters())
                {
                    argsType += param.ParameterType.Name + ",";
                }

                if (method.ReturnType != typeof(void))
                    argsType += returnType;
                argsType = argsType.Trim(',');
                SetKeyValue("{$argsType}", argsType);
            }
            else if (data is TypeReference)
            {
                var tr = (TypeReference)data;
                var argsType = "";
                //Console.WriteLine("===========>" + tr.FullName);
                var gtype = tr as GenericInstanceType;
                var td = tr as TypeDefinition;
                if (gtype == null && td.GenericParameters.Count == 0)
                {//该data来自于主工程
                    foreach (var method in td.Methods)
                    {
                        if (method.FullName.Contains("Invoke")&& !method.FullName.Contains("BeginInvoke")&&!method.FullName.Contains("EndInvoke"))
                        {//如果是Invoke方法,获取其参数
                            foreach (var parameter in method.Parameters)
                            {
                                if (parameter != null)
                                    argsType += parameter.ParameterType + ",";
                            }

                            //如果返回值不为null,则添加返回值
                            if (!method.ReturnType.FullName.Equals("System.Void"))
                                argsType += method.ReturnType.FullName;

                            argsType = argsType.Trim(',');
                            SetKeyValue("{$argsType}", argsType);
                            return true;
                        }
                    }

                    return false;

                    //if (td == null || td.GenericParameters.Count == 0)
                    //{
                    //    SetKeyValue("{$argsType}", argsType);
                    //    return true;
                    //}
                }

                if (gtype != null)
                {
                    foreach (var param in gtype.GenericArguments)
                    {
                        if (param != null)
                            argsType += param.FullName + ",";
                    }
                }


                argsType = argsType.Trim(',');
                SetKeyValue("{$argsType}", argsType);
            }
            else
                return false;

            return true;
        }
    }
}
