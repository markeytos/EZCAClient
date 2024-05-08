using EZCAClient.Models;

namespace SampleCode.Services;

public static class InputService
{
    public static AvailableCAModel SelectCA(AvailableCAModel[] availableCAs)
    {
        if (availableCAs == null || availableCAs.Any() == false)
        {
            throw new ArgumentNullException(nameof(availableCAs));
        }
        if (availableCAs.Length == 1)
        {
            return availableCAs[0];
        }
        Console.WriteLine("Please Select CA:");
        for (int i = 0; i < availableCAs.Length; i++)
        {
            Console.WriteLine($"Enter {i} to select {availableCAs[i].CAFriendlyName}");
        }
        int selection = -1;
        while (selection >= availableCAs.Length || selection < 0)
        {
            string? input = Console.ReadLine();
            if (
                !Int32.TryParse(input, out selection)
                || selection >= availableCAs.Length
                || selection < 0
            )
            {
                Console.WriteLine(
                    $"Invalid Input: Please enter a value between 0 " + $"and {availableCAs.Length}"
                );
            }
        }
        return availableCAs[selection];
    }

    public static string GetDomainName()
    {
        Console.WriteLine("Please enter the domain you want to register");
        string? input = Console.ReadLine();
        while (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("Please enter a valid domain you want to register");
            input = Console.ReadLine();
        }
        return input;
    }
}
