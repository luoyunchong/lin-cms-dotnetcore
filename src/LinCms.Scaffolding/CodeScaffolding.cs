using Microsoft.Extensions.FileProviders;
using Scriban;
using Scriban.Runtime;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LinCms.Scaffolding
{
    public class CodeScaffolding
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="templatePath">模板的根目录（绝对目录）</param>
        /// <param name="outputDir">输出目录（绝对目录/相对目录）</param>
        public CodeScaffolding(string templatePath, string outputDir)
        {
            TemplatePath = templatePath;
            OutputDir = outputDir;
        }

        public string TemplatePath { get; }
        public string OutputDir { get; }

        public string PostFix { get; set; } = ".txt";

        public async Task GenerateAsync(object model)
        {
            IFileProvider fileProvider = new PhysicalFileProvider(TemplatePath);
            if (!Directory.Exists(OutputDir))
            {
                Directory.CreateDirectory(OutputDir);
            }

            int count = 0;
            foreach (var (path, file) in fileProvider.GetFilesRecursively("./"))
            {
                string templateText = await file.ReadAsStringAsync(Encoding.UTF8);
                Console.WriteLine(templateText);

                TemplateContext context = new TemplateContext();
                ScriptObject scriptObject = new ScriptObject();
                scriptObject.Import(model, renamer: member => member.Name);
                context.PushGlobal(scriptObject);
                context.MemberRenamer = member => member.Name;

                Template template = Template.Parse(templateText);
                string result = template.Render(context);
                Console.WriteLine(result);

                Template pathTemplate = Template.Parse(Path.Combine(OutputDir, path.RemovePostFix(PostFix)));
                string targetFilePathName = pathTemplate.Render(context);

                string dir = Path.GetDirectoryName(targetFilePathName);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                File.WriteAllText(targetFilePathName, result);
                count += 1;
            }
            Console.WriteLine(count);
        }
    }
}
