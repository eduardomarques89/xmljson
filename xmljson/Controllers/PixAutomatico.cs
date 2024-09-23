using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Xml;
using InterAPI.Model.Pix;
using InterAPI.Service;
using System.Drawing;
using TEDIE.Service;
using InterAPI.Model;

namespace PixAutomatico.Controllers
{
    [ApiController]
    [Route("PixAutomatico")]
    public class PixAutomatico : Controller
    {
        private readonly ILogger<PixAutomatico> _logger;

        public PixAutomatico(ILogger<PixAutomatico> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "PixAutomatico")]
        public IEnumerable<Pix> Get(string valor)
        {
            AccessToken tk = new AccessToken();
            tk = SOAuth.GetToken(null);
            var token = tk.Access_token;

            string tipopix = "";

            Pix dadospix = new Pix();
            dadospix.chave = "19881867000121";
            dadospix.solicitacaoPagador = "Pagamento compra realizado no aplicativo TEDIE";

            PixValor pixvalor = new PixValor();
            pixvalor.modalidadeAlteracao = 1;
            pixvalor.original = Convert.ToDecimal(valor).ToString();

            PixDevedor pixdev = new PixDevedor();

            pixdev.cpf = "37035929827";//cpf do administrador
            tipopix = "cpf";
            pixdev.nome = "Cliente Consumidor";

            PixCalendario pixcalend = new PixCalendario();
            pixcalend.criacao = DateTime.Now;
            pixcalend.expiracao = 3600;

            PixInfoAdicional pixinfo = new PixInfoAdicional();
            pixinfo.nome = "Pagamento de compra realizada  no aplicativo TEDIE";
            pixinfo.valor = valor;

            Pixloc pixloc = new Pixloc();
            pixloc.tipoCob = "cob";

            dadospix.devedor = pixdev;
            dadospix.loc = pixloc;
            //dadospix.infoAdicionais[0] = pixinfo;
            dadospix.valor = pixvalor;
            dadospix.calendario = pixcalend;

            //dadospix.infoAdicionais[0].nome = "Pagamento do serviço via plataforma W7 Pay";
            //dadospix.infoAdicionais[0].valor = "1";

            Pix retornopix = new Pix();

            //var certificates = new X509Certificate2();
            //X509KeyStorageFlags fleg = new X509KeyStorageFlags();
            //certificates.Import(PathCert, PassCert, fleg);

            retornopix = SPixImediato.Post(dadospix, "71824669", token.ToString(), tipopix);
            var status = retornopix.status;
            var txid = retornopix.txid;
            var copiacola = retornopix.pixCopiaECola;


            return Enumerable.Range(1, 1).Select(index => new Pix { imagem = "https://gerarqrcodepix.com.br/api/v1?brcode=" +copiacola +"&tamanho=256" });
        }
    }
}
