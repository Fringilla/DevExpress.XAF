﻿using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using Fasterflect;
using Microsoft.CSharp;
using Xpand.Extensions.AppDomain;
using Xpand.Extensions.XAF.XafApplication;
using Xpand.TestsLib;
using Xpand.XAF.Modules.ModelMapper.Configuration;
using Xpand.XAF.Modules.ModelMapper.Services;
using Xpand.XAF.Modules.ModelMapper.Tests.BOModel;
using TypeMappingService = Xpand.XAF.Modules.ModelMapper.Services.TypeMapping.TypeMappingService;

namespace Xpand.XAF.Modules.ModelMapper.Tests{
    public abstract class ModelMapperBaseTest:BaseTest{
        

        protected const string MMListViewNodePath = "Views/" + nameof(MM) + "_ListView";
        protected const string MMDetailViewNodePath = "Views/" + nameof(MM) + "_DetailView";
        protected const string MMDetailViewTestItemNodePath = "Views/" + nameof(MM) + "_DetailView/Items/Test";
        protected const string MMListViewTestItemNodePath = "Views/" + nameof(MM) + "_ListView/Columns/Test";
        public const string DynamicTypeName = "DynamicTypeName";
        internal ModelMapperModule DefaultModelMapperModule(string title,Platform platform,params ModuleBase[] modules){
            var xafApplication = platform.NewApplication<ModelMapperModule>();
            xafApplication.Modules.AddRange(modules);
            var modelMapperModule = xafApplication.AddModule<ModelMapperModule>(title,typeof(MM));
            xafApplication.Logon();
            using (xafApplication.CreateObjectSpace()){
            }

            return modelMapperModule;
        }

        protected static PropertyInfo[] ModelTypeProperties(Type modelType){
            return modelType.Properties().Where(info =>!TypeMappingService.ReservedPropertyNames.Contains(info.Name) &&
                                                       info.Name!=TypeMappingService.ModelMappersNodeName).ToArray();
        }

        private void ConfigureLayoutViewPredefinedMapService(PredefinedMap predefinedMap=PredefinedMap.LayoutView){
            if (new[]{PredefinedMap.LayoutView,PredefinedMap.LayoutViewColumn}.Contains(predefinedMap)){
                typeof(PredefinedMapService).Field("_xpandWinAssembly",Flags.Static|Flags.AnyVisibility).Set(GetType().Assembly);
                typeof(PredefinedMapService).Field("_layoutViewListEditorTypeName",Flags.Static|Flags.AnyVisibility).Set(typeof(CustomGridListEditor).FullName);
            }
        }

        internal Type CreateDynamicType(string name,string version="1.0.0.0"){
            var compilerParameters = new CompilerParameters{
                CompilerOptions = "/t:library",
                OutputAssembly = $@"{Path.GetDirectoryName(typeof(ModelMapperModule).Assembly.Location)}\DynamicType{name}{version.Substring(0,1)}.dll"
            };
            
            var codeProvider = new CSharpCodeProvider();
            compilerParameters.ReferencedAssemblies.Add(typeof(object).Assembly.Location);
            compilerParameters.ReferencedAssemblies.Add(typeof(AssemblyVersionAttribute).Assembly.Location);

            string code=$@"
[assembly:{typeof(AssemblyVersionAttribute).FullName}(""{version}"")]
[assembly:{typeof(AssemblyFileVersionAttribute).FullName}(""{version}"")]
public class {DynamicTypeName}{{
    public string Test{{ get; set; }}
}}
";
            var compilerResults = codeProvider.CompileAssemblyFromSource(compilerParameters, code);
            return compilerResults.CompiledAssembly.GetType(DynamicTypeName);
        }

        internal string InitializeMapperService(string modelMapperAssemblyName,Platform platform=Platform.Agnostic,bool newAssemblyName=true ){
            var mapperAssemblyName = $"{GetType().Name}{modelMapperAssemblyName}{platform}".GetHashCode();
            
            if (newAssemblyName){
                TypeMappingService.ModelMapperAssemblyName = $"{Guid.NewGuid()}{mapperAssemblyName}";
            }
            var applicationPath = AppDomain.CurrentDomain.ApplicationPath();
            var files = Directory.GetFiles(applicationPath,$"*{mapperAssemblyName}*.dll").ToArray();
            foreach (var file in files){
                try{
                    File.Delete(file);
                }
                catch (Exception){
                    // ignored
                }
            }

            TypeMappingService.Reset(platform:platform);
            TypeMappingService.OutputAssembly =
                $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\{Path.GetFileNameWithoutExtension(TypeMappingService.OutputAssembly)}.dll";
            ConfigureLayoutViewPredefinedMapService();
            return mapperAssemblyName.ToString();
        }
    }

}