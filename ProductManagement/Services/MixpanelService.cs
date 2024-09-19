using Mixpanel;
using Newtonsoft.Json;
using ProductManagement.Models;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.Services
{
    public class MixpanelService : IMixpanelService
    {
        private readonly MixpanelClient _mixpanelClient;

        public MixpanelService()
        {
            _mixpanelClient = new MixpanelClient("874f28d8800d06ca29cf542aa0b618ad");
        }

     
        public async Task<bool> TrackEventAsync(string eventName, object product)
        {
            string distinctId = Guid.NewGuid().ToString();
            string insertId = Guid.NewGuid().ToString();
            
            var properties = new
            {
                token = "874f28d8800d06ca29cf542aa0b618ad",
                time = 12, 
                distinct_id = distinctId, 
                insert_id = insertId, 
                price = "123"
            };

            string jsonProperties = JsonConvert.SerializeObject(properties);
            var content = new StringContent(jsonProperties, Encoding.UTF8, "application/json");

            bool status = await _mixpanelClient.TrackAsync(eventName, null, content);

            return status;
        }

    }
}
