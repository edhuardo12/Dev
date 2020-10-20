using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ePowerCCBApi.ePowerBridge;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using ePowerCCBApi.Models;
using System.Configuration;

namespace ePowerCCBApi.Controllers
{
    [Authorize]
    public class ePowerToSetController : ApiController
    {
        public async Task<IHttpActionResult> Post([FromBody] ePowerToSetModel model)
        {
            ePowerBridge.ServicioEpowerClient _servicioEpower = new ServicioEpowerClient();
            ePowerBridge.ResultadoAccion resultadoAccion = null;
            ePowerBridge.Contenido contenido = null;

            try
            {
                ePowerBridge.Datos[] dataQuery = new ePowerBridge.Datos[model.parametro.Length];

                for (int i = 0; i < model.parametro.Length; i++)
                {
                    ePowerBridge.Datos datoConsulta = new ePowerBridge.Datos();
                    datoConsulta.ValorString = model.parametro[i];
                    dataQuery[i] = datoConsulta;
                }

                ePowerBridge.Datos[] dataInsert = new ePowerBridge.Datos[model.parametroSet.Length];

                for (int i = 0; i < model.parametroSet.Length; i++)
                {
                    ePowerBridge.Datos datoInsercion = new ePowerBridge.Datos();
                    datoInsercion.ValorString = model.parametroSet[i];
                    dataInsert[i] = datoInsercion;
                }

                ePowerBridge.Contenido[] contenidoInsert = new ePowerBridge.Contenido[1];
                contenido = new ePowerBridge.Contenido();

                contenido.BinarioBase64 = model.imagen;
                contenido.TabName = model.carpeta;
                contenido.ExtensionBinario = model.extension;
                contenido.NombreBinario = model.imgname;

                contenidoInsert[0] = contenido;

                var usuario = ConfigurationManager.AppSettings["currentUserName"].ToString();

                var password = ConfigurationManager.AppSettings["currentUserPassword"].ToString();

                //resultadoAccion = await _servicioEpower.SaveDocumentTestAsync(usuario, password, model.applicationId, model.doctypeId, model.queryId, dataQuery, dataInsert, contenidoInsert, model.posicion);

                resultadoAccion = await _servicioEpower.SaveDocumentAsync(usuario, password, model.applicationId, model.doctypeId, model.queryId, dataQuery, dataInsert, contenidoInsert, model.posicion);

                return Ok(resultadoAccion);
            }
            catch (Exception)
            {

                throw;
            }



        }
    }
}
