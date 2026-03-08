using MetadataExtractor;

namespace Verasium.Core
{
    //Responsável por extrair metadados de arquivos
    public class MetadataExtractorService
    {
        public MetadataInfo Extract(string inputContent)
        {
            var info = new MetadataInfo { InputContent = inputContent };

            if (!File.Exists(inputContent))
            {
                info.MetadataError = "Arquivo nao encontrado, mas pode ser apenas um texto";
                return info;
            }

            try
            {
                var directories = ImageMetadataReader.ReadMetadata(inputContent);

                foreach (var directory in directories)
                {
                    foreach (var tag in directory.Tags)
                    {
                        string key = $"{directory.Name} - {tag.Name}";
                        info.Tags.TryAdd(key, tag.Description ?? "");
                    }
                }
            }
            catch (Exception ex)
            {
                info.MetadataError = $"Erro ao ler metadados do arquivo: {ex.Message}";
            }

            return info;
        }
    }
}
