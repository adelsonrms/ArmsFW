/// <summary>
/// Contem objetos para tratativa de security
/// </summary>
namespace ArmsFW.Security
{
    /// <summary>
    /// Status de um Token
    /// </summary>
    public enum eTokenStatus
    {
        NaoGerado = -1,
        Desconhecido = 0,
        Valido,
        Expirado,
        ClienteInvalido,
        EmissorInvalido,
        AssinanteInvalido,
        Excessao, Bloquado
    }
}


