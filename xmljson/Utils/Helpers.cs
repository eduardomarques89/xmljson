using System.Linq;
using System;
using System.Security.Cryptography.X509Certificates;
using InterAPI.Model;
using System.Collections.Generic;
using InterAPI.ModelRequest;
using System.IO;
using InterAPI.Model.Pix;

namespace InterAPI.Utils
{
    public class Helpers
    {
        public static X509Certificate2 GetCertificate2(byte[] certFile, string password)
        {
            try
            {
                return new X509Certificate2(certFile, password);

            }
            catch
            {
                return null;
            }
        }
        public static string GetOnlyNumbers(string text)
        {
            return new String(text.Where(Char.IsDigit).ToArray());
        }
        public static string ValidarCnpjCpf(string cnpjCpf)
        {
            if (string.IsNullOrEmpty(cnpjCpf))
                return null;

            string formatted = GetOnlyNumbers(cnpjCpf);
            if (formatted.Length != 11 && formatted.Length != 15)
                return null;

            return formatted;
        }

        //public static byte[] GetCert(File file)
        //{
        //    byte[] buff = null;
        //    try
        //    {
        //        {
        //            buff = File.ReadAllBytes(file.FileName);
        //        }
        //    }
        //    catch
        //    {

        //    }
        //    return buff;
        //}

        //public static BoletoGetParametro GetParametros()
        //{
        //    return new BoletoGetParametro
        //    {
        //        cpfCnpj = null,
        //        dataFinal = DateTime.Now.AddDays(10),
        //        dataInicial = DateTime.Now.AddDays(-10),
        //        email = null,
        //        filtrarDataPor = "VENCIMENTO",
        //        nome = null,
        //        situacao = "EMABERTO",
        //        Paginacao = new BoletoGetPaginacao
        //        {
        //            paginaAtual = 0,
        //            itensPorPagina = 20,
        //            ordenarPor = "PAGADOR",
        //            tipoOrdenacao = "ASC"
        //        }
        //    };
        //}

        public static PixGetParametro PixGetParametro()
        {
            return new PixGetParametro
            {
                cnpj = null,
                cpf = null,
                fim = DateTime.Now.AddDays(10),
                inicio = DateTime.Now.AddDays(-10),
                locationPresente = "false",
                loteCobVId = 0,
                status = "ATIVA",
                paginacao = new PixGetParametroPaginacao
                {
                    paginaAtual = 0,
                    itensPorPagina = 10,
                    quantidadeDePaginas = 10,
                    quantidadeTotalDeItens = 10,
                }
            };
        }

        public static Pix GetPix(string txid = null)
        {
            return new Pix
            {
                infoAdicionais = new List<PixInfoAdicional> { new PixInfoAdicional { nome = "teste", valor = "valorTesete" } },
                calendario = new PixCalendario { expiracao = 4000 },
                chave = "19881867000121",
                devedor = new PixDevedor
                {
                    //ou CNPJ ou CPF, nunca os dois
                    cnpj = "",
                    cpf = "37035929827",
                    nome = "Cliente Estabelecimento"
                },
                loc = new Pixloc { tipoCob = "cob" },
                valor = new PixValor
                {
                    original = "100.00",
                    modalidadeAlteracao = 0,
                },
                txid = txid
            };
        }

        public static PixVencimento GetPixVencimento(string txid)
        {
            return new PixVencimento
            {
                txid = txid,
                calendario = new PixVencimentoCalendario
                {
                    dataVencimento = DateTime.Now.ToString("yyyy-MM-dd"),
                    validadeAposVencimento = 30,
                },
                loc = new Pixloc
                {
                    tipoCob = "cobv"
                },
                valor = new PixVencimentoValor
                {
                    original = "100.0",
                    multa = new PixVencVMulJurAbaDesc
                    {
                        modalidade = 0,
                        valorPerc = "1.00"
                    }
                },
                chave = "INFORME SUA CHAVE PIX",
                solicitacaoPagador = "Informe um texto para o pagador aqui",
                infoAdicionais = new List<PixInfoAdicional>
                {
                    new PixInfoAdicional
                    {
                        nome = "nome",
                        valor = "valor"
                    }
                }

            };
        }
    }
}