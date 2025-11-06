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

            var analyzer = new FileAnalyzer();
            analyzer.SetInputContent(inputContent);

            await analyzer.RunAnalysis(); // Roda a análise completa

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