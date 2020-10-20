using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using ePWS.WS.Type;
using System.IO;
using Oracle.ManagedDataAccess.Client;

namespace EpowerWSManagement
{
    public class ClaseColas
    {
        public string queuepath { get; set; }
        public string queuename { get; set; }
    }

    public class ServicioEpower : IServicioEpower
    {
        Bitacorizador log = null;

        public ResultadoAccion LoginePower(string User, string Password)
        {
            ResultadoAccion resultado = new ResultadoAccion();

            resultado = doLoginePower(User, Password);

            return resultado;
        }

        public List<Resultado> QueryDocument(string User, string Password, int appId, int doctypeId, int queryId, Datos[] data)
        {
            List<Resultado> resultado = new List<Resultado>();
            if (data != null)
            {
                log = new Bitacorizador();
                log.LogInfo("Ingrese al metodo de consulta, enviando datos al metodo buscarDocumento");
                resultado = buscarDocumento(User, Password, appId, doctypeId, queryId, data);
            }
            return resultado;
        }

        public ResultadoAccion SaveDocument(string User, string Password, int appId, int doctypeId, int queryId, Datos[] data, Datos[] dataset, Contenido[] contenido, int pos)
        {
            ResultadoAccion resultado = new ResultadoAccion();

            resultado.Resultado = true;
            resultado.Mensaje = "";
            if (data != null)
            {
                resultado = guardarDocumento(User, Password, appId, doctypeId, queryId, data, dataset, contenido, pos);
            }

            return resultado;
        }
        
        public ResultadoAccion doLoginePower(string User, string Password)
        {
            ResultadoAccion resultado = new ResultadoAccion();
            log = new Bitacorizador();
            int session = 0;
            Ewsm ePower = null;
            ePower = null;
            try
            {
                ePower = new Ewsm();
                log.LogInfo("Inicia Login hacia ePower");
                
                session = ePower.doLogin(User, Password, "eAccess");
                resultado.Resultado = true;
                resultado.Mensaje = "";
                return resultado;
            }
            catch (Exception ex1)
            {
                resultado.Resultado = false;
                resultado.Mensaje = "Error al ejecutar el método: LoginePower, Detalles " + "Mensaje: " + ex1.Message + " StackTrace: " + ex1.StackTrace;
                log = new Bitacorizador();
                log.LogError("Error al ejecutar el método: setLoginePower, Detalles " + "Mensaje: " + ex1.Message + " StackTrace: " + ex1.StackTrace);
                return resultado;
                //throw new Exception("Error al ejecutar el método: sLoginePower, Detalles " + "Mensaje: " + ex1.Message + " StackTrace: " + ex1.StackTrace);
            }
            finally
            {
                ePower.doLogout(session);
            }

        }
        
