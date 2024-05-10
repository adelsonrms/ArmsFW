/// <summary>
/// Contem objetos para tratativa de security
/// </summary>
namespace ArmsFW.Security
{
    public enum eAuthType
    {
        TokenJwt,
        UserPassword,
        Code,
        ClientSecret,
        NaoAutenticado,
        ApiKey
    }
}


