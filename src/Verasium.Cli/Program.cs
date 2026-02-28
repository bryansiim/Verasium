using Verasium.Core;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("=====Bem vindo ao detector de IA Verasium!======");

        Console.WriteLine("\nDigite a opção que voce deseja acessar");
        Console.WriteLine("1- AIScanner");
        Console.WriteLine("0- Sair");

        var option = Console.ReadLine();

        if (option == "1")
        {
            Console.WriteLine("Cole o texto ou arraste o arquivo e pressione ENTER.");
            string inputContent = Console.ReadLine()?.Trim('"');

            if (string.IsNullOrWhiteSpace(inputContent))
            {
                Console.WriteLine("Nenhum conteúdo fornecido.");
                return;
            }

            try
            {
                var gemini = new GeminiAnalyzer();
                var analyzer = new FileAnalyzer(inputContent, gemini);
                var result = await analyzer.RunAnalysis();

                Console.WriteLine("=====Resposta da IA=====");

                if (result.IsSuccessful)
                {
                    Console.WriteLine(result.AiResponse);
                }
                else
                {
                    Console.WriteLine($"Erro: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado: {ex.Message}");
            }
        }
        else if (option == "0")
        {
            Console.WriteLine("Saindo...");
        }
        else
        {
            Console.WriteLine("Opcao inválida, tente novamente");
        }
    }
}