        public List<Resultado> buscarDocumento(string User, string Password, int appId, int doctypeId, int queryId, Datos[] data)
        {
            ClaseColas[] allQueue = null;
            Resultado resultado = new Resultado();
            List<Resultado> lista = new List<Resultado>();
            log = new Bitacorizador();
            int session = 0;
            string[] fldValue = null;

            Ewsm ePower = null;
            Interfaces.Collection Documento;
            ePower = null;
            var list = new List<ClaseColas>();
            try
            {
                
                ePower = new Ewsm();
                log.LogInfo("Inicia Login hacia ePower");
                try
                {
                    session = ePower.doLogin(User, Password, "eAccess");
                }
                catch (Exception ex)
                {
                    log = new Bitacorizador();
                    log.LogError("Error al autenticarse con ePower: Imposible autenticarse con ePower, verifique sus credenciales y conexión a ePower. " + ex.Message);
                    
                }
                log.LogInfo("Se busca el documento");

                string connectionString = Properties.Settings.Default.ConnectionString;
                string queryString = "SELECT QUEPATH, QUEUENAME FROM QUEUE WHERE APPLICATIONID = " + appId;

                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    OracleCommand command = new OracleCommand(queryString, connection);
                    connection.Open();
                    OracleDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                            
                            list.Add(new ClaseColas { queuepath = reader.GetString(0), queuename = reader.GetString(1) });
                            allQueue = list.ToArray();
                            log.LogInfo("Leyendo consulta y guardando en arreglo");
                    }
                    finally
                    {
                        reader.Close();
                        log.LogInfo("Cerrando conexion a base de datos");
                    }
                }
                fldValue = new string[data.Length];
                for (int i = 0; i < data.Length; i++)
                {
                    fldValue[i] = data[i].ValorString;
                }
                ePWS.WS.Type.QueryInst queryinst = ePower.doExecuteQuery(session, appId, doctypeId, queryId, 500, fldValue, null, null);
                if (queryinst.DocInst != null)
                {
                    log.LogInfo("Encontro datos y anidando");
                    for (int j = 0; j < queryinst.DocInst.Length; j++)
                    {
                        int DocId = queryinst.DocInst[j].DocId;

                        var resultadoDto = new List<ResultDetalle>();
                        var resultadoCdo = new List<ResultadoCont>();

                        lista.Add(new Resultado()
                        {
                            docId = DocId,
                            ItemsDetalle = resultadoDto,
                            ItemsContenido = resultadoCdo
                        });

                        Documento = ePower.getFullDocument(session, appId, doctypeId, queryId, DocId);
                        for (int k = 0; k < queryinst.DocInst[j].Fields.Length; k++)
                        {
                            string nombreCampo = queryinst.Column[k].Name;
                            string valor = queryinst.DocInst[j].Fields[k].Value.ToString();

                            resultadoDto.Add(new ResultDetalle
                            {
                                campos = nombreCampo,
                                valores = valor
                            });
                        }

                        foreach (var item in Documento.References)
                        {
                            if (item.FileName != "")
                            {
                                ePWS.WS.Type.Tab[] TabName;
                                TabName = ePower.getTabStruct(session, doctypeId, queryId, DocId);
                                string nombretab = ePower.getTabName(TabName, item.ParentId);

                                if (item.FileName != "")
                                {
                                    string rutacola = "";
                                    foreach (var que in allQueue)
                                    {
                                        if (que.queuename == item.QueueName)
                                        {
                                            rutacola = que.queuepath;
                                            break;
                                        }
                                    }

                                    var nombre_archivo = Path.GetRandomFileName().Replace(".", "x").ToUpper() + "." + item.OriginalExtension;

                                    string rutaoriginal = Path.Combine(Path.Combine(Path.Combine(Path.Combine(rutacola, item.FileName.Substring(0, 2)), item.FileName.Substring(2, 2)), item.FileName.Substring(4, 2)), item.FileName.Substring(6, 2) + "." + item.QueueName);
                                    string rutadestino = Path.Combine(Properties.Settings.Default.rutaDestino, nombre_archivo);
                                    log.LogInfo("ruta " + rutadestino);
                                    log.LogInfo("ruta " + rutaoriginal);
                                    if (File.Exists(rutaoriginal))
                                    {
                                        log.LogInfo("Encontre ruta original... copiando");
                                        File.Copy(rutaoriginal, rutadestino, true);
                                    }
                                    else
                                    {
                                        log.LogInfo("no existe");
                                    }
                                    resultadoCdo.Add(new ResultadoCont
                                    {
                                        cejilla = nombretab,
                                        ruta = nombre_archivo,
                                        etiqueta = item.DisplayAlias
                                    });
                                }
                            }
                        }
                    }
                }
                else 
                {
                    log.LogInfo("No encontre registros... saliendo metodo consulta");
                }
                return lista;
            }
            catch (Exception ex)
            {
                log = new Bitacorizador();
                log.LogError("Error al ejecutar el método: QueryDocumento, Detalles " + "Mensaje: " + ex.Message + " StackTrace: " + ex.StackTrace);
                throw new Exception("Error al ejecutar el método: QueryDocumento, Detalles " + "Mensaje: " + ex.Message + " StackTrace: " + ex.StackTrace);
                
            }
            finally
            {
                ePower.doLogout(session);
            }
        }

        public ResultadoAccion guardarDocumento(string User, string Password, int appId, int doctypeId, int queryId, Datos[] data, Datos[] dataset, Contenido[] contenido, int pos)
        {
            ResultadoAccion resultado = new ResultadoAccion();
            resultado.Resultado = true;
            resultado.Mensaje = "";
            log = new Bitacorizador();
            int session = 0;
            string[] fldValue = null;

            Ewsm ePower = null;
            //Interfaces.Collection Documento;
            ePower = null;
            try
            {
                log.LogInfo("InsertDocument con los siguientes parámetros:");
                log.LogInfo("Con los siguientes Indices: ");
                ePower = new Ewsm();
                log.LogInfo("Inicia Login hacia ePower");
                try
                {
                    session = ePower.doLogin(User, Password, "eAccess");
                }
                catch (Exception ex)
                {
                    log.LogError("Error al autenticarse con ePower: Imposible autenticarse con ePower, verifique sus credenciales y conexión a ePower. " + ex.Message);
                    resultado.Resultado = false;
                    resultado.Mensaje = "Error al autenticarse con ePower: Imposible autenticarse con ePower, verifique sus credenciales y conexión a ePower. " + ex.Message;
                }
                log.LogInfo("Se busca el documento");

                fldValue = new string[data.Length];
                for (int i = 0; i < data.Length; i++)
                {
                    fldValue[i] = data[i].ValorString;
                }
                ePWS.WS.Type.QueryInst queryinst = ePower.doExecuteQuery(session, appId, doctypeId, queryId, 500, fldValue, null, null);
                if (queryinst.DocInst != null)
                {       // update
                    for (int j = 0; j < queryinst.DocInst.Length; j++)
                    {
                        int DocId = queryinst.DocInst[j].DocId;
                        resultado = actualizaDocumento(User, Password, appId, doctypeId, queryId, DocId, dataset, contenido, pos);
                    }
                }
                else    // insert
                {
                    resultado = insertaDocumento(User, Password, appId, doctypeId, queryId, dataset, contenido);
                }

                return resultado;
            }
            catch (Exception ex)
            {
                log = new Bitacorizador();
                log.LogError("Error al ejecutar el método: GuardarDocumento, Detalles " + "Mensaje: " + ex.Message + " StackTrace: " + ex.StackTrace);
                resultado.Resultado = false;
                resultado.Mensaje = "Error al ejecutar el método: GuardarDocumento, Detalles " + "Mensaje: " + ex.Message + " StackTrace: " + ex.StackTrace;
                return resultado;
            }
            finally
            {
                ePower.doLogout(session);
            }
        }

        public ResultadoAccion insertaDocumento(string User, string Password, int appId, int doctypeId, int queryId, Datos[] data, Contenido[] content)
        {
            ResultadoAccion resultado = new ResultadoAccion();
            resultado.Resultado = true;
            resultado.Mensaje = "";

            Interfaces.Collection Documento;


            int idcejilla = 1;
            int session = 0;
            Ewsm ePower = null;
            try
            {
                log = new Bitacorizador();
                log.LogInfo("InsertDocument con los siguientes parámetros:");

                log.LogInfo("Con los siguientes Indices: ");

                ePower = new Ewsm();
                log.LogInfo("Inicia Login hacia ePower");
                try
                {
                    session = ePower.doLogin(User, Password, "eAccess");
                }
                catch (Exception ex)
                {
                    resultado.Resultado = false;
                    resultado.Mensaje = "Error al autenticarse con ePower: Imposible autenticarse con ePower, verifique sus credenciales y conexión a ePower. " + ex.Message;
                }
                log.LogInfo("Se inicia la creación de un documento");
                Documento = ePower.getNewDocument(session, appId, doctypeId, queryId);
                if (!Documento.Equals(null))
                {
                    //Rellena los indices del documento
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (Documento.Fields[i].Type == 1)
                        {
                            Documento.Fields[i].ValorString = data[i].ValorString.ToString();
                        }
                        else if (Documento.Fields[i].Type == 2)
                        {
                            Documento.Fields[i].ValorDec = double.Parse(data[i].ValorString.ToString());
                        }                        
                    }

                    //Cejillas
                    
                    Comun.gsiStructTab[] tabsTemplete;
                    tabsTemplete =  ePower.getTabTempleteStruct(session, appId, doctypeId);
                    Comun.gsiStructTab[] tabs = new Comun.gsiStructTab[tabsTemplete.Length + 2];

                    int tabId = 0;
                    int k = 0;
                    for (int i =0; i < tabsTemplete.Length; i++)
                    {
                        tabs[i] = new Comun.gsiStructTab();
                        tabs[i].TabName = tabsTemplete[i].TabName;
                        tabs[i].TabParentId = tabsTemplete[i].TabParentId;
                        tabs[i].TabId = tabsTemplete[i].TabId;
                        tabs[i].PreviousTabId = tabsTemplete[i].PreviousTabId;
                        tabId = tabsTemplete[i].TabId;
                        k = i;
                    }

                    tabs[k + 1] = new Comun.gsiStructTab();
                    tabs[k + 1].TabName = "ERM";
                    tabs[k + 1].TabParentId = 0;
                    tabs[k + 1].TabId = tabId + 1;
                    tabs[k + 1].PreviousTabId = 1;

                    tabs[k + 2] = new Comun.gsiStructTab();
                    tabs[k + 2].TabName = "Otros Documentos";
                    tabs[k + 2].TabParentId = 0;
                    tabs[k + 2].TabId = tabId + 2;
                    tabs[k + 2].PreviousTabId = tabId + 1;

                    /***Referencias a imagenes u otros documentos***/
                    log.LogInfo("Insertando Referencias");

                    Comun.gsiStructRef[] referencias = new Comun.gsiStructRef[content.Length];                    

                    for (int i = 0; i < content.Length; i++)
                    {                       

                        idcejilla = 0;
                        for (int j = 0; j < tabs.Length; j++)
                        {
                            if (tabs[j].TabName.ToUpper() == content[i].TabName.ToUpper())
                            {
                                idcejilla = tabs[j].TabId;
                                break;
                            }
                            log.LogInfo("Imprimiendo " + tabs[j].TabName.ToUpper());
                        }
                        if (idcejilla == 0)
                        {
                            idcejilla = 1;
                        }

                        gsiFile gsifile = new gsiFile();
                        gsifile.extension = content[i].ExtensionBinario;
                        log.LogInfo("Informacion agregada al componente gsifile: extension " + gsifile.extension.ToString());
                        gsifile.Document = System.Convert.FromBase64String(content[i].BinarioBase64);
                        gsifile.parentId = idcejilla;
                        log.LogInfo("Informacion agregada al componente gsifile: parentID " + gsifile.parentId.ToString());
                        log.LogInfo("Inicia funcion saveRef EWSM");
                        log.LogInfo("Se envia el siguiente appID " + appId.ToString());
                        Comun.gsiStructRef rf1 = ePower.saveRef(session, appId, gsifile);

                        referencias[i] = rf1;
                        referencias[i].DisplayAlias = content[i].NombreBinario;
                        referencias[i].OriginalExtension = content[i].ExtensionBinario;
                        referencias[i].ParentId = idcejilla;// content[i].IdTab;
                    }

                    log.LogInfo("Salvando Documento");
                    log.LogInfo("Enviando " + session.ToString() + " " + appId.ToString() + " " + doctypeId.ToString() + " " + queryId.ToString() + " " + idcejilla.ToString());
                    ePower.saveFullDocument(session, appId, doctypeId, queryId, tabs, Documento.Fields, referencias, null, null);
                }
                return resultado;
            }
            catch (Exception ex)
            {
                log = new Bitacorizador();
                log.LogError("Error al ejecutar el método: InsertaArchivo, Detalles " + "Mensaje: " + ex.Message + " StackTrace: " + ex.StackTrace);
                resultado.Resultado = false;
                resultado.Mensaje = "Error al ejecutar el método: GuardarDocumento, Detalles " + "Mensaje: " + ex.Message + " StackTrace: " + ex.StackTrace;
                return resultado;
            }
            finally
            {
                ePower.doLogout(session);
            }
        }
        
        public ResultadoAccion actualizaDocumento(string User, string Password, int appId, int doctypeId, int queryId, int DocId, Datos[] data, Contenido[] content, int pos)
        {
            ResultadoAccion resultado = new ResultadoAccion();
            resultado.Resultado = true;
            resultado.Mensaje = "";
            Interfaces.Collection Documento;
            int session = 0;
            Ewsm ePower = null;
            int RefSize = 0;
            int lastRef = 0;
            int idcejilla = 1;
            try
            {
                log = new Bitacorizador();
                log.LogInfo("UpdateDocument con los siguientes parámetros:");
               
                log.LogInfo("Con los siguientes Indices: ");
                log.LogInfo("DocId: " + DocId);
                
                log.LogInfo("Con el siguiente Contenido: ");
                ePower = new Ewsm();
                log.LogInfo("Inicia Login hacia ePower");
                try
                {
                    session = ePower.doLogin(User, Password, "eAccess");
                }
                catch (Exception ex)
                {
                    resultado.Resultado = false;
                    resultado.Mensaje = "Error al autenticarse con ePower: Imposible autenticarse con ePower, verifique sus credenciales y conexión a ePower. " + ex.Message;
                }
                log.LogInfo("Se inicia la actualización del documento, Recuperando documento");
                Documento = ePower.getFullDocument(session, appId, doctypeId, queryId, DocId);
                //Actualiza Indices
                //Rellena los indices del documento
                for (int i = 0; i < data.Length; i++)
                {
                    if (Documento.Fields[i].Type == 1)
                    {
                        Documento.Fields[i].ValorString = data[i].ValorString.ToString();
                    }
                    else if (Documento.Fields[i].Type == 2)
                    {
                        Documento.Fields[i].ValorDec = double.Parse(data[i].ValorString.ToString());
                    }
                }

                ePWS.WS.Type.Tab[] TabName;
                TabName = ePower.getTabStruct(session, doctypeId, queryId, DocId);                

                /***Referencias a imagenes u otros documentos***/
                log.LogInfo("Actualizando Referencias ");

                RefSize = Documento.References.Length + content.Length;
                lastRef = Documento.References.Length;
                Comun.gsiStructRef[] referencias = new Comun.gsiStructRef[RefSize];
                for (int i = 0; i < Documento.References.Length; i++)
                {
                    referencias[i] = Documento.References[i];
                }

                for (int j = 0; j < content.Length; j++)
                {
                    idcejilla = ePower.getTabId(TabName, content[j].TabName.ToUpper());                   

                    gsiFile gsifile1 = new gsiFile();
                    gsifile1.extension = content[j].ExtensionBinario;
                    log.LogInfo("Se ha capturado el siguiente parametro para gsifile1: extension " + gsifile1.extension.ToString());
                    gsifile1.Document = System.Convert.FromBase64String(content[j].BinarioBase64);
                    gsifile1.parentId = idcejilla;
                    log.LogInfo("Se ha capturado el siguiente parametro para gsifile1: parentID " + gsifile1.parentId.ToString());
                    log.LogInfo("Inicia funcion saveRef EWSM");
                    log.LogInfo("Paramentros de envio para appID " + appId.ToString());
                    Comun.gsiStructRef rf1 = ePower.saveRef(session, appId, gsifile1);

                    referencias[lastRef] = rf1;
                    referencias[lastRef].DisplayAlias = content[j].NombreBinario;
                    referencias[lastRef].OriginalExtension = content[j].ExtensionBinario;
                    referencias[lastRef].ParentId = idcejilla;
                    lastRef++;
                }
                log.LogInfo("Actualizando Documento");
                ePower.updateFullDocument(session, appId, doctypeId, queryId, Documento.DocId, Documento.Tabs, Documento.Fields, referencias, null, null);
                return resultado;
            }
            catch (Exception ex)
            {
                log = new Bitacorizador();
                log.LogError("Error al ejecutar el método: UpdateDocument, Detalles " + "Mensaje: " + ex.Message + " StackTrace: " + ex.StackTrace);
                resultado.Resultado = false;
                resultado.Mensaje = "Error al ejecutar el método: UpdateDocument, Detalles " + "Mensaje: " + ex.Message + " StackTrace: " + ex.StackTrace;
                return resultado;
            }
            finally
            {
                ePower.doLogout(session);
            }
        }

        public ResultadoAccion LoginePowerTest(string User, string Password)
        {
            ResultadoAccion resultado = new ResultadoAccion();

            if (Password == "admin")
            {
                resultado.Resultado = true;
                resultado.Mensaje = "";
            }
            else
            {
                resultado.Resultado = false;
                resultado.Mensaje = "Error 1";
            }

            return resultado;
        }

        public List<Resultado> QueryDocumentTest(string User, string Password, int appId, int doctypeId, int queryId, Datos[] data)
        {
            List<Resultado> resultado = new List<Resultado>();
            if (data != null)
            {
                resultado = QueryDocumentoTest(User, Password, appId, doctypeId, queryId, data);
            }
            return resultado;
        }

        public List<Resultado> QueryDocumentoTest(string User, string Password, int appId, int doctypeId, int queryId, Datos[] data)
        {
            Resultado resultado = new Resultado();
            List<Resultado> lista = new List<Resultado>();
            log = new Bitacorizador();

            try
            {
                var resultadoDto = new List<ResultDetalle>();
                var resultadoCdo = new List<ResultadoCont>();
                lista.Add(new Resultado()
                {
                    docId = 12345,
                    ItemsDetalle = resultadoDto,
                    ItemsContenido = resultadoCdo
                });



                resultadoDto.Add(new ResultDetalle
                {
                    campos = "NUMERO DE CUENTA",
                    valores = "507458778"
                });

                resultadoDto.Add(new ResultDetalle
                {
                    campos = "NUMERO DE CHEQUE",
                    valores = "1254"
                });

                resultadoCdo.Add(new ResultadoCont
                {
                    cejilla = "CEJILLA 1",
                    ruta = @"C:\IMAGENES\IMAGEN1.JPG"
                });

                resultadoCdo.Add(new ResultadoCont
                {
                    cejilla = "CEJILLA 1",
                    ruta = @"C:\IMAGENES\IMAGEN2.JPG"
                });

                resultadoCdo.Add(new ResultadoCont
                {
                    cejilla = "CEJILLA 2",
                    ruta = @"C:\IMAGENES\IMAGEN3.JPG"
                });
                resultadoCdo.Add(new ResultadoCont
                {
                    cejilla = "CEJILLA 4",
                    ruta = @"C:\IMAGENES\IMAGEN4.JPG"
                });

                lista.Add(new Resultado()
                {
                    docId = 67890,
                    ItemsDetalle = resultadoDto,
                    ItemsContenido = resultadoCdo
                });



                resultadoDto.Add(new ResultDetalle
                {
                    campos = "NUMERO DE CUENTA",
                    valores = "123214512"
                });

                resultadoDto.Add(new ResultDetalle
                {
                    campos = "NUMERO DE CHEQUE",
                    valores = "5211"
                });

                resultadoCdo.Add(new ResultadoCont
                {
                    cejilla = "CEJILLA 1",
                    ruta = @"C:\IMAGENES\IMAGEN5.JPG"
                });

                resultadoCdo.Add(new ResultadoCont
                {
                    cejilla = "CEJILLA 2",
                    ruta = @"C:\IMAGENES\IMAGEN7.JPG"
                });
                resultadoCdo.Add(new ResultadoCont
                {
                    cejilla = "CEJILLA 10",
                    ruta = @"C:\IMAGENES\IMAGEN8.JPG"
                });



                return lista;
            }
            catch (Exception ex)
            {
                log = new Bitacorizador();
                log.LogError("Error al ejecutar el método: QueryDocumento, Detalles " + "Mensaje: " + ex.Message + " StackTrace: " + ex.StackTrace);
                throw new Exception("Error al ejecutar el método: QueryDocumento, Detalles " + "Mensaje: " + ex.Message + " StackTrace: " + ex.StackTrace);
            }


        }

        public ResultadoAccion SaveDocumentTest(string User, string Password, int appId, int doctypeId, int queryId, Datos[] data, Datos[] dataset, Contenido[] contenido, int pos)
        {
            ResultadoAccion resultado = new ResultadoAccion();

            resultado.Resultado = true;
            resultado.Mensaje = "";


            return resultado;
        }

    }
}
