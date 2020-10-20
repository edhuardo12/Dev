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
    public class ServicioEpower : IServicioEpower
    {
        Bitacorizador log = null;

        public ResultadoAccion DocumentSet(string User, string Password, int appId, int doctypeId, int queryId, Datos[] data, Datos[] dataset, Contenido[] contenido, int pos)
        {
            ResultadoAccion resultado = new ResultadoAccion();

            resultado.Resultado = true;
            resultado.Mensaje = "";

            return resultado;
        }

        public ResultadoAccion DocumentSetTest(string User, string Password, int appId, int doctypeId, int queryId, Datos[] data, Datos[] dataset, Contenido[] contenido, int pos)
        {
            ResultadoAccion resultado = new ResultadoAccion();

            resultado.Resultado = true;
            resultado.Mensaje = "";

            return resultado;
        }

        public ResultadoAccion LoginePower(string User, string Password)
        {
            ResultadoAccion resultado = new ResultadoAccion();
            
            resultado = setLoginePower(User, Password);
            
            return resultado;
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
        public List<Resultado> QueryDocument(string User, string Password, int appId, int doctypeId, int queryId, Datos[] data)
        {
            List<Resultado> resultado = new List<Resultado>();
            if (data != null)
            {
                resultado = QueryDocumento(User, Password, appId, doctypeId, queryId, data);
            }
            return resultado;
        }
        public int InsertDocument(string User, string Password, string No_Caso, string Institucion, string Fecha, string No_Correspondencia, string Remitente, string Destinatario, string tab, Contenido Imagen, Contenido Otro_Doc)
        {
            if (Imagen == null)
            {
                if (Otro_Doc == null)
                {
                    log = new Bitacorizador();
                    log.LogError("Error al ejecutar el método: InsertDocument, Detalles " + "Mensaje: " + " La estructura de imágenes y de otros documentos se encuentran vacías");
                    throw new Exception("Error al ejecutar el método: UpdateDocument, Detalles " + "Mensaje: " + " La estructura de imágenes y de otros documentos se encuentran vacías");
                }
                else
                {
                    return InsertaDocumento(User, Password, No_Caso, Institucion, Fecha, No_Correspondencia, Remitente, Destinatario, tab, Otro_Doc.BinarioBase64, Otro_Doc.NombreBinario, Otro_Doc.ExtensionBinario, false);
                }
            }
            else
            {
                return InsertaDocumento(User, Password, No_Caso, Institucion, Fecha, No_Correspondencia, Remitente, Destinatario, tab, Imagen.BinarioBase64, Imagen.NombreBinario, Imagen.ExtensionBinario, true);
            }
        }

        public bool UpdateDocument(string User, string Password, int DocId, string No_Caso, string Institucion, string Fecha, string No_Correspondencia, string Remitente, string Destinatario, string tab, Contenido Imagen, Contenido Otro_Doc)
        {
            if (Imagen == null)
            {
                if (Otro_Doc == null)
                {
                    log = new Bitacorizador();
                    log.LogError("Error al ejecutar el método: UpdateDocument, Detalles " + "Mensaje: " + " La estructura de imágenes y de otros documentos se encuentran vacías");
                    throw new Exception("Error al ejecutar el método: UpdateDocument, Detalles " + "Mensaje: " + " La estructura de imágenes y de otros documentos se encuentran vacías");
                }
                else
                {
                    return ActualizaDocumento(User, Password, DocId, No_Caso, Institucion, Fecha, No_Correspondencia, Remitente, Destinatario, tab, Otro_Doc.BinarioBase64, Otro_Doc.NombreBinario, Otro_Doc.ExtensionBinario, false); 
                }
            }
            else
            {
                return ActualizaDocumento(User, Password, DocId, No_Caso, Institucion, Fecha, No_Correspondencia, Remitente, Destinatario, tab, Imagen.BinarioBase64, Imagen.NombreBinario, Imagen.ExtensionBinario, true);
            }
        }

        public bool DeleteDocument(string User, string Password, int DocId)
        {
            return BorraDocumento(User,  Password, DocId);
        }

        public ResultadoAccion setLoginePower(string User, string Password)
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

        public List<Resultado> QueryDocumentoTest(string User, string Password, int appId, int doctypeId, int queryId, Datos[] data)
        {
            Resultado resultado = new Resultado();
            List<Resultado> lista = new List<Resultado>();
            log = new Bitacorizador();
           
            try
            {
                var resultadoDto = new List<ResultadoDet>();
                var resultadoCdo = new List<ResultadoCont>();
                lista.Add(new Resultado()
                {
                    docId = 12345,
                    ItemsDetalle = resultadoDto,
                    ItemsContenido = resultadoCdo
                });

                        

                resultadoDto.Add(new ResultadoDet
                {
                    campos = "NUMERO DE CUENTA",
                    valores = "507458778"
                });

                resultadoDto.Add(new ResultadoDet
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



                resultadoDto.Add(new ResultadoDet
                {
                    campos = "NUMERO DE CUENTA",
                    valores = "123214512"
                });

                resultadoDto.Add(new ResultadoDet
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

        public List<Resultado> QueryDocumento(string User, string Password, int appId, int doctypeId, int queryId, Datos[] data)
        {
            Resultado resultado = new Resultado();
            List<Resultado> lista = new List<Resultado>();
            log = new Bitacorizador();
            int session = 0;
            string[] fldValue = null;

            Ewsm ePower = null;
            Interfaces.Collection Documento;
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
                    throw new Exception("Error al autenticarse con ePower: Imposible autenticarse con ePower, verifique sus credenciales y conexión a ePower. " + ex.Message);
                }
                log.LogInfo("Se busca el documento");

                fldValue = new string[data.Length];
                for (int i = 0; i < data.Length; i++)
                {
                    fldValue[i] = data[i].ValorString;
                }
                ePWS.WS.Type.QueryInst queryinst = ePower.doExecuteQuery(session, appId, doctypeId, queryId, 500, fldValue, null, null);
                if (queryinst.DocInst != null)
                {
                    
                    for (int j = 0; j < queryinst.DocInst.Length; j++)
                    {
                        int DocId = queryinst.DocInst[j].DocId;


                        var resultadoDto = new List<ResultadoDet>();
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

                            resultadoDto.Add(new ResultadoDet
                            {
                                campos = nombreCampo,
                                valores = valor
                            });
                        }

                        foreach (var item in Documento.References)
                        {
                            if (item.FileName != "")
                            {
                                //string nombreArchivo = item.FileName;
                                string extensionOriginal = item.OriginalExtension;
                                int parentId = item.ParentId;

                                ePWS.WS.Type.Tab[] TabName;
                                TabName = ePower.getTabStruct(session, doctypeId, queryId, DocId);
                                string nombretab = ePower.getTabName(TabName, parentId);

                                if (item.FileName != "")
                                {
                                    string quename = item.QueueName;
                                    string rutacola = "";

                                    string connectionString = Properties.Settings.Default.ConnectionString;
                                    string queryString = "SELECT QUEPATH FROM QUEUE WHERE QUEUENAME = '" + quename + "'";

                                    using (OracleConnection connection = new OracleConnection(connectionString))
                                    {
                                        OracleCommand command = new OracleCommand(queryString, connection);
                                        connection.Open();
                                        OracleDataReader reader = command.ExecuteReader();
                                        try
                                        {
                                            while (reader.Read())
                                            {
                                                rutacola = reader["QUEPATH"].ToString();
                                                string rutaoriginal = Path.Combine(Path.Combine(Path.Combine(Path.Combine(rutacola, item.FileName.Substring(0, 2)), item.FileName.Substring(2, 2)), item.FileName.Substring(4, 2)), item.FileName.Substring(6, 2) + "." + quename);

                                                resultadoCdo.Add(new ResultadoCont
                                                {
                                                    cejilla = nombretab,
                                                    ruta = rutaoriginal
                                                });
                                            }
                                        }
                                        finally
                                        {
                                            reader.Close();
                                        }
                                    }
                                }
                            }
                        }
                    }
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


        public int InsertaDocumento(string User, string Password, string No_Caso, string Institucion, string Fecha, string No_Correspondencia, string Remitente, string Destinatario, string tab, string Archivo, string OriginalName, string OriginalExtension, bool Imagen)
        {
            Interfaces.Collection Documento;
            Comun.gsiStructTab[] Tabs;

            int idcejilla = 1;
            int session = 0;
            Ewsm ePower = null;
            try
            {                
                log = new Bitacorizador();
                log.LogInfo("InsertDocument con los siguientes parámetros:");
                //Cargado de los parametros del config.
                //string epUser = Properties.Settings.Default.epowerUser;
                //string epPass = Properties.Settings.Default.epowerPassword;
                int appId = Properties.Settings.Default.appId;
                int doctype = Properties.Settings.Default.doctype;
                int query = Properties.Settings.Default.query;                
                log.LogInfo("Usuario: " + User +" appId: " + appId + " Doctype: " + doctype + " Query: " + query);
                log.LogInfo("Con los siguientes Indices: ");
                BitacorizaIndices(log, No_Caso, Institucion, Fecha, No_Correspondencia, Remitente, Destinatario);
                log.LogInfo("Con el siguiente Contenido: ");
                log.LogInfo("Nombre Archivo: " + OriginalName);
                log.LogInfo("Extension: " + OriginalExtension);
                ePower = new Ewsm();
                log.LogInfo("Inicia Login hacia ePower");
                try
                { 
                    session = ePower.doLogin(User, Password, "eAccess");
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al autenticarse con ePower: Imposible autenticarse con ePower, verifique sus credenciales y conexión a ePower. " + ex.Message);
                }
                log.LogInfo("Se inicia la creación de un documento");
                Documento = ePower.getNewDocument(session, appId, doctype, query);
                if (!Documento.Equals(null))
                {
                    //Rellena los indices del documento
                    Documento.Fields[0].ValorDec = int.Parse(No_Caso);
                    Documento.Fields[1].ValorString = Institucion;
                    Documento.Fields[2].ValorString = Fecha;
                    Documento.Fields[3].ValorString = No_Correspondencia;
                    Documento.Fields[4].ValorString = Remitente;
                    Documento.Fields[5].ValorString = Destinatario;
                    //Cejillas
                    Tabs = creaCejillas();
                    /***Referencias a imagenes u otros documentos***/
                    log.LogInfo("Insertando Referencias");
                    Comun.gsiStructRef[] referencias = new Comun.gsiStructRef[1];
                    gsiFile gsifile = new gsiFile();
                    gsifile.extension = OriginalExtension;
                    gsifile.Document = System.Convert.FromBase64String(Archivo);
                    if (Imagen)
                    {                        
                        if (tab == "Correspondencia Recibida")
                        {
                            idcejilla = 4;
                        }
                        else if (tab == "Correspondencia Enviada")
                        {
                            idcejilla = 5;
                        }
                        else
                        {
                            idcejilla = 1;//Cejilla de imagenes
                        }
                    }
                    else
                    {
                        idcejilla = 3;//Cejilla de Otros Documentos
                    }
                    gsifile.parentId = idcejilla;
                    Comun.gsiStructRef rf = ePower.saveRef(session, appId, gsifile);
                    referencias[0] = rf;
                    referencias[0].DisplayAlias = OriginalName;
                    referencias[0].OriginalExtension = OriginalExtension;
                    referencias[0].ParentId = idcejilla;
                    log.LogInfo("Salvando Documento");
                    ePower.saveFullDocument(session, appId, doctype, query, Tabs, Documento.Fields, referencias, null, null);
                }
                return Documento.DocId+1;
            }
            catch (Exception ex)
            {
                log = new Bitacorizador();
                log.LogError("Error al ejecutar el método: InsertaArchivo, Detalles " + "Mensaje: " + ex.Message + " StackTrace: " + ex.StackTrace);
                throw new Exception("Error al ejecutar el método: InsertaArchivo, Detalles " + "Mensaje: " + ex.Message + " StackTrace: " + ex.StackTrace);
            }
            finally
            {
                ePower.doLogout(session); 
            } 
        }

        public bool ActualizaDocumento(string User, string Password, int DocId, string No_Caso, string Institucion, string Fecha, string No_Correspondencia, string Remitente, string Destinatario, string tab, string Archivo, string OriginalName, string OriginalExtension, bool Imagen)
        {
            Interfaces.Collection Documento;
            int idcejilla = 1;
            int session = 0;
            Ewsm ePower = null;
            int RefSize = 0;
            int lastRef = 0;
            try
            {
                log = new Bitacorizador();
                log.LogInfo("UpdateDocument con los siguientes parámetros:");
                //Cargado de los parametros del config.
                //string epUser = Properties.Settings.Default.epowerUser;
                //string epPass = Properties.Settings.Default.epowerPassword;
                int appId = Properties.Settings.Default.appId;
                int doctype = Properties.Settings.Default.doctype;
                int query = Properties.Settings.Default.query;
                log.LogInfo("Usuario: " + User + "appId: " + appId + " Doctype: " + doctype + " Query: " + query);
                log.LogInfo("Con los siguientes Indices: ");
                log.LogInfo("DocId: " + DocId);
                BitacorizaIndices(log, No_Caso, Institucion, Fecha, No_Correspondencia, Remitente, Destinatario);
                log.LogInfo("Con el siguiente Contenido: ");
                log.LogInfo("Nombre Archivo: " + OriginalName);
                log.LogInfo("Extension: " + OriginalExtension);
                ePower = new Ewsm();
                log.LogInfo("Inicia Login hacia ePower");
                try
                {
                    session = ePower.doLogin(User, Password, "eAccess");
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al autenticarse con ePower: Imposible autenticarse con ePower, verifique sus credenciales y conexión a ePower. " + ex.Message);
                }
                log.LogInfo("Se inicia la actualización del documento, Recuperando documento");
                Documento = ePower.getFullDocument(session, appId, doctype, query, DocId);
                //Actualiza Indices
                //Rellena los indices del documento
                Documento.Fields[0].ValorDec = int.Parse(No_Caso);
                Documento.Fields[1].ValorString = Institucion;
                Documento.Fields[2].ValorString = Fecha;
                Documento.Fields[3].ValorString = No_Correspondencia;
                Documento.Fields[4].ValorString = Remitente;
                Documento.Fields[5].ValorString = Destinatario;

                //localizacion de la cejilla
                if (Imagen)
                {
                    if (tab == "Correspondencia Recibida")
                    {
                        idcejilla = 4;
                    }
                    else if (tab == "Correspondencia Enviada")
                    {
                        idcejilla = 5;
                    }
                    else
                    {
                        idcejilla = 1;//Cejilla de imagenes
                    }
                }
                else
                {
                    idcejilla = 3;//Cejilla de Otros Documentos
                }
                /***Referencias a imagenes u otros documentos***/
                log.LogInfo("Actualizando Referencias ");
                RefSize = Documento.References.Length + 1;
                lastRef = Documento.References.Length;
                Comun.gsiStructRef[] referencias = new Comun.gsiStructRef[RefSize];
                for (int i = 0; i < Documento.References.Length; i++)
                {
                    referencias[i] = Documento.References[i];
                }
                gsiFile gsifile = new gsiFile();
                gsifile.extension = OriginalExtension;
                gsifile.Document = System.Convert.FromBase64String(Archivo);
                Comun.gsiStructRef rf = Bridge.getInstance().saveRef(session, appId, gsifile);
                referencias[lastRef] = rf;
                referencias[lastRef].DisplayAlias = OriginalName;
                referencias[lastRef].OriginalExtension = OriginalExtension;
                referencias[lastRef].ParentId = idcejilla;
                log.LogInfo("Actualizando Documento");
                ePower.updateFullDocument(session, appId, doctype, query, Documento.DocId, Documento.Tabs, Documento.Fields, referencias, null, null);
                return true;
            }
            catch (Exception ex)
            {
                log = new Bitacorizador();
                log.LogError("Error al ejecutar el método: UpdateDocument, Detalles " + "Mensaje: " + ex.Message + " StackTrace: " + ex.StackTrace);
                throw new Exception("Error al ejecutar el método: UpdateDocument, Detalles " + "Mensaje: " + ex.Message + " StackTrace: " + ex.StackTrace);
            }
            finally
            {
                ePower.doLogout(session);
            }
        }

        public bool BorraDocumento(string User, string Password, int DocId)
        {
            int session = 0;
            Ewsm ePower = null;
            try
            {
                log = new Bitacorizador();
                log.LogInfo("DeleteDocument con los siguientes parámetros:");
                //Cargado de los parametros del config.
                //string epUser = Properties.Settings.Default.epowerUser;
                //string epPass = Properties.Settings.Default.epowerPassword;
                int appId = Properties.Settings.Default.appId;
                int doctype = Properties.Settings.Default.doctype;
                int query = Properties.Settings.Default.query;
                log.LogInfo("Usuario: " + User + "appId: " + appId + " Doctype: " + doctype + " Query: " + query);
                log.LogInfo("Con los siguientes Indices: ");
                log.LogInfo("DocId: " + DocId);
                ePower = new Ewsm();
                log.LogInfo("Inicia Login hacia ePower");
                try
                {
                    session = ePower.doLogin(User, Password, "eAccess");
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al autenticarse con ePower: Imposible autenticarse con ePower, verifique sus credenciales y conexión a ePower. " + ex.Message);
                }
                log.LogInfo("Eliminando Documento...");
                ePower.delFullDocument(session, appId, doctype, query, DocId);
                return true;
            }
            catch (Exception ex)
            {
                log = new Bitacorizador();
                log.LogError("Error al ejecutar el método: DeleteDocument, Detalles " + "Mensaje: " + ex.Message + " StackTrace: " + ex.StackTrace);
                throw new Exception("Error al ejecutar el método: DeleteDocument, Detalles " + "Mensaje: " + ex.Message + " StackTrace: " + ex.StackTrace);
            }
            finally
            {
                ePower.doLogout(session);
            }

        }

        private void BitacorizaIndices(Bitacorizador log, string No_Caso, string Institucion, string Fecha, string No_Correspondencia, string Remitente, string Destinatario)
        {
            log.LogInfo("No_Caso: " + No_Caso);
            log.LogInfo("Institucion: " + Institucion);
            log.LogInfo("Fecha: " + Fecha);
            log.LogInfo("No_Correspondencia " + No_Correspondencia);
            log.LogInfo("Remitente: " + Remitente);
            log.LogInfo("Destinatario: " + Destinatario);
        } 

        private Comun.gsiStructTab[] creaCejillas()
        {            
            Comun.gsiStructTab[] tabs = new Comun.gsiStructTab[5];
            tabs[0] = new Comun.gsiStructTab();
            tabs[0].TabName = "Imagenes";
            tabs[0].TabParentId = 0;
            tabs[0].TabId = 1;

            tabs[1] = new Comun.gsiStructTab();
            tabs[1].TabName = "ERM";
            tabs[1].TabParentId = 0;
            tabs[1].TabId = 2;

            tabs[2] = new Comun.gsiStructTab();
            tabs[2].TabName = "Otros Documentos";
            tabs[2].TabParentId = 0;
            tabs[2].TabId = 3;

            tabs[3] = new Comun.gsiStructTab();
            tabs[3].TabName = "Correspondencia Recibida";
            tabs[3].TabParentId = 1;
            tabs[3].TabId = 4;

            tabs[4] = new Comun.gsiStructTab();
            tabs[4].TabName = "Correspondencia Enviada";
            tabs[4].TabParentId = 1;
            tabs[4].TabId = 5;

            return tabs;
        }
    }
}
