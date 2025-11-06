using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

namespace Verasium.Core
{
    //Essa classe é responsável por fazer toda a análise e devolve a resposta da IA
    public class FileAnalyzer 
    {
        private string inputContent;
        private MetadataInfo metadataInfo = new MetadataInfo();

        public void SetInputContent(string inputcontent)
        {
            inputContent = inputcontent;
        }

        //Roda a análise completa
        public async Task RunAnalysis()
        {
            var metadata = ExtractMetadata();
            var airesult = await RunAIAnalysis();
            Console.WriteLine("=====Resposta da IA=====");
            Console.WriteLine(airesult.aiResponse); //Devolve a resposta da IA
        }

        //Faz a extração de metadados do arquivo
        public MetadataInfo ExtractMetadata()
        {

            metadataInfo = new MetadataInfo { InputContent = inputContent };

            if (!File.Exists(inputContent))
            {
                metadataInfo.MetadataError = "Arquivo nao encontrado, mas pode ser apenas um texto";
                return metadataInfo;
            }

            try
            {
                var directories = ImageMetadataReader.ReadMetadata(inputContent);
                    
                foreach (var directory in directories)
                {
                    foreach (var tag in directory.Tags)
                    {
                        string key = $"{directory.Name} - {tag.Name}";
                        if (!metadataInfo.Tags.ContainsKey(key))
                            metadataInfo.Tags.Add(key, tag.Description); // Percorre os metadados e adiciona ao dicionário
                    }
                }

            }
            catch (Exception)
            {
                metadataInfo.MetadataError = "Erro ao ler metadados do arquivo.";
                return metadataInfo;
            }

            return metadataInfo;
        }



        //Manda os metadados/inputContent para a IA analisar
        public async Task<AIAnalysisResult> RunAIAnalysis()
        {
            AIAnalysisResult aiAnalysis = new AIAnalysisResult();

            string tagsFormatted = string.Join(", ", metadataInfo.Tags.Select(t => $"{t.Key}: {t.Value}"));

            string resume = ($"\nInputContent = {metadataInfo.InputContent}\nTags = {tagsFormatted}\nMetadataError = {metadataInfo.MetadataError}\n " +
             $"MetadataAvailable = {metadataInfo.MetadataAvailable}"); // Formata os metadados/inputContent para enviar para a IA

            var runAI = new GeminiAnalyzer();

            aiAnalysis.aiResponse = await runAI.RunAIAsync(resume);

            return aiAnalysis;
        }

    }
}
