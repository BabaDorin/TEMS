using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Net.Http.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace temsAPI.Services
{
    // BEFREE: I need this class to return the conversion rate from mdl to eur and usd
    // as fast as possible. I'll get back to it later to re-design it and to make it generic
    // in order to handle any currencies;
    public class CurrencyConvertor
    {
        public double EUR_MDL_rate { get; set; }
        public double USD_MDL_rate { get; set; }

        string mdl_eur_uri = "https://cdn.jsdelivr.net/gh/fawazahmed0/currency-api@1/latest/currencies/eur/mdl.json";
        string mdl_usd_uri = "https://cdn.jsdelivr.net/gh/fawazahmed0/currency-api@1/latest/currencies/usd/mdl.json";
        
        private Timer timer;

        private class CurrencyRate
        {
            public DateTime Date { get; set; }
            public double Mdl { get; set; }
        }

        public CurrencyConvertor()
        {
            RefreshRates();
            InitTimer();
        }

        private void InitTimer()
        {
            timer = new System.Timers.Timer(2 * 86400 * 1000); // 2 days
            timer.Elapsed += new ElapsedEventHandler(Interval);
            timer.Start();
        }

        private void Interval(object source, ElapsedEventArgs e)
        {
            RefreshRates();   
        }

        private void RefreshRates()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Accept.Clear();

                    HttpResponseMessage response = client.GetAsync(mdl_eur_uri).Result;
                    if (response.IsSuccessStatusCode)
                        EUR_MDL_rate = response.Content.ReadFromJsonAsync<CurrencyRate>().Result.Mdl;

                    response = client.GetAsync(mdl_usd_uri).Result;
                    if (response.IsSuccessStatusCode)
                        USD_MDL_rate = response.Content.ReadFromJsonAsync<CurrencyRate>().Result.Mdl;
                }
                catch (Exception ex)
                {
                    // Most probably there is no internet connection
                    Debug.WriteLine(ex);

                    // Here we set at least an aproximative value for each rate.
                    // (08.06.2021)
                    USD_MDL_rate = 18;
                    EUR_MDL_rate = 21;
                }
            }
        }
    }
}
