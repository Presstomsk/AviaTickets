using AviaTickets.Processes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AviaTickets.ViewModel
{
    public class GetRequest
    {

        HttpWebRequest _request;
        string _address;

        public string Response { get; set; }

        public GetRequest(string address)
        {
            _address = address;
        }

        public void Run()
        {
            _request = (HttpWebRequest)WebRequest.Create(_address);
            _request.Method = "Get";            
            HttpWebResponse response = (HttpWebResponse)_request.GetResponse();
            var stream = response.GetResponseStream();
            if (stream != null) Response = new StreamReader(stream).ReadToEnd();
        }
    }

}
