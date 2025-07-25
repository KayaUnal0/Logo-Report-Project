using System.IO;
using System.Threading.Tasks;
using Scriban;

namespace Infrastructure.Logic.Templates
{
    public class TemplateRenderer
    {
        public async Task<string> RenderTemplateAsync(string templatePath, object model)
        {
            var templateText = await File.ReadAllTextAsync(templatePath);
            var template = Template.Parse(templateText);
            return template.Render(model, member => member.Name);
        }
    }
}
