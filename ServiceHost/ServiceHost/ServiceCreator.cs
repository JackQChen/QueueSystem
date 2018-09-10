using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Web.Http;

namespace ServiceHost
{
    public class ServiceCreator
    {

        //HelloService->HelloServiceController:ApiController
        public static void Create(Type[] serviceTypes)
        {
            AppDomain domain = AppDomain.CurrentDomain;
            AssemblyName asmName = new AssemblyName() { Name = "ApplicationService" };
            AssemblyBuilder asmBuilder = domain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder modBuilder = asmBuilder.DefineDynamicModule(asmName.Name, asmName.Name + ".dll");
            foreach (var serviceType in serviceTypes)
            {
                TypeBuilder typeBuilder = modBuilder.DefineType("ApplicationService." + serviceType.Name + "Controller", TypeAttributes.Public, typeof(ApiController));
                FieldBuilder fieldBuilder = typeBuilder.DefineField("_serviceObj", serviceType, FieldAttributes.Private);
                ConstructorBuilder ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
                ILGenerator ilg = ctorBuilder.GetILGenerator();
                //OpCodes.Ldarg_0 = base
                //ApiController.ctor();
                ilg.Emit(OpCodes.Ldarg_0);
                ilg.Emit(OpCodes.Call, typeof(ApiController).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null));
                //newObject
                ilg.Emit(OpCodes.Ldarg_0);
                ilg.Emit(OpCodes.Newobj, serviceType.GetConstructor(Type.EmptyTypes));
                ilg.Emit(OpCodes.Stfld, fieldBuilder);
                ilg.Emit(OpCodes.Ret);
                MethodAttributes methodAttrs = MethodAttributes.Public | MethodAttributes.HideBySig;
                Assembly attrAssembly = typeof(HttpGetAttribute).Assembly;
                var prefix = "System.Web.Http.";
                #region RESTful
                ////Get
                //ILGenerator getIL = typeBuilder.DefineMethod("Get", methodAttrs, typeof(string), Type.EmptyTypes).GetILGenerator();
                //getIL.Emit(OpCodes.Ldarg_0);
                //getIL.Emit(OpCodes.Ldfld, fieldBuilder);
                //getIL.Emit(OpCodes.Call, serviceType.GetMethod("GetTest", Type.EmptyTypes));
                //getIL.Emit(OpCodes.Ret);
                //var paramType = new Type[] { typeof(string) };
                ////GetById
                //ILGenerator getByIdIL = typeBuilder.DefineMethod("Get", methodAttrs, typeof(string), paramType).GetILGenerator();
                //getByIdIL.Emit(OpCodes.Ldarg_0);
                //getByIdIL.Emit(OpCodes.Ldfld, fieldBuilder);
                //getByIdIL.Emit(OpCodes.Ldarg_1);
                //getByIdIL.Emit(OpCodes.Call, serviceType.GetMethod("GetTest", paramType));
                //getByIdIL.Emit(OpCodes.Ret);
                ////Post
                //ILGenerator postIL = typeBuilder.DefineMethod("Post", methodAttrs, typeof(string), paramType).GetILGenerator();
                //postIL.Emit(OpCodes.Ldarg_0);
                //postIL.Emit(OpCodes.Ldfld, fieldBuilder);
                //postIL.Emit(OpCodes.Ldarg_1);
                //postIL.Emit(OpCodes.Call, serviceType.GetMethod("PostTest", paramType));
                //postIL.Emit(OpCodes.Ret);
                #endregion
                foreach (var method in serviceType.GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(p => p.Module.Name == "BLL.dll"))
                {
                    var methodParams = method.GetParameters();
                    var methodBuilder = typeBuilder.DefineMethod(method.Name, methodAttrs, method.ReturnType, methodParams.Select(s => s.ParameterType).ToArray());
                    var attrNames = method.GetCustomAttributes().Select(s => s.GetType().Name);
                    //HttpGet
                    if (!attrNames.Contains("HttpGetAttribute"))
                    {
                        var getAttrBuilders = new CustomAttributeBuilder(attrAssembly.GetType(prefix + "HttpGetAttribute").GetConstructor(Type.EmptyTypes), new object[] { });
                        methodBuilder.SetCustomAttribute(getAttrBuilders);
                    }
                    //HttpPost
                    if (!attrNames.Contains("HttpPostAttribute"))
                    {
                        var getAttrBuilders = new CustomAttributeBuilder(attrAssembly.GetType(prefix + "HttpPostAttribute").GetConstructor(Type.EmptyTypes), new object[] { });
                        methodBuilder.SetCustomAttribute(getAttrBuilders);
                    }
                    foreach (var attrName in attrNames)
                    {
                        var attrBuilder = new CustomAttributeBuilder(attrAssembly.GetType(prefix + attrName).GetConstructor(Type.EmptyTypes), new object[] { });
                        methodBuilder.SetCustomAttribute(attrBuilder);
                    }
                    for (int i = 0; i < methodParams.Length; i++)
                        methodBuilder.DefineParameter(i + 1, ParameterAttributes.None, methodParams[i].Name);
                    ILGenerator methodIL = methodBuilder.GetILGenerator();
                    methodIL.Emit(OpCodes.Ldarg_0);
                    methodIL.Emit(OpCodes.Ldfld, fieldBuilder);
                    for (int i = 0; i < methodParams.Length; i++)
                        methodIL.Emit(OpCodes.Ldarg, i + 1);
                    methodIL.Emit(OpCodes.Call, method);
                    methodIL.Emit(OpCodes.Ret);
                }
                typeBuilder.CreateType();
            }
            asmBuilder.Save(asmName.Name + ".dll");
        }
    }
}
