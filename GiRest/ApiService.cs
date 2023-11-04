using Newtonsoft.Json;
using System.Net;

namespace GiRest
{
    public class ApiService
    {
        private string apiUri;
        private string addRequestString;
        private object? addHeader;

        public ApiService(string apiUri, object? requestTemplate = null, object? headerTemplate = null)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            this.apiUri = apiUri;
            addRequestString = requestTemplate is null ? "" : HttpRequestSerializer.SerializeObject(requestTemplate, "");
            addHeader = headerTemplate;
        }

        private HttpWebRequest CreateRequest(string path, string method = "GET", object? request = null, object? data = null, object? header = null)
        {
            string dataJson = data is null ? "" : JsonConvert.SerializeObject(data);
            string requestString = 
                request is null ? 
                $"?{addRequestString}" : 
                $"{HttpRequestSerializer.SerializeObject(request)}&{addRequestString}";

#pragma warning disable SYSLIB0014
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create($"{apiUri}{path}{requestString}");
#pragma warning restore SYSLIB0014
            webRequest.Method = method;
            webRequest.Accept = "application/json";
            if (addHeader is not null)
            {
                HttpRequestSerializer.ParseHeaderFromObject(webRequest.Headers, addHeader);
            }
            if (header is not null)
            {
                HttpRequestSerializer.ParseHeaderFromObject(webRequest.Headers, header);
            }
            if (dataJson.Length > 0)
            {
                webRequest.SendChunked = true;
                webRequest.ContentLength = dataJson.Length;
                webRequest.ContentType = "application/json";
                try
                {
                    using (StreamWriter writer = new StreamWriter(webRequest.GetRequestStream()))
                    {
                        writer.Write(dataJson);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"{ex.Message}\n{path} {method}\n{requestString}\n{dataJson}");
                }
            }
            return webRequest;
        }
        private string GetResponseString(HttpWebRequest webRequest, out HttpStatusCode statusCode)
        {
            HttpWebResponse webResponse;
            try
            {
                webResponse = (HttpWebResponse)webRequest.GetResponse();
            }
            catch (WebException e)
            {
                var exceptionWebResponse = e.Response as HttpWebResponse;
                if (exceptionWebResponse is null) throw e;
                webResponse = exceptionWebResponse;
            }
            statusCode = webResponse.StatusCode;
            using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }

        public T DoRequest<T>(string path, string method = "GET", object? request = null, object? data = null, object? header = null) where T: new()
        {
            HttpWebRequest webRequest = CreateRequest(path, method, request, data, header);
            string responseText = GetResponseString(webRequest, out HttpStatusCode statusCode);
            switch (statusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.Accepted:
                case HttpStatusCode.NonAuthoritativeInformation:
                case HttpStatusCode.NoContent:
                case HttpStatusCode.PartialContent:
                    return JsonConvert.DeserializeObject<T>(responseText) ?? new T();
                default:
                    string log = GetLogText(path, method, request, data, header, statusCode, responseText);
                    throw new Exception(log);
            }
        }
        public void DoRequest(string path, string method = "GET", object? request = null, object? data = null, object? header = null)
        {
            HttpWebRequest webRequest = CreateRequest(path, method, request, data, header);
            string responseText = GetResponseString(webRequest, out HttpStatusCode statusCode);
            switch (statusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.Accepted:
                case HttpStatusCode.NonAuthoritativeInformation:
                case HttpStatusCode.NoContent:
                case HttpStatusCode.PartialContent:
                    return;
                default:
                    string log = GetLogText(path, method, request, data, header, statusCode, responseText);
                    throw new Exception(log);
            }
        }

        private string GetLogText(string path, string method, object? request, object? data, object? header, HttpStatusCode statusCode, string responseText)
        {
            return 
                $"{method} {path}\n" +
                $"Request: {JsonConvert.SerializeObject(request)}\n" +
                $"Data: {JsonConvert.SerializeObject(data)}\n" +
                $"Header: {JsonConvert.SerializeObject(header)}\n" +
                $"Status: {statusCode} {(int)statusCode}\n" +
                $"Response: {responseText}";
        }
    }
}