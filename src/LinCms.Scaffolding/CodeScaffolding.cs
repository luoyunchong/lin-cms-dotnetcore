using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Scriban;
using Scriban.Runtime;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LinCms.Scaffolding
{
    public class CodeScaffolding
    {
        private readonly ILogger<App> _logger;
        public string TemplatePath { get; }
        public string OutputDir { get; }
        public string PostFix { get; set; } = ".txt";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="templatePath">模板的根目录（绝对目录）</param>
        /// <param name="outputDir">输出目录（绝对目录/相对目录）</param>
        public CodeScaffolding(string templatePath, string outputDir, ILogger<App> logger)
        {
            TemplatePath = templatePath;
            OutputDir = outputDir;
            _logger = logger;
        }

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
                _logger.LogInformation(templateText);

                TemplateContext context = new TemplateContext();
                ScriptObject scriptObject = new ScriptObject();
                scriptObject.Import(model, renamer: member => member.Name);
                context.PushGlobal(scriptObject);
                context.MemberRenamer = member => member.Name;

                Template template = Template.Parse(templateText);
                string result = template.Render(context);
                _logger.LogInformation(result);

                Template pathTemplate = Template.Parse(Path.Combine(OutputDir, path.RemovePostFix(PostFix)));
                string targetFilePathName = pathTemplate.Render(context);

                string dir = Path.GetDirectoryName(targetFilePathName);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                File.WriteAllText(targetFilePathName, result);
                count += 1;
            }
            _logger.LogInformation($"共生成{count}个");
        }
    }
}
