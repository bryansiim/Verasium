using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.GenAI;
using Google.GenAI.Types;

namespace Verasium.Core
{
    //Classe responsável por fazer a análise da IA usando o Gemini
    public class GeminiAnalyzer
    {
        private readonly Client client;

        public GeminiAnalyzer()
        {
            //O cliente já le automaticamente a variavel GOOGLE_API_KEY setada no ambiente
            client = new Client();
        }


        public async Task<string> RunAIAsync(string resume)
        {
            var config = new GenerateContentConfig
            {
                SystemInstruction = new Content
                {
                    Parts = new List<Part> //Prompt de instrução do sistema
                    {
                        new Part { Text = "Você é um detector de conteúdo gerado por IA. " +
                        "Analise as informações fornecidas e determine se o conteúdo parece ter origem humana ou sintética (gerada por IA)." +
                        "\r\n\r\nVocê receberá os seguintes campos:" +
                        "\r\n- InputContent: caminho do arquivo ou texto analisado." +
                        "\r\n- Tags: metadados extraídos do arquivo (caso existam)." +
                        "\r\n- MetadataError: motivo pelo qual não foi possível obter metadados, se aplicável." +
                        "\r\n- MetadataAvailable: indica se há metadados disponíveis (true/false)." +
                        "\r\n\r\nCom base nesses dados, gere a resposta no formato abaixo:" +
                        "\r\n\r\n1. Conclusão resumida: indique claramente se o conteúdo foi provavelmente **gerado por IA** ou **produzido por humano**.  " +
                        "\r\n   Exemplo: “Provavelmente gerado por IA.”" +
                        "\r\n\r\n2. Score: um valor percentual de 0 a 100 indicando o grau de confiança na conclusão (quanto maior, maior a chance de ser IA)." +
                        "\r\n\r\n3. Justificativa: uma explicação breve e objetiva da razão por trás da conclusão (ex: “ausência de metadados típicos de captura por câmera real” " +
                        "ou “padrões linguísticos artificiais detectados”)." }
                    }
                },
                Temperature = 0.2,
                MaxOutputTokens = 256
            };

            var response = await client.Models.GenerateContentAsync(
                model: "gemini-2.0-flash",
                contents: resume,
                config: config
            );

            // Garante que não vai dar erro caso a resposta venha vazia
            return response?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text
                   ?? "Sem resposta do modelo.";


        }
    }
}

