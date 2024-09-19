//using Mixpanel;
//using ProductManagement.Models;
//using System.Text;

using System.Threading.Tasks;

namespace ProductManagement.Services
{
    public interface IMixpanelService
    {
        Task<bool> TrackEventAsync(string eventName, object properties);
    }
}
