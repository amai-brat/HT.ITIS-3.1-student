using Dotnet.Homeworks.DataAccess.Specs.Infrastructure;
using Dotnet.Homeworks.Domain.Entities;

namespace Dotnet.Homeworks.DataAccess.Specs;

public class UsersSpecs : IUsersSpecs
{
    public Specification<User> HasGoogleEmail()
    {
       return new Specification<User>(user => user.Email.EndsWith("@gmail.com"));
    }

    public Specification<User> HasYandexEmail()
    {
        return new Specification<User>(user => user.Email.EndsWith("@yandex.ru"));
    }

    public Specification<User> HasMailEmail()
    {
        return new Specification<User>(user => user.Email.EndsWith("@mail.ru"));
    }

    public Specification<User> HasPopularEmailVendor()
    {
        return HasGoogleEmail() || HasYandexEmail() || HasMailEmail();
    }

    public Specification<User> HasLongName()
    {
        return new Specification<User>(user => user.Name.Length > 15);
    }

    public Specification<User> HasCompositeNameWithWhitespace()
    {
        return new Specification<User>(user => user.Name.Any(char.IsWhiteSpace));
    }

    public Specification<User> HasCompositeNameWithHyphen()
    {
        return new Specification<User>(user => user.Name.Contains('-'));
    }

    public Specification<User> HasCompositeName()
    {
        return HasCompositeNameWithWhitespace() || HasCompositeNameWithHyphen();
    }

    public Specification<User> HasComplexName()
    {
        return HasLongName() && HasCompositeName();
    }
}