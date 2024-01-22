using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BankTelegramBot
{
    public static class CenvertionMoney
    {


        public static List<object> ConnectWithJson()
        {
            HttpClient httpClient = new HttpClient();

            var reques = new HttpRequestMessage(HttpMethod.Get, "https://www.nbu.uz/exchange-rates/json/");
            var responce = httpClient.SendAsync(reques).Result;


            var body = responce.Content.ReadAsStringAsync().Result;


            return JsonConvert.DeserializeObject<List<object>>(body);
        }
        public static double Convertion(List<object> info, string CountryCode)
        {
            List<object> list = info;
            for (int i = 0; i < list.Count; i++)
            {
                JObject temp = JObject.Parse(JsonConvert.SerializeObject(list[i]));
                if (temp["code"].ToString().Equals(CountryCode))
                {
                    return Convert.ToDouble(temp["cb_price"]);
                }
            }
            return 0.0;
        }
    }
}
