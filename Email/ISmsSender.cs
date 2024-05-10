using System.Threading.Tasks;

namespace ArmsFW.Services.Email
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
