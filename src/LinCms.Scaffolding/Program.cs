using Humanizer;
using LinCms.Scaffolding.Entities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LinCms.Scaffolding
{
    class Program
    {
        static ProjectInfo ProjectParser( SettingOptions settingOptions)
        {
            Console.WriteLine($"baseDirectory：{settingOptions.BaseDirectory}"); ;
            var coreCsprojFile = Directory.EnumerateFiles(settingOptions.BaseDirectory, "*.Core.csproj", SearchOption.AllDirectories).FirstOrDefault();

            var fileName = Path.GetFileName(coreCsprojFile);
            var fullName = fileName.RemovePostFix(".Core.csproj");
            var projectInfo = new ProjectInfo(settingOptions.BaseDirectory, fullName);

            return projectInfo;
        }

        private static EntityInfo EntityParse(string entityFilePath, ProjectInfo projectInfo, SettingOptions settingOptions)
        {
            var sourceText = File.ReadAllText(entityFilePath);

            SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceText);

            var root = tree.GetCompilationUnitRoot();
            var @namespace = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>().Single().Name.ToString();//不满足项目命名空间
            var classDeclarationSyntax = root.DescendantNodes().OfType<ClassDeclarationSyntax>().Single();
            var className = classDeclarationSyntax.Identifier.ToString();
            var baseList = classDeclarationSyntax.BaseList;
            var genericNameSyntax = baseList.DescendantNodes().OfType<SimpleBaseTypeSyntax>()
                .First(node => !node.ToFullString().StartsWith("I")) // Not interface
                .DescendantNodes().OfType<GenericNameSyntax>()
                .FirstOrDefault();

            string baseType;
            string primaryKey;
            if (genericNameSyntax == null)
            {
                // No generic parameter -> Entity with Composite Keys
                baseType = baseList.DescendantNodes().OfType<SimpleBaseTypeSyntax>().Single().Type.ToString();
                primaryKey = "long";

            }
            else
            {
                // Normal entity
                baseType = genericNameSyntax.Identifier.ToString();
                primaryKey = genericNameSyntax.DescendantNodes().OfType<TypeArgumentListSyntax>().Single().Arguments[0].ToString();
            }
            List<PropertyInfo> properties = root.DescendantNodes().OfType<PropertyDeclarationSyntax>()
                  .Select(prop =>

                        new PropertyInfo(prop.Type.ToString(), prop.Identifier.Value.ToString())
                  )
                  .ToList();

            string xmlPath = settingOptions.BaseDirectory + projectInfo.FullName + ".Core.xml";
            string entityRemark = Util.GetEntityRemarkBySummary(xmlPath, properties, @namespace + "." + className);


            if (settingOptions.Areas != null)
            {
                @namespace = projectInfo.FullName + "." + settingOptions.Areas + "." + className.Pluralize();
            }
            else
            {
                @namespace = projectInfo.FullName + "." + className.Pluralize();
            }
            var relativeDirectory = @namespace.RemovePreFix(projectInfo.FullName + ".").Replace('.', '/');

            EntityInfo entityInfo = new EntityInfo(@namespace, className, baseType, primaryKey, relativeDirectory);
            entityInfo.Properties.AddRange(properties);
            entityInfo.EntityRemark = entityRemark;

            return entityInfo;
        }

        static ServiceProvider GetProvider()
        {
            var build = new ConfigurationBuilder();
            build.SetBasePath(Directory.GetCurrentDirectory());
            build.AddJsonFile("appsettings.json", true, true);
            IConfigurationRoot configurationRoot = build.Build();
            var service = new ServiceCollection();
            service.Configure<SettingOptions>(configurationRoot.GetSection(SettingOptions.Name));
            ServiceProvider provider = service.BuildServiceProvider();
            return provider;
        }

        static async Task Main(string[] args)
        {

            ServiceProvider provider = GetProvider();
            SettingOptions settingOptions = provider.GetService<IOptionsMonitor<SettingOptions>>().CurrentValue;
            ProjectInfo projectInfo = ProjectParser(settingOptions);


            string entityPath = Path.Combine(settingOptions.BaseDirectory, settingOptions.EntityFilePath);
            EntityInfo entityInfo = EntityParse(entityPath, projectInfo, settingOptions);

            var model = new
            {
                ProjectInfo = projectInfo,
                EntityInfo = entityInfo,
                Option = new CommandOption()
                {
                    CustomRepository = false,
                },
                SettingOptions = settingOptions
            };
            /*
            -   ProjectInfo	{LinCms.Scaffolding.Entities.ProjectInfo}	LinCms.Scaffolding.Entities.ProjectInfo
                BaseDirectory	"D:/code/github/lin-cms-dotnetcore/src/LinCms.Core/"	string
                FullName	"LinCms"	string
                Name	"LinCms"	string
           
            -   EntityInfo
                BaseType	"FullAduitEntity"	string
                Name	"Doc"	string		
                NameCamelize	"doc"	string
                NamePluralized	"Docs"	string
                Namespace	"LinCms.v1.Docs"	string
                NamespaceLastPart	"Docs"	string
                PrimaryKey	"long"	string
                RelativeDirectory	"v1/Docs"	string
                RelativeNamespace	"v1.Docs"	string

            -   SettingOptions	{LinCms.Scaffolding.SettingOptions}	LinCms.Scaffolding.SettingOptions
                Areas	"Base"	string
                AreasCamelize	"base"	string
                BaseDirectory	"D:/code/github/lin-cms-dotnetcore/src/LinCms.Core/"	string
                EntityFilePath	"Entities/Doc.cs"	string
                OutputDirectory	"D:/code/github/Outputs"	string
                ProjectName	"LinCms"	string
                TemplatePath	"./Templates"	string

        */
            string templatePath = Path.Combine(Environment.CurrentDirectory, settingOptions.TemplatePath);
            CodeScaffolding codeScaffolding = new CodeScaffolding(templatePath, settingOptions.OutputDirectory);
            await codeScaffolding.GenerateAsync(model);

            Console.WriteLine("EveryThings is Ok!");
        }
    }
}
