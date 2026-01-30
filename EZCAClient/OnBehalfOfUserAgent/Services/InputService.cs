using EZCAClient.Models;

namespace OnBehalfOfUserAgent.Services;

public static class InputService
{
    public static DBSelfServiceScep SelectProfile(AvailableSelfServiceModel availableCAs)
    {
        
        if (availableCAs.TenantSelfServiceProfiles.Count == 0)
        {
            throw new Exception("No available CA profiles found for this tenant.");
        }
        if (availableCAs.TenantSelfServiceProfiles.Count == 1)
        {
            return availableCAs.TenantSelfServiceProfiles[0];
        }
        Console.WriteLine("Please Select CA:");
        for (int i = 0; i < availableCAs.TenantSelfServiceProfiles.Count; i++)
        {
            Console.WriteLine(
                $"Enter {i} to select {availableCAs.TenantSelfServiceProfiles[i].PolicyName}"
            );
        }
        int selection = -1;
        while (selection >= availableCAs.TenantSelfServiceProfiles.Count || selection < 0)
        {
            string? input = Console.ReadLine();
            if (
                !Int32.TryParse(input, out selection)
                || selection >= availableCAs.TenantSelfServiceProfiles.Count
                || selection < 0
            )
            {
                Console.WriteLine(
                    $"Invalid Input: Please enter a value between 0 "
                        + $"and {availableCAs.TenantSelfServiceProfiles.Count - 1}"
                );
            }
        }
        return availableCAs.TenantSelfServiceProfiles[selection];
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
