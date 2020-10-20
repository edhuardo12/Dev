using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using ePowerCCBApi.ePowerBridge;
using System.Web.Http;
using ePowerCCBApi.Models;
using System.Threading.Tasks;
using System.Configuration;
using System.Net.Http.Headers;

namespace ePowerCCBApi.Controllers
{
    public class ePowerGetInfoController : ApiController
    {
        [Authorize]
        public async Task<IHttpActionResult> Post([FromBody] ePowerGetInfoModels model)
        {
            ePowerBridge.ServicioEpowerClient _servicioEpower = new ServicioEpowerClient();

            try
            {
                ePowerBridge.Datos[] dataQuery = new ePowerBridge.Datos[model.parametros.Length];

                for (int i = 0; i < model.parametros.Length; i++)
                {
                    ePowerBridge.Datos dato = new ePowerBridge.Datos();
                    dato.ValorString = model.parametros[i];
                    dataQuery[i] = dato;
                }

                var usuario = ConfigurationManager.AppSettings["currentUserName"].ToString();

                var password = ConfigurationManager.AppSettings["currentUserPassword"].ToString();

                //Sentencia para UAT/DEV

                /*List<ePowerBridge.Resultado> resultados = 
                    new List<Resultado>(await _servicioEpower.QueryDocumentTestAsync(usuario, password, model.applicationId, model.doctypeId, model.queryId, dataQuery));*/

                List<ePowerBridge.Resultado> resultados = new
                    List<Resultado>(await _servicioEpower.QueryDocumentAsync(usuario, password, model.applicationId, model.doctypeId, model.queryId, dataQuery));

                return Ok(resultados);

            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }

        }
    }
}
