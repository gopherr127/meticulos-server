using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Meticulos.Api.App.Icons
{
    public class NounProjectIconSearcher : IIconSearcher
    {
        private readonly IOptions<Settings> _settings;
        private readonly string _apiBaseUrl = "http://api.thenounproject.com/icons/";

        public NounProjectIconSearcher(IOptions<Settings> settings)
        {
            _settings = settings;
        }
        
        public async Task<List<Icon>> SearchForIcons(string term)
        {
            OAuth.OAuthRequest request = new OAuth.OAuthRequest();
            request.ConsumerKey = _settings.Value.TheNounProject_ConsumerKey;
            request.ConsumerSecret = _settings.Value.TheNounProject_ConsumerKeySecret;
            request.Method = "GET";
            request.SignatureMethod = OAuth.OAuthSignatureMethod.HmacSha1;
            request.Version = "1.0";
            request.RequestUrl = _apiBaseUrl + term;

            string authHeader = request.GetAuthorizationHeader();

            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.Add("Authorization", authHeader);

                var httpResponse = await http.GetAsync(request.RequestUrl);

                if (httpResponse.IsSuccessStatusCode)
                {
                    var jsonString = await httpResponse.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<NounProjectIconSearchResult>(jsonString);
                    return result.Icons.Select(i => i.ToIcon()).ToList();
                }
                else
                {
                    return new List<Icon>();
                }
            }
        }
    }

    public class NounProjectIconSearchResult
    {
        [JsonProperty("icons")]
        public List<NounProjectIcon> Icons { get; set; }
    }
    
    public class NounProjectIcon
    {
        public string term { get; set; }
        public string preview_url { get; set; }

        public Icon ToIcon()
        {
            return new Icon()
            {
                Name = term,
                Url = preview_url
            };
        }
    }
}
