using System.Threading.Tasks;

namespace temsAPI.Services.Actions
{
    interface IScheduledAction
    {
        Task Start();
    }
}
