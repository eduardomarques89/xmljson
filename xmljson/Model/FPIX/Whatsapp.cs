using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace w7pay.Model.FPIX
{
    public class Whatsapp
    {
        public string clientId { get; set; }
        public string responsavelId { get; set; }
        public string token { get; set; }
        public string email { get; set; }
        public string senha { get; set; }
        public string nome { get; set; }
        public string qrcode { get; set; }
        public string numero { get; set; }
        public string mensagem { get; set; }
        public string status { get; set; }
        public DateTime criacao { get; set; }

        internal string toCreateToken()
        {
            var obj = new
            {
                this.email,
                this.senha,
            };
            return JsonConvert.SerializeObject(obj);
        }

        internal string toCreateMensagem()
        {
            var obj = new
            {
                this.clientId,
                this.numero,
                this.mensagem
            };
            return JsonConvert.SerializeObject(obj);
        }
    }
}