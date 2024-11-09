using Dotnet.Homeworks.Mailing.API.Dto;
using Dotnet.Homeworks.Shared.Dto;

namespace Dotnet.Homeworks.Mailing.API.Services;

public class MockMailingService : IMailingService
{
    public Task<Result> SendEmailAsync(EmailMessage emailDto)
    {
        Console.WriteLine(emailDto);
        return Task.FromResult(new Result(true));
    }
}