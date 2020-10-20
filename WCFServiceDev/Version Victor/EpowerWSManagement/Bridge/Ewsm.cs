#define Query
#define Update

using System;
using System.Collections;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Configuration;
using System.IO;
using ePWS.SessionBean;
using ePWS.ConditionBuilder;


namespace EpowerWSManagement
{
    public class Ewsm
    {
        private ePWS.epCorbaTransmisor epTransmisor;

        /// <summary>
        /// Contructor por defecto
        /// </summary>
        public Ewsm()
        {
            //InitializeComponent();

            Global.wsCntURL = Properties.Settings.Default.ePowerIOR;
            Global.wsTimeOut = Properties.Settings.Default.WS_TimeOut.ToString();
            Global.packSize = Properties.Settings.Default.WS_Packet_Size;
            Global.wsRefHTTP = Properties.Settings.Default.epowerReferenceHTTP;
            Global.wsRefUNC = Properties.Settings.Default.epowerReferenceUNC;
            Global.epRef = Properties.Settings.Default.epowerReference;

            Global.sessions = new ArrayList();

            this.epTransmisor = null;

        }

        #region Component Designer generated code

        /// <summary>
        /// Requirido por el diseñador del Web Services
        /// </summary>
        //private IContainer components = null;

        /// <summary>
        /// Metodo requerido para inicializar los componentes del Web Service
        /// </summary>
        /*private void InitializeComponent()
		{
		}*/

        /// <summary>
        /// Limpia los recursos usados.
        /// </summary>
        /*protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);		
		}*/

        #endregion

        #region Utiles
        /// <summary>
        /// Metodo que busca y retorna la sesion por el Id.
        /// </summary>
        /// <param name="uid">Id del usuario dueño de la sesion</param>
        /// <returns>Sesion</returns>
        private epSession getSession(int uid)
        {
            lock (Global.sessions)
            {
                for (int i = 0; i < Global.sessions.Count; i++)
                {
                    epSession actual = (epSession)Global.sessions[i];
                    if (actual.Id == uid)
                        return actual;
                }
            }
            return null;
        }
        /// <summary>
        /// Metodo que elimina una sesion por Id
        /// </summary>
        /// <param name="uid">Id del usuario dueño de la sesion</param>
        private void delSession(int uid)
        {
            lock (Global.sessions)
            {
                for (int i = 0; i < Global.sessions.Count; i++)
                {
                    epSession actual = (epSession)Global.sessions[i];
                    if (actual.Id == uid)
                        Global.sessions.RemoveAt(i);
                }
            }
        }
        /// <summary>
        /// Evento disparado cuando se cumplio el tiempo limite de inactividad.
        /// </summary>
        /// <param name="e">Sesion que disparo el evento</param>
        /// <param name="Message">Mensaje informativo</param>
        private void newSession_closeByInactivity(epSession e, string Message)
        {
            e.logout();
            Global.sessions.Remove(e);
        }
        #endregion

        #region Funciones de Autentificacion
        /// <summary>
        /// Metodo de autentificacion de usuario para crear una sesion
        /// </summary>
        /// <param name="user">Nombre de usuario</param>
        /// <param name="password">Clave de usuario</param>
        /// <returns>Identificacion de la sesion</returns>
        /// <example>
        /// <code>
        /// [C#]
        /// string user = "GSI";     //usuario de ePower
        /// string password = "GSI"; //password del usuario anterior
        /// 
        /// ewsm service = new ewsm();
        /// int uid = service.doLogin(user,password);
        /// if(uid > 0)
        ///		MessageBox.Show("Auntentificación exitosa: Id = " + uid);
        ///	else
        ///		MessageBox.Show("Auntentificación fallida");
        /// </code>
        /// <seealso cref="doLogout"/>
        /// </example>
        //[WebMethod(Description="Obtiene Lista de Aplicaciones",EnableSession=true)]
        public int doLogin(string user, string password, string module)
        {
            try
            {
                this.epTransmisor = ePWS.epCorbaTransmisor.newCorbaTransmisor();
                epSession newSession = new epSession(user, password, Global.wsCntURL, Global.epRef, Global.wsRefUNC, Global.wsRefHTTP, null,
                    /*HttpContext.Current.Request.PhysicalApplicationPath,*/ this.epTransmisor, Global.packSize);

                newSession.closeByInactivity += new epSession.inactivity(newSession_closeByInactivity);
                if (newSession.login(int.Parse(Global.wsTimeOut), module))
                {
                    Global.sessions.Add(newSession);
                    return newSession.Id;
                }
                else
                    return -1;
            }
            catch (Exception e)
            {
                throw e;
            }

            //return -1;
        }
        /// <summary>
        /// Procedimiento que finaliza la sesion del usuario
        /// </summary>
        /// <param name="sessionId">Id de la sesion que se desea cerrar</param>
        /// <returns>Resultado de la operacion</returns>
        /// <example>
        /// <code>
        /// [C#]
        /// string user = "GSI";     //usuario de ePower
        /// string password = "GSI"; //password del usuario anterior
        /// 
        /// ewsm service = new ewsm();
        /// int uid = service.doLogin(user,password);
        /// if(uid > 0)
        ///		service.doLogout(uid);
        /// </code>
        /// <seealso cref="doLogin"/>
        /// </example>
        //[WebMethod(Description="Obtiene Lista de Aplicaciones",EnableSession=true)]
        public bool doLogout(int sessionId)
        {
            lock (sessionId.ToString())
            {
                bool result = false;
                epSession actual = getSession(sessionId);
                if (actual != null)
                {
                    result = actual.logout();
                    delSession(sessionId);
                }
                return result;
            }
        }

        /// <summary>
        /// Procedimiento que cierra el canal de corba
        /// </summary>
        public void disconnectServer()
        {
            this.epTransmisor.disconnectFromServer();
        }

        #endregion
#if Query
        #region Funciones de obtencion Basicas
        /// <summary>
        ///	Procedimiento que retorna la lista de aplicaciones
        /// </summary>
        /// <param name="sessionId">Id de la sesion en la cual se esta trabajando</param>
        /// <returns>Estrutura de aplicaciones</returns>
        /// <example>
        /// <code>
        /// [C#]
        /// 
        /// //Ejemplo: Llena un TreeView con las aplicaciones
        /// //Autor: Grupo de Soluciones Informáticas
        /// 
        /// string user = "GSI";     //usuario de ePower
        /// string password = "GSI"; //password del usuario anterior
        /// 
        /// ewsm service = new ewsm();
        /// int uid = service.doLogin(user,password);
        /// if(uid > 0)
        /// {
        ///		App[] apps = service.getAppList(uid);	//Lista de aplicaciones
        ///		for(int i = 0 ; i &lt; apps.Length ; i++)
        ///		{
        ///			TreeNode tmp = new TreeNode(apps[i].AppName);
        ///			tmp.Tag = apps[i].AppId
        ///			_tree.Nodes.Add(tmp);				//_tree es una instancia de un TreeView
        ///		}
        /// }
        /// </code>
        /// </example>
        //[WebMethod(Description="Obtiene Lista de Aplicaciones",EnableSession=true)]
        public global::Session.App[] getAppList(int sessionId)
        {
            lock (sessionId.ToString())
            {
                epSession actual = getSession(sessionId);

                if (actual != null)
                    return actual.getApps();
                else
                    return null;
            }
        }
        /// <summary>
        ///	Procedimiento que retorna la lista de tipos de documento de una aplicación
        /// </summary>
        /// <param name="sessionId">Id de la sesion en la cual se esta trabajando</param>
        /// <param name="appId">Id de la aplicación de la cual se desea extraer los tipos de documentos</param>
        /// <returns>Estructura de tipos de documentos</returns>
        /// <example>
        /// <code>
        /// [C#]
        /// 
        /// //Ejemplo: Llena un TreeView con las aplicaciones y los tipos de documentos
        /// //Autor: Grupo de Soluciones Informáticas
        /// 
        /// string user = "GSI";     //usuario de ePower
        /// string password = "GSI"; //password del usuario anterior
        /// 
        /// ewsm service = new ewsm();
        /// int uid = service.doLogin(user,password);
        /// if(uid > 0)
        /// {
        ///		App[] apps = service.getAppList(uid);	//Lista de aplicaciones
        ///		for(int i = 0 ; i &lt; apps.Length ; i++)
        ///		{
        ///			TreeNode appTmp = new TreeNode(apps[i].AppName);
        ///			appTmp.Tag = apps[i].AppId
        ///			DocType[] dts = service.getDocTypeList(uid,apps[i].AppId);
        ///			for(int j = 0 ; j &lt; dts.Length; j++)
        ///			{
        ///				TreeNode docTmp = new TreeNode(dts[j].DocTypeName);
        ///				docTmp.Tag = dts[j].DocTypeId;
        ///				appTmp.Nodes.Add(docTmp);
        ///			}
        ///			_tree.Nodes.Add(appTmp);			//_tree es una instancia de un TreeView
        ///		}
        /// }
        /// </code>
        /// <seealso cref="getAppList"/>
        /// </example>
        //[WebMethod(Description="Obtiene Lista de Tipos de Documento",EnableSession=true)]
        public global::Session.DocType[] getDocTypeList(int sessionId, int appId)
        {
            lock (sessionId.ToString())
            {
                epSession actual = getSession(sessionId);

                if (actual != null)
                    return actual.getDocTypes(appId);
                else
                    return null;
            }
        }

        /// <summary>
        ///	Procedimiento que retorna la lista de documentos asociados de una aplicación
        /// </summary>
        /// <param name="sessionId">Id de la sesion en la cual se esta trabajando</param>
        /// <param name="appId">Id de la aplicacion a la que pertenece el tipo de documento del cual se desea adquirir los documentos asociados</param>
        /// <param name="docTypeId">Id del tipo de documento del que se desea extraer los documentos asociados</param>
        /// <returns>Estructura de SoftRef que contiene los documentos asociados</returns>
        /// <example>
        /// <code>
        /// [C#]
        /// 
        /// //Ejemplo:	Llena un TreeView con las aplicaciones y los tipos de documentos
        ///	//			y al seleccionar cada Tipo de Documento te muestra los 
        ///	//			documentos asociados
        /// //Autor:	Grupo de Soluciones Informáticas
        /// 
        /// int uid;
        /// TreeView _tree;
        /// 
        /// ctor()
        /// {
        ///		uid = -1;
        ///		_tree = new TreeView();
        ///		_tree.AfterSelect += new TreeViewEventHandler(TreeView1_AfterSelect);
        ///		_tree.PathSeparator = ".";
        ///		llenarTreeView();
        /// }
        /// 
        /// public void llenarTreeView()
        /// {
        ///		string user = "GSI";     //usuario de ePower
        ///		string password = "GSI"; //password del usuario anterior
        /// 
        ///		ewsm service = new ewsm();
        ///		uid = service.doLogin(user,password);
        ///		if(uid > 0)
        ///		{
        ///			App[] apps = service.getAppList(uid);	//Lista de aplicaciones
        ///			for(int i = 0 ; i &lt; apps.Length ; i++)
        ///			{
        ///				TreeNode appTmp = new TreeNode(apps[i].AppName);
        ///				appTmp.Tag = apps[i].AppId
        ///				DocType[] dts = service.getDocTypeList(uid,apps[i].AppId);
        ///				for(int j = 0 ; j &lt; dts.Length; j++)
        ///				{
        ///					TreeNode docTmp = new TreeNode(dts[j].DocTypeName);
        ///					docTmp.Tag = dts[j].DocTypeId;
        ///					appTmp.Nodes.Add(docTmp);
        ///				}
        ///				_tree.Nodes.Add(appTmp);			//_tree es una instancia de un TreeView
        ///			}
        ///		}
        ///	}
        ///	
        ///	private void TreeView1_AfterSelect(System.Object sender, 
        ///		System.Windows.Forms.TreeViewEventArgs e)
        ///	{
        ///		TreeNode tmp = (TreeNode) sender;
        ///		
        ///		if(tmp.Parent != null)  //Las aplicaciones no tienen padre 
        ///		{						//pero los tipos de documentos si
        ///			int app = tmp.Parent.Tag;
        ///			int docType = tmp.Tag;
        ///			
        ///			SoftRef[] docAsoc = service.getAsocDocQueryList(uid, app, docType);
        ///			string asociaciones = "";
        ///			for(int i = 0 ; i &lt; docAsoc.Length ; i++)
        ///			{
        ///				asociaciones = docAsoc[i].AssocName + "\n";
        ///			}
        ///			MessageBox.Show(asociaciones);
        ///		}
        ///	}
        /// </code>
        /// <seealso cref="getDocTypeList"/>
        /// </example>
        //[WebMethod(Description="Obtiene Lista de Documentos Asociados de una Aplicación",EnableSession=true)]
        public global::Session.SoftRef[] getAsocDocQueryList(int sessionId, int appId, int docTypeId)
        {
            lock (sessionId.ToString())
            {
                epSession actual = getSession(sessionId);

                if (actual != null)
                    return actual.getAsocDocList(appId, docTypeId);
                else
                    return null;
            }
        }
        /// <summary>
        ///	Procedimiento que retorna la lista de consultas para un tipo de documento
        /// </summary>
        /// <param name="sessionId">Id de la sesion en la cual se esta trabajando</param>
        /// <param name="appId">Id de la aplicacion a la cual pertenece el tipo de documento</param>
        /// <param name="docTypeId">Id del tipo de documento del cual se desea extraer la lista de consultas</param>
        /// <returns>Estructura de Consultas</returns>
        /// <example>
        /// <code>
        /// [C#]
        /// 
        /// //Ejemplo:	Llena un TreeView con las aplicaciones, los tipos de documentos
        ///	//			y las consultas
        /// //Autor:	Grupo de Soluciones Informáticas
        /// 
        /// string user = "GSI";     //usuario de ePower
        /// string password = "GSI"; //password del usuario anterior
        /// 
        /// ewsm service = new ewsm();
        /// int uid = service.doLogin(user,password);
        /// if(uid > 0)
        /// {
        ///		App[] apps = service.getAppList(uid);	//Lista de aplicaciones
        ///		for(int i = 0 ; i &lt; apps.Length ; i++)
        ///		{
        ///			TreeNode appTmp = new TreeNode(apps[i].AppName);
        ///			appTmp.Tag = apps[i].AppId
        ///			DocType[] dts = service.getDocTypeList(uid,apps[i].AppId);
        ///			for(int j = 0 ; j &lt; dts.Length; j++)
        ///			{
        ///				TreeNode docTmp = new TreeNode(dts[j].DocTypeName);
        ///				docTmp.Tag = dts[j].DocTypeId;
        ///				Query[] querys = service.getQueryList(uid,apps[i].AppId, dts[j].DocTypeId);
        ///				for(int k = 0 ; k &lt; querys.Length; k++)
        ///				{
        ///					TreeNode queryTmp = new TreeNode(querys[k].QueryName);
        ///					queryTmp.Tag = querys[k].QueryId;
        ///					docTmp.Nodes.Add(queryTmp);
        ///				}
        ///				appTmp.Nodes.Add(docTmp);
        ///			}
        ///			_tree.Nodes.Add(appTmp);			//_tree es una instancia de un TreeView
        ///		}
        /// }
        /// </code>
        /// <seealso cref="getAppList"/>
        /// <seealso cref="getDocTypeList"/>
        /// <seealso cref="Query"/>
        /// </example>
        //[WebMethod(Description="Obtiene Lista de Consultas",EnableSession=true)]
        public global::Session.Query[] getQueryList(int sessionId, int appId, int docTypeId)
        {
            lock (sessionId.ToString())
            {
                epSession actual = getSession(sessionId);

                if (actual != null)
                    return actual.getQuerys(appId, docTypeId, 2);
                else
                    return null;
            }
        }
        /// <summary>
        ///	Procedimiento que retorna la lista de campos de un tipo de documento
        /// </summary>
        /// <param name="sessionId">Id de la sesion en la cual se esta trabajando</param>
        /// <param name="appId">Id de la aplicacion a la cual pertenece el tipo de documento</param>
        /// <param name="docTypeId">Id del tipo de documento al cual pertenece la consulta</param>
        /// <param name="queryId">Id de la consulta de donde se desea extraer los campos del tipo de documento</param>
        /// <returns>Estructura de campos del Tipo de Documento</returns>
        /// <example>
        /// <code>
        /// [C#]
        /// 
        /// //Ejemplo:	Llena un TreeView con las aplicaciones y los tipos de documentos
        ///	//			y al seleccionar cada Tipo de Documento te muestra los 
        ///	//			campos de dicho tipo de documento
        /// //Autor:	Grupo de Soluciones Informáticas
        /// 
        /// int uid;
        /// TreeView _tree;
        /// 
        /// ctor()
        /// {
        ///		uid = -1;
        ///		_tree = new TreeView();
        ///		_tree.AfterSelect += new TreeViewEventHandler(TreeView1_AfterSelect);
        ///		_tree.PathSeparator = ".";
        ///		llenarTreeView();
        /// }
        /// 
        /// public void llenarTreeView()
        /// {
        ///		string user = "GSI";     //usuario de ePower
        ///		string password = "GSI"; //password del usuario anterior
        /// 
        ///		ewsm service = new ewsm();
        ///		uid = service.doLogin(user,password);
        ///		if(uid > 0)
        ///		{
        ///			App[] apps = service.getAppList(uid);	//Lista de aplicaciones
        ///			for(int i = 0 ; i &lt; apps.Length ; i++)
        ///			{
        ///				TreeNode appTmp = new TreeNode(apps[i].AppName);
        ///				appTmp.Tag = apps[i].AppId
        ///				DocType[] dts = service.getDocTypeList(uid,apps[i].AppId);
        ///				for(int j = 0 ; j &lt; dts.Length; j++)
        ///				{
        ///					TreeNode docTmp = new TreeNode(dts[j].DocTypeName);
        ///					docTmp.Tag = dts[j].DocTypeId;
        ///					appTmp.Nodes.Add(docTmp);
        ///				}
        ///				_tree.Nodes.Add(appTmp);			//_tree es una instancia de un TreeView
        ///			}
        ///		}
        ///	}
        ///	
        ///	private void TreeView1_AfterSelect(System.Object sender, 
        ///		System.Windows.Forms.TreeViewEventArgs e)
        ///	{
        ///		TreeNode tmp = (TreeNode) sender;
        ///		
        ///		if(tmp.Parent != null)  //Las aplicaciones no tienen padre 
        ///		{						//pero los tipos de documentos si
        ///			int app = tmp.Parent.Tag;
        ///			int docType = tmp.Tag;
        ///			
        ///			FieldStruct[] fields = service.getDocTypeFields(uid,app,docType);
        ///			string campos = "";
        ///			for(int i = 0 ; i &lt; fields.Length ; i++)
        ///			{
        ///				asociaciones = "Campo: " + fields[i].FieldName + " Tipo: " + fields[i].FieldType + "\n";
        ///			}
        ///			MessageBox.Show(asociaciones);
        ///		}
        ///	}
        /// </code>
        /// </example>
        //[WebMethod(Description="Obtiene los campos del Tipo de Documento",EnableSession=true)]
        public global::Session.FieldStruct[] getDocTypeFields(int sessionId, int appId, int docTypeId, int queryId)
        {
            lock (sessionId.ToString())
            {
                epSession actual = getSession(sessionId);

                if (actual != null)
                    return actual.getDocTypeFields(appId, docTypeId);
                else
                    return null;
            }
        }
        /// <summary>
        ///	Procedimiento que extrae la lista de variables que contiene una consulta
        /// </summary>
        /// <param name="sessionId">Id de la sesion en la cual se esta trabajando</param>
        /// <param name="appId">Id de la aplicación a la cual pertenece el tipo de documento</param>
        /// <param name="docTypeId">Id del Tipo de documento al cual pertenece la consulta</param>
        /// <param name="queryId">Id de la consulta de la cual se desea extraer las variables</param>
        /// <returns>Estructura de campos con los valores y definición de las variables</returns>
        /// <example>
        /// <code>
        /// [C#]
        /// 
        /// //Ejemplo:	Llena un TreeView con las aplicaciones, tipos de documentos y consultas
        ///	//			y al seleccionar cada Consulta se muestran los campos de la consulta 
        /// //Autor:	Grupo de Soluciones Informáticas
        /// 
        /// int uid;
        /// TreeView _tree;
        /// 
        /// ctor()
        /// {
        ///		uid = -1;
        ///		_tree = new TreeView();
        ///		_tree.AfterSelect += new TreeViewEventHandler(TreeView1_AfterSelect);
        ///		_tree.PathSeparator = ".";
        ///		llenarTreeView();
        /// }
        /// 
        /// public void llenarTreeView()
        /// {
        ///		string user = "GSI";     //usuario de ePower
        ///		string password = "GSI"; //password del usuario anterior
        /// 
        ///		ewsm service = new ewsm();
        ///		uid = service.doLogin(user,password);
        ///		if(uid > 0)
        ///		{
        ///			App[] apps = service.getAppList(uid);	//Lista de aplicaciones
        ///			for(int i = 0 ; i &lt; apps.Length ; i++)
        ///			{
        ///				TreeNode appTmp = new TreeNode(apps[i].AppName);
        ///				appTmp.Tag = apps[i].AppId
        ///				DocType[] dts = service.getDocTypeList(uid,apps[i].AppId);
        ///				for(int j = 0 ; j &lt; dts.Length; j++)
        ///				{
        ///					TreeNode docTmp = new TreeNode(dts[j].DocTypeName);
        ///					docTmp.Tag = dts[j].DocTypeId;
        ///					for(int k = 0 ; k &lt; querys.Length; k++)
        ///					{
        ///						TreeNode queryTmp = new TreeNode(querys[k].QueryName);
        ///						queryTmp.Tag = querys[k].QueryId;
        ///						docTmp.Nodes.Add(queryTmp);
        ///					}
        ///					appTmp.Nodes.Add(docTmp);
        ///				}
        ///				_tree.Nodes.Add(appTmp);			//_tree es una instancia de un TreeView
        ///			}
        ///		}
        ///	}
        ///	
        ///	private void TreeView1_AfterSelect(System.Object sender, 
        ///		System.Windows.Forms.TreeViewEventArgs e)
        ///	{
        ///		TreeNode tmp = (TreeNode) sender;
        ///		
        ///		if(tmp.Parent != null)  // El padre de la contulta debe ser un Tipo de Documento
        ///		{					
        ///			if(tmp.Parent.Parent != null) //El padre del tipo de documento debe ser una aplicacion
        ///			{
        ///				int app = tmp.Parent.Parent.Tag;
        ///				int docType = tmp.Parent.Tag;
        ///				int query = tmp.Tag;
        ///			
        ///				Qfld[] fields = service.getQueryFields(uid,app,docType,query);
        ///				string campos = "";
        ///				for(int i = 0 ; i &lt; fields.Length ; i++)
        ///				{
        ///					campos = "Campo: " + fields[i].FldEt + "\n";
        ///				}
        ///				MessageBox.Show(campos);
        ///			}
        ///		}
        ///	}
        /// </code>
        /// <seealso cref="Query"/>
        /// <seealso cref="getQueryList"/>
        /// </example>
        //[WebMethod(Description="Obtiene Lista de campos variables de la consulta",EnableSession=true)]
        public global::Comun.Qfld[] getQueryFields(int sessionId, int appId, int docTypeId, int queryId)
        {
            lock (sessionId.ToString())
            {
                epSession actual = getSession(sessionId);

                if (actual != null)
                    return actual.getQueryFields(appId, docTypeId, queryId);
                else
                    return null;
            }
        }

        /// <summary>
        ///	Procedimiento que retorna la estructura de arbol que contiene las referencias de un documento
        /// </summary>
        /// <param name="sessionId">Id de la sesion en la cual se esta trabajando</param>
        /// <param name="docTypeId">Id del tipo de documento</param>
        /// <param name="queryId">Id de la consulta que devuelve el documento deseado</param>
        /// <param name="docId">Id del Documento al que se le desea extraer la estrutura de cejillas</param>
        /// <returns>Estructura de cejillas de un documento</returns>
        /// <example>
        /// <code>
        /// [C#]
        /// 
        /// //Ejemplo:	Llena un TreeView con las aplicaciones, tipos de documentos y consultas,
        ///	//			se dispone de un metodo que ejecuta una consulta y selecciona el primer registro 
        ///	//			del resultado y pide la estructura de cejillas y la pinta en un treeView
        /// //Autor:	Grupo de Soluciones Informáticas
        /// 
        /// int uid;
        /// TreeView _tree;
        /// TreeView _tabView;
        /// 
        /// ctor()
        /// {
        ///		uid = -1;
        ///		_tree = new TreeView();
        ///		_tree.AfterSelect += new TreeViewEventHandler(TreeView1_AfterSelect);
        ///		_tree.PathSeparator = ".";
        ///		_tabView = new TreeView();
        ///		llenarTreeView();
        /// }
        /// 
        /// public void llenarTreeView()
        /// {
        ///		string user = "GSI";     //usuario de ePower
        ///		string password = "GSI"; //password del usuario anterior
        /// 
        ///		ewsm service = new ewsm();
        ///		uid = service.doLogin(user,password);
        ///		if(uid > 0)
        ///		{
        ///			App[] apps = service.getAppList(uid);	//Lista de aplicaciones
        ///			for(int i = 0 ; i &lt; apps.Length ; i++)
        ///			{
        ///				TreeNode appTmp = new TreeNode(apps[i].AppName);
        ///				appTmp.Tag = apps[i].AppId
        ///				DocType[] dts = service.getDocTypeList(uid,apps[i].AppId);
        ///				for(int j = 0 ; j &lt; dts.Length; j++)
        ///				{
        ///					TreeNode docTmp = new TreeNode(dts[j].DocTypeName);
        ///					docTmp.Tag = dts[j].DocTypeId;
        ///					for(int k = 0 ; k &lt; querys.Length; k++)
        ///					{
        ///						TreeNode queryTmp = new TreeNode(querys[k].QueryName);
        ///						queryTmp.Tag = querys[k].QueryId;
        ///						docTmp.Nodes.Add(queryTmp);
        ///					}
        ///					appTmp.Nodes.Add(docTmp);
        ///				}
        ///				_tree.Nodes.Add(appTmp);			//_tree es una instancia de un TreeView
        ///			}
        ///		}
        ///	}
        ///	
        ///	/*Metodo:	Cuando recibe una instancia de un Nodo distinta de nulo
        ///				llena dicho nodo con la informacion del tab que recibio 
        ///				como parametro, es un metodo recursivo.
        ///				Si la instancia del Nodo es nula, lo agrega en el TreeView
        ///	sdgsdg*/
        ///	private void builderTabs(service.Tab[] tabs,TreeNode that)
        ///	{
        ///		foreach(service.Tab tmp in tabs)
        ///		{
        ///			TreeNode temp = new TreeNode(tmp.TabName);
        ///			temp.Tag = tmp.TabId;
        ///			if(tmp.Tabs != null)
        ///			{
        ///				if(tmp.Tabs.Length > 0)
        ///				{
        ///					builderTabs(tmp.Tabs,temp);
        ///				}
        ///			}
        ///			if(tmp.References != null)
        ///			{
        ///				if(tmp.References.Length > 0)
        ///				{
        ///					foreach(service.References reftemp in tmp.References)
        ///					{
        ///						TreeNode refElement = new TreeNode(reftemp.Alias);
        ///						refElement.Tag = reftemp.ReferenceId;
        ///						temp.Nodes.Add(refElement);
        ///					}
        ///				}
        ///			}
        ///			else
        ///			{
        ///				TreeNode refElement = new TreeElement("--Ninguna--");
        ///				temp.Nodes.Add(refElement);
        ///			}
        ///			if(that != null)
        ///				that.Node.Add(temp);
        ///			else
        ///				_tabView.Nodes.Add(temp);	//_tabView es una instancia de un TreeView
        ///		}
        ///	}
        /// </code>
        /// <seealso cref="Query"/>
        /// <seealso cref="getEntryForm"/>
        /// </example>
        //[WebMethod(Description="Obtiene Estructura de Cejillas",EnableSession=true)]
        public ePWS.WS.Type.Tab[] getTabStruct(int sessionId, int docTypeId, int queryId, int docId)
        {
            lock (sessionId.ToString())
            {
                epSession actual = getSession(sessionId);

                if (actual != null)
                    return actual.getTabStruct(sessionId, docTypeId, queryId, docId);
                else
                    return null;
            }
        }

        public string getTabName(ePWS.WS.Type.Tab[] Tabs, int parentId)
        {
            string tabName = "";
            foreach (var tabs in Tabs)
            {
                ePWS.WS.Type.Tab[] newTabs = tabs.SubTabs;

                if (tabs.TabId == parentId)
                {
                    tabName = tabs.TabName;
                }
                if (tabName == "")
                {
                    if (newTabs != null)
                    {
                        tabName = getTabName(newTabs, parentId);
                    }
                    if (tabName != "")
                        return tabName;
                }
                else
                    return tabName;
            }
            return tabName;
        }

        public int getTabId(ePWS.WS.Type.Tab[] Tabs, string tabName)
        {
            int tabId = 0;
            foreach (var tabs in Tabs)
            {
                ePWS.WS.Type.Tab[] newTabs = tabs.SubTabs;

                if (tabs.TabName == tabName)
                {
                    tabId = tabs.TabId;
                }
                if (tabId == 0)
                {
                    if (newTabs != null)
                    {
                        tabId = getTabId(newTabs, tabName);
                    }
                    if (tabId != 0)
                        return tabId;
                }
                else
                    return tabId;
            }
            return tabId;
        }

        public global::Comun.gsiStructTab[] getTabTempleteStruct(int sessionId, int appId, int docTypeId)
        {
            lock (sessionId.ToString())
            {
                epSession actual = getSession(sessionId);

                if (actual != null)
                    return actual.getDocTypeTabsTemplate(sessionId, appId, docTypeId);
                else
                    return null;
            }
        }

        /// <summary>
        /// Procedimiento que retorna el formulario de entrada de un documento
        /// </summary>
        /// <param name="sessionId">Id de la sesion en la cual se esta trabajando</param>
        /// <param name="docTypeId">Id del Tipo de Documento</param>
        /// <param name="queryId">Id de la consulta que retorna el documento deseado</param>
        /// <param name="docId">Id del documento al que se le desea extraer el formulario de entrada</param>
        /// <returns>Estructura de campos del formulario de entrada del documento</returns>
        /// <example>
        /// <code>
        /// [C#]
        /// string user = "GSI";     //usuario de ePower
        /// string password = "GSI"; //password del usuario anterior
        /// int docTypeId = 1002;    //ejemplo de Id de un tipo de documento dentro de ePower
        /// int queryId = 1002;      //ejemplo de Id de una consulta dentro de ePower
        /// int docId = 12345;       //ejemplo de Id de un documento dentro de ePower
        /// 
        /// ewsm service = new ewsm();
        /// int uid = service.doLogin(user,password);
        /// gsiStructFld[] fieldsForm = service.getEntryForm(uid, docTypeId, queryId, docId);
        /// </code>
        /// </example>
        //[WebMethod(Description="Obtiene el formulario de entrada para un documento",EnableSession=true)]
        public global::Comun.gsiStructFld[] getEntryForm(int sessionId, int docTypeId, int queryId, int docId)
        {
            lock (sessionId.ToString())
            {
                epSession actual = getSession(sessionId);

                if (actual != null)
                    return actual.getEntryForm(sessionId, docTypeId, queryId, docId);
                else
                    return null;
            }
        }
        /// <summary>
        /// Obtiene las acciones para un formulario
        /// </summary>
        /// <param name="sessionId">Id de la sesion</param>
        /// <param name="formId">Id del formulario</param>
        /// <param name="formFldId">Id del campo del formulario</param>
        /// <returns>Accion</returns>
        //[WebMethod(Description="Obtiene las acciones para el entryForm",EnableSession=true)]
        public global::Session.EventAct[] getEventActionByEF(int sessionId, int formId, int formFldId)
        {
            lock (sessionId.ToString())
            {
                epSession actual = getSession(sessionId);

                if (actual != null)
                    return actual.getEventActionByEF(formId, formFldId);
                else
                    throw new Exception("Sesion no existe");
            }
        }
        /// <summary>
        /// Procedimiento que retorna todas la estructuras pertenecientes a un documento
        /// </summary>
        /// <param name="sessionId">Id de la sesion en la cual se está trabajando</param>
        /// <param name="appId">Id de la aplicación a la que pertenece el documento</param>
        /// <param name="docTypeId">Id del Tipo de Documento</param>
        /// <param name="queryId">Id de la consulta que que retorna el documento deseado</param>
        /// <param name="docId">Id del documento al que se le desea extraer todas las estructuras</param>
        /// <returns>Estructuras completas del documento</returns>
        /// <example>
        /// <code>
        /// [C#]
        /// string user = "GSI";     //usuario de ePower
        /// string password = "GSI"; //password del usuario anterior
        /// int docTypeId = 1000;    //ejemplo de Id de una aplicacion dentro de ePower
        /// int docTypeId = 1001;    //ejemplo de Id de un tipo de documento dentro de ePower
        /// int queryId = 1002;      //ejemplo de Id de una consulta dentro de ePower
        /// int docId = 12345;       //ejemplo de Id de un documento dentro de ePower
        /// 
        /// ewsm service = new ewsm();
        /// int uid = service.doLogin(user,password);
        /// Collection fullDoc = service.getFullDocument(uid, appId, docTypeId, queryId, docId);
        /// </code>
        /// </example>
        //[WebMethod(Description="Obtiene todas las estructuras de un documento",EnableSession=true)]
        public global::Interfaces.Collection getFullDocument(int sessionId, int appId, int docTypeId, int queryId, int docId)
        {
            lock (sessionId.ToString())
            {
                epSession actual = getSession(sessionId);

                if (actual != null)
                    return actual.getFullDoc(appId, docTypeId, queryId, docId, false);
                else
                    throw new Exception("Sesion no existe");
            }
        }

        public string getUrlWSReference(int sessionId, int appId, int docTypeId, int queryId, int docId, int refId)
        {
            lock (sessionId.ToString())
            {
                epSession actual = getSession(sessionId);

                if (actual != null)

                    return actual.getUrlRef(appId, docTypeId, queryId, docId, refId);

                //return actual.getRef(appId, docTypeId, queryId, docId, refId);
                else
                    throw new Exception("Sesion no existe");
            }
        }

        /// <summary>
        ///	Procedimiento que ejecuta una consulta dentro de ePower
        /// </summary>
        /// <param name="sessionId">Id de la sesion en la cual se está trabajando</param>
        /// <param name="appId">Id de la aplicación a la que pertenece del tipo de documento</param>
        /// <param name="docTypeId">Id del tipo de documento al que pertenece la consulta</param>
        /// <param name="queryId">Id de la consulta que se desea ejecutar</param>
        /// <param name="maxRows">Cantidad de filas que desea que devuelva la consulta</param>
        /// <param name="fldValue">Vector de valores en caso de que la consulta contega algun parametro variable</param>
        /// <param name="condicion">Arbol que contiene la condicion para filtrar la busqueda</param>
        /// <param name="FTCond">Estructura que indica si desea hacer busqueda en fulltext y como desea hacerla</param>
        /// <returns>Estructura de filas resultantes de la consulta</returns>
        /// <example>
        /// <code>
        /// [C#]
        /// string user = "GSI";       //usuario de ePower
        /// string password = "GSI";   //password del usuario anterior
        /// int appId = 1000;      //ejemplo de Id de una aplicacion dentro de ePower
        /// int docTypeId = 1001;      //ejemplo de Id de un tipo de documento dentro de ePower
        /// int queryId = 1002;        //ejemplo de Id de una consulta dentro de ePower
        /// int maxrows = -1;
        /// 
        /// ewsm service = new ewsm();
        /// int uid = service.doLogin(user,password);
        /// ePWS.WS.Type.QueryInst result = service.doExecuteQuery(uid, appId, docTypeId, queryId, maxrows,null,null,null);
        /// </code>
        /// </example>
        //[WebMethod(Description="Ejecuta una consulta",EnableSession=true)]
        public ePWS.WS.Type.QueryInst doExecuteQuery(int sessionId, int appId, int docTypeId, int queryId, int maxRows,
            string[] fldValue, GSI.eTree condicion, ePWS.WS.Type.FullText FTCond)
        {

            lock (sessionId.ToString())
            {
                epSession actual = getSession(sessionId);

                if (actual != null)
                {
                    //return actual.doExecuteQuery(appId, docTypeId, queryId, maxRows, null, new GSI.eTree(),null);
                    if (condicion != null && FTCond != null)
                    {
                        return actual.doExecuteQuery(appId, docTypeId, queryId, maxRows, fldValue, condicion, FTCond);
                    }
                    else if (condicion != null)
                    {
                        return actual.doExecuteQuery(appId, docTypeId, queryId, maxRows, fldValue, condicion, null);
                    }
                    else if (FTCond != null)
                    {
                        return actual.doExecuteQuery(appId, docTypeId, queryId, maxRows, fldValue, new GSI.eTree(), FTCond);
                    }
                    else
                    {
                        return actual.doExecuteQuery(appId, docTypeId, queryId, maxRows, fldValue, new GSI.eTree(), null);
                    }
                }
                else
                    return null;
            }
        }
        /// <summary>
        ///	Procedimiento que ejecuta una consulta personalizada contra la base de datos
        /// </summary>
        /// <param name="sessionId">Id de la sesion en la cual se está trabajando</param>
        /// <param name="appId">Id de la aplicación a la que pertenece del tipo de documento</param>
        /// <param name="docTypeId">Id del tipo de documento</param>
        /// <param name="maxRows">Cantidad de filas que se desea debuelva la consulta</param>
        /// <param name="columns">Columnas que desea que contenga el Grid resultado</param>
        /// <param name="condicion">Arbol que contiene la condicion para filtrar la busqueda</param>
        /// <param name="FTCond">Estructura que indica si desea hacer busqueda en fulltext y como desea hacerla</param>
        /// <returns>Estructura de filas resultantes de la consulta</returns>
        /// <example>
        /// <code>
        /// [C#]
        /// string user = "GSI";       //usuario de ePower
        /// string password = "GSI";   //password del usuario anterior
        /// int appId = 1000;      //ejemplo de Id de una aplicacion dentro de ePower
        /// int docTypeId = 1001;      //ejemplo de Id de un tipo de documento dentro de ePower
        /// int maxrows = -1;
        /// string[] columns = {"n","1000.1001.1234"}; //Para más informacion leer manual del API Web de ePower
        /// 
        /// ewsm service = new ewsm();
        /// int uid = service.doLogin(user,password);
        /// ePWS.WS.Type.QueryInst result = service.doExecutePersonalQuery(uid, appId, docTypeId, maxrows,columns,null,null);
        /// </code>
        /// </example>
        //[WebMethod(Description="Ejecuta una consulta personalizada",EnableSession=true)]
        public ePWS.WS.Type.QueryInst doExecutePersonalQuery(int sessionId, int appId, int docTypeId, int maxRows, string[] columns,
            GSI.eTree condicion, ePWS.WS.Type.FullText FTCond)
        {
            lock (sessionId.ToString())
            {
                epSession actual = getSession(sessionId);

                if (actual != null)
                {
                    if (condicion != null && FTCond != null)
                    {
                        return actual.doExecutePersonalQuery(appId, docTypeId, maxRows, columns, condicion, FTCond);
                    }
                    else if (condicion != null)
                    {
                        return actual.doExecutePersonalQuery(appId, docTypeId, maxRows, columns, condicion, null);
                    }
                    else if (FTCond != null)
                    {
                        return actual.doExecutePersonalQuery(appId, docTypeId, maxRows, columns, new GSI.eTree(), FTCond);
                    }
                    else
                    {
                        return actual.doExecutePersonalQuery(appId, docTypeId, maxRows, columns, new GSI.eTree(), null);
                    }
                }
                else
                    return null;
            }
        }
        /// <summary>
        ///	Procedimiento que ejecuta una consulta para encontrar documentos asociados
        /// </summary>
        /// <param name="sessionId">Id de la sesion en la cual se está trabajando</param>
        /// <param name="docId">Id del documento del cual deseas encontrar los documentos asociados</param>
        /// <param name="maxRows">Cantidad de filas que se desea debuelva la consulta</param>
        /// <param name="reference">Estructura que contiene la informacion del documento asociado</param>
        /// <param name="condicion">Arbol que contiene la condicion para filtrar la busqueda</param>
        /// <param name="FTCond">Estructura que indica si desea hacer busqueda en fulltext y como desea hacerla</param>
        /// <returns>Estructura de filas resultantes de la consulta</returns>
        /// <example>
        /// <code>
        /// [C#]
        /// string user = "GSI";       //usuario de ePower
        /// string password = "GSI";   //password del usuario anterior
        /// int appId = 1000;          //ejemplo de Id de una aplicacion dentro de ePower
        /// int docTypeId = 1001;      //ejemplo de Id de un tipo de documento dentro de ePower
        /// int docId = 12345		   //ejemplo de un Id de un documento dentro de ePower
        /// int maxrows = -1;
        /// 
        /// ewsm service = new ewsm();
        /// int uid = service.doLogin(user,password);
        /// SoftRef[] sr = service.getAsocDocQueryList(uid,appId,docTypeId);
        /// if(sr.Length > 0)
        ///		if(sr[0].SoftId != -1)
        ///			ePWS.WS.Type.QueryInst result = service.doExecuteAsocDocQuery(uid, docId, maxRows, sr[0],null,null);
        /// </code>
        /// </example>
        //[WebMethod(Description="Ejecuta una consulta",EnableSession=true)]
        public ePWS.WS.Type.QueryInst doExecuteAsocDocQuery(int sessionId, int docId, int maxRows, global::Session.SoftRef reference, GSI.eTree condicion, ePWS.WS.Type.FullText FTCond)
        {

            lock (sessionId.ToString())
            {
                epSession actual = getSession(sessionId);
                string[] fldValue = new string[1];
                fldValue[0] = docId.ToString();
                if (actual != null)
                {
                    if (condicion != null && FTCond != null)
                    {
                        return actual.doExecuteAsocDocQuery(fldValue, maxRows, reference, condicion, FTCond);
                    }
                    else if (condicion != null)
                    {
                        return actual.doExecuteAsocDocQuery(fldValue, maxRows, reference, condicion, null);
                    }
                    else
                    {
                        return actual.doExecuteAsocDocQuery(fldValue, maxRows, reference, new GSI.eTree(), FTCond);
                    }
                }
                else
                    return null;
            }
        }
        /// <summary>
        /// Procedimiento que retorna una referencia a un archivo digital de un documento
        /// </summary>
        /// <param name="sessionId">Id de la sesion en la cual se está trabajando</param>
        /// <param name="docTypeId">Id del tipo de documento del documento al que pertence el documento</param>
        /// <param name="queryId">Id de la consulta con la cual obtuvo el Id del documento</param>
        /// <param name="docId">Id del documento del cual desea obtener la referencia</param>
        /// <param name="refId">Id de la referencia del archivo digital que desea obtener</param>
        /// <returns>Estructura que contiene el archivo en un array de bytes</returns>
        /// <example>
        /// <code>
        /// [C#]
        /// string user = "GSI";		//usuario de ePower
        /// string password = "GSI";	//password del usuario anterior
        /// int appId = 1000;			//ejemplo de Id de una aplicacion dentro de ePower
        /// int docTypeId = 1001;		//ejemplo de Id de un tipo de documento dentro de ePower
        /// int queryId = 1002;			//ejemplo de Id de una consulta dentro de ePower
        /// int maxrows = -1;
        /// int docId;					//docId de un documento dentro de ePower
        /// int refId;					//Id de una referencia de un documento en ePower
        /// 
        /// ewsm service = new ewsm();
        /// int uid = service.doLogin(user,password);
        /// ePWS.WS.Type.QueryInst result = service.doExecuteQuery(uid, appId, docTypeId, queryId, maxrows,null,null,null);
        /// 
        /// docId = result.DocInst[0].DocId;
        /// ePWS.WS.Type.Tab[] tabs = service.getTabStruct(uid, docTypeId, queryId, docId);
        /// if(tabs.Length > 0)
        ///		if(tabs[0].References > 0)
        ///			refId = tabs[0].References[0].ReferenceId;
        /// ePWS.WS.Type.DocFile doc = service.getRef(uid,docTypeId,queryId,docId,refId);
        /// </code>
        /// </example>
        //[WebMethod(Description="Obtiene una referencia para un Documento",EnableSession=true)]
        public ePWS.WS.Type.DocFile getRef(int sessionId, int docTypeId, int queryId, int docId, int refId)
        {
            lock (sessionId.ToString())
            {
                ePWS.WS.Type.DocFile Doc = new ePWS.WS.Type.DocFile();
                epSession actual = getSession(sessionId);

                if (actual != null)
                {
                    Doc.document = actual.getRef(sessionId, docTypeId, queryId, docId, refId, ref Doc.ext);
                    return Doc;
                }
                else
                    return null;
            }
        }

        //[WebMethod(Description = "Verifica si la referencia existe", EnableSession = true)]
        public bool existsRef(int sessionId, int appId, int docTypeId, int queryId, int docId, int refId)
        {
            lock (sessionId.ToString())
            {
                epSession actual = getSession(sessionId);

                if (actual != null)
                {
                    return actual.existsRef(appId, docTypeId, queryId, docId, refId);
                }
                else
                    return false;
            }
        }
        /// <summary>
        /// Metodo que obtiene una referencia en formato PDF
        /// </summary>
        /// <param name="sessionId">Identificador de la sesion</param>
        /// <param name="docTypeId">Tipo de Documento al que petenece la referencia</param>
        /// <param name="queryId">Consulta con la cual se referencio el documento al que pertenece la referencia</param>
        /// <param name="docId">Identificador del documento</param>
        /// <param name="refId">Identificador de la referencia</param>
        /// <param name="SaveAs">Parametro de seguridad, el pdf permite salvado</param>
        /// <param name="Print">Parametro de seguridad, el pdf permite impresion</param>
        /// <param name="Copy">Parametro de seguridad, el pdf permite copia de cualquier parte del documento</param>
        /// <returns></returns>
        //[WebMethod(Description="Obtiene una referencia en formato pdf para un Documento",EnableSession=true)]
        public ePWS.WS.Type.DocFile getPdfRef(int sessionId, int docTypeId, int queryId, int docId, int refId, bool SaveAs, bool Print, bool Copy)
        {
            lock (sessionId.ToString())
            {
                ePWS.WS.Type.DocFile Doc = new ePWS.WS.Type.DocFile();
                epSession actual = getSession(sessionId);

                if (actual != null)
                {
                    Doc.document = actual.getPdfRef(sessionId, docTypeId, queryId, docId, refId, ref Doc.ext, SaveAs, Print, Copy);
                    return Doc;
                }
                else
                    return null;
            }
        }
        /// <summary>
        /// Procedimiento que retorna una referencia a un archivo digital de un documento
        /// </summary>
        /// <param name="sessionId">Id de la sesion en la cual se está trabajando</param>
        /// <param name="docTypeId">Id del tipo de documento del documento al que pertence el documento</param>
        /// <param name="queryId">Id de la consulta con la cual obtuvo el Id del documento</param>
        /// <param name="docId">Id del documento del cual desea obtener la referencia</param>
        /// <param name="refId">Id de la referencia del archivo digital que desea obtener</param>
        /// <returns>Estructura que contiene el archivo en un array de bytes</returns>
        /// <example>
        /// <code>
        /// [C#]
        /// string user = "GSI";		//usuario de ePower
        /// string password = "GSI";	//password del usuario anterior
        /// int appId = 1000;			//ejemplo de Id de una aplicacion dentro de ePower
        /// int docTypeId = 1001;		//ejemplo de Id de un tipo de documento dentro de ePower
        /// int queryId = 1002;			//ejemplo de Id de una consulta dentro de ePower
        /// int maxrows = -1;
        /// int docId;					//docId de un documento dentro de ePower
        /// int refId;					//Id de una referencia de un documento en ePower
        /// 
        /// ewsm service = new ewsm();
        /// int uid = service.doLogin(user,password);
        /// ePWS.WS.Type.QueryInst result = service.doExecuteQuery(uid, appId, docTypeId, queryId, maxrows,null,null,null);
        /// 
        /// docId = result.DocInst[0].DocId;
        /// ePWS.WS.Type.Tab[] tabs = service.getTabStruct(uid, docTypeId, queryId, docId);
        /// if(tabs.Length > 0)
        ///		if(tabs[0].References > 0)
        ///			refId = tabs[0].References[0].ReferenceId;
        /// ePWS.WS.Type.DocFile doc = service.getRef(uid,docTypeId,queryId,docId,refId);
        /// </code>
        /// </example>
        //[WebMethod(Description="Obtiene una referencia para un Documento",EnableSession=true)]
        public string getUrlRef(int sessionId, int docTypeId, int queryId, int docId, int refId)
        {
            lock (sessionId.ToString())
            {
                epSession actual = getSession(sessionId);

                if (actual != null)
                {
                    return actual.getUrlRef(sessionId, docTypeId, queryId, docId, refId);
                }
                else
                    return null;
            }
        }
        /// <summary>
        /// Procedimiento que obtiene una referencia a un archivo digital
        /// </summary>
        /// <param name="sessionId">Id de la sesión</param>
        /// <param name="docTypeId">Id del tipo de documento al que pertenece el archivo</param>
        /// <param name="queryId">Id de la consulta con la que se obtuvo el archivo</param>
        /// <param name="docId">Id de la instancia del documento donde está ubicado el archivo</param>
        /// <param name="refId">Id de la referencia al archivo</param>
        /// <returns>Información del Archivo</returns>
        //[WebMethod(Description="Obtiene y mantiene en cache la informacion de una referencia de un Documento",EnableSession=true)]
        public ePWS.WS.Type.refInfo obtainCacheRef(int sessionId, int docTypeId, int queryId, int docId, int refId)
        {
            lock (sessionId.ToString())
            {
                epSession actual = getSession(sessionId);

                if (actual != null)
                {
                    return actual.obtainRef(docTypeId, queryId, docId, refId);
                }
                else
                    return null;
            }
        }
        /// <summary>
        /// Funcion que permite obtener un paquete del archivo cargado en el cache
        /// </summary>
        /// <param name="sessionId">Id de la sesion del usuario</param>
        /// <param name="packNum">Indice del paquete que desea obtener</param>
        /// <returns>Binario correspondiente al paquete deseado</returns>
        //[WebMethod(Description="Obtiene un bloque del archivo actualmente en cache",EnableSession=true)]
        public byte[] getBlockRef(int sessionId, int packNum)
        {
            lock (sessionId.ToString())
            {
                epSession actual = getSession(sessionId);

                if (actual != null)
                {
                    return actual.getBlockRef(packNum);
                }
                else
                    return null;
            }
        }
        /// <summary>
        /// Funcion que libera la memoria correspondiente al archivo cargado en el cache
        /// </summary>
        /// <param name="sessionId">Id de la sesion del usuario</param>
        /// <returns>Resultado de la operacion</returns>
        //[WebMethod(Description="Libera el archivo actualmente en cache",EnableSession=true)]
        public bool releaseCacheRef(int sessionId)
        {
            lock (sessionId.ToString())
            {
                epSession actual = getSession(sessionId);

                if (actual != null)
                {
                    return actual.releaseRef();
                }
                else
                    return false;
            }
        }

        /// <summary>
        /// Funcion que asigna el valor de una accion
        /// </summary>
        /// <param name="sessionId">Id de la sesion</param>
        /// <param name="actionId">Id de la accion</param>
        /// <returns>Valor del campo</returns>
        //[WebMethod(Description="Obtiene la informacion de una accion",EnableSession=true)]
        public global::Comun.Fld[] fillAction(int sessionId, int actionId)
        {
            lock (sessionId.ToString())
            {
                epSession actual = getSession(sessionId);

                if (actual != null)
                {
                    return actual.fillAction(actionId);
                }
                else
                    return null;
            }
        }
        /// <summary>
        /// Funcion que devuelve el entryForm de una consulta
        /// </summary>
        /// <param name="sessionId">Id de la sesion</param>
        /// <param name="queryId">Id de la consulta de la cual se desea obtener el formulario de entrada</param>
        /// <returns></returns>
        //[WebMethod(Description="Obtiene el EntryForm de una consulta",EnableSession=true)]
        public int getEntryFormByQuery(int sessionId, int queryId)
        {
            lock (sessionId.ToString())
            {
                epSession actual = getSession(sessionId);

                if (actual != null)
                {
                    return actual.getEntryFormByQuery(queryId);
                }
                else
                    return -1;
            }
        }
        /// <summary>
        /// Procedimiento que retorna la lista de usuarios de ePower
        /// </summary>
        /// <param name="sessionId">Id de la sesion en la cual se está trabajando</param>
        /// <returns>Lista de usuarios</returns>
        //[WebMethod(Description="Obtiene la Lista de Usuarios de ePower",EnableSession=true)]
        public global::Session.User[] getUsers(int sessionId)
        {
            lock (sessionId.ToString())
            {
                epSession actual = getSession(sessionId);

                if (actual != null)
                {
                    return actual.getUsers();

                }
                else
                    return null;
            }
        }
        //[WebMethod(Description="Obtiene la Lista de Grupos de ePower",EnableSession=true)]
        public global::Session.Group[] getGroups(int sessionId)
        {
            lock (sessionId.ToString())
            {
                epSession actual = getSession(sessionId);

                if (actual != null)
                {
                    return actual.getGroups();

                }
                else
                    return null;
            }
        }
        //[WebMethod(Description = "Obtiene la lista de permisos del Usuario por recurso", EnableSession = true)]
        public ePWS.WS.Type.Permission[] getRightsByResource(int sessionId, String userId, int resourceId, int type, bool isUser)
        {
            lock (sessionId.ToString())
            {
                epSession actual = getSession(sessionId);

                if (actual != null)
                {
                    return actual.getRightsByUser(resourceId, userId, type, isUser);

                }
                else
                    return null;
            }
        }
        #endregion
#endif

#if Update
        #region Funciones de modificacion
        /// <summary>
        ///	Procedimiento de insercion de nuevos documentos a ePower
        /// </summary>
        /// <param name="sessionId">Id de la sesion sobre la que se esta trabajando</param>
        /// <param name="appId">Id de la applicación a la cual pertenece el documento</param>
        /// <param name="docFiles">Documento digital que se va a guardar</param>
        /// <returns>true si se logro agregar el documento</returns>
        //[WebMethod(Description="Agrega referencias",EnableSession=true)]
        public global::Comun.gsiStructRef saveRef(int sessionId, int appId, ePWS.WS.Type.gsiFile docFiles)
        {
            lock (sessionId.ToString())
            {
                epSession actual = getSession(sessionId);

                if (actual != null)
                    return actual.saveRef(appId, docFiles);
                else
                    throw new Exception("Sesion no existe");
            }
        }

        /// <summary>
        ///	Procedimiento de insercion de nuevos documentos a un tipo de documento
        /// </summary>
        /// <param name="sessionId">Identificador de la sesion</param>
        /// <param name="appId">Id de la applicación a la cual pertenece el documento</param>
        /// <param name="docTypeId">Id del tipo de documento al que se le incluirá la referencia</param>
        /// <param name="queryId">Id de la consulta que genera el resultado deseado</param>
        /// <param name="tabs">Estructura de cejillas para el nuevo documento</param>
        /// <param name="entryForm">Valores correspondientes a cada campo del formulario de entrada</param>
        /// <param name="references">Conjunto de Referencias para el nuevo documento</param>
        /// <param name="anotations">Anotaciones correspondientes al nuevo documento</param>
        /// <param name="hits">Hits correspondientes al nuevo documento de ePower</param>
        /// <param name="docFiles">Archivos que se desean insertar como referencias</param>
        /// <returns>true si se logro agrregar la referencia</returns>
        //[WebMethod(Description="Agrega referencias",EnableSession=true)]
        public bool saveFullDocument(int sessionId, int appId, int docTypeId, int queryId,
            global::Comun.gsiStructTab[] tabs,
            global::Comun.gsiStructFld[] entryForm,
            global::Comun.gsiStructRef[] references,
            global::Comun.gsiStructAnot[] anotations,
            global::Comun.gsiHit[] hits)
        {
            lock (sessionId.ToString())
            {
                epSession actual = getSession(sessionId);

                if (actual != null)
                    return actual.saveDocIns(sessionId, 18, appId, docTypeId, queryId, tabs, entryForm, references, anotations, hits);
                else
                    return false;
            }
        }

        /// <summary>
        /// Operacion que modifica un documento existente
        /// </summary>
        /// <param name="sessionId">Identificador de la sesion</param>
        /// <param name="appId">Aplicacion a la que pertenece el documento</param>
        /// <param name="docTypeId">Tipo de documento al que pertenece el documento</param>
        /// <param name="queryId">Consulta con la cual se referencio al documento</param>
        /// <param name="docId">Identificador del documento</param>
        /// <param name="tabs">Estructura de cejillas con las que se va actualizar el documento</param>
        /// <param name="entryForm">Valores del formulario de entrada con los que se va actualizar el documento</param>
        /// <param name="references">Referencias con las que se va actualizar el documento</param>
        /// <param name="anotations">Anotaciones con las que se va actualizar el documento</param>
        /// <param name="hits">Hits con los que se va actualizar el documento</param>
        /// <param name="docFiles">Archivos digitales que se desea agregar al documento</param>
        /// <returns></returns>
        //[WebMethod(Description="Agrega referencias",EnableSession=true)]
        public bool updateFullDocument(int sessionId, int appId, int docTypeId, int queryId, int docId,
            global::Comun.gsiStructTab[] tabs,
            global::Comun.gsiStructFld[] entryForm,
            global::Comun.gsiStructRef[] references,
            global::Comun.gsiStructAnot[] anotations,
            global::Comun.gsiHit[] hits)
        {
            lock (sessionId.ToString())
            {
                epSession actual = getSession(sessionId);

                if (actual != null)
                    return actual.updDocIns(sessionId, 18, appId, docTypeId, queryId, docId, tabs, entryForm, references, anotations, hits);
                else
                    return false;
            }
        }

        /// <summary>
        /// Operacion que permite el borrado de un documento
        /// </summary>
        /// <param name="sessionId">Identificador de la sesion</param>
        /// <param name="appId">Aplicacion a la que pertenece el documento</param>
        /// <param name="docTypeId">Tipo de documento al que pertenece el documento</param>
        /// <param name="queryId">Consulta con la cual se referencio al documento</param>
        /// <param name="docId">Identificador del documento</param>
        /// <returns>Resultado de la operacion, [true] exitoso, [false] fallido</returns>
        //[WebMethod(Description="Metodo que elimina documentos",EnableSession=true)]
        public bool delFullDocument(int sessionId, int appId, int docTypeId, int queryId, int docId)
        {
            lock (sessionId.ToString())
            {
                epSession actual = getSession(sessionId);

                if (actual != null)
                    return actual.delDocument(appId, docTypeId, queryId, docId);
                else
                    return false;
            }
        }

        /// <summary>
        /// Metodo para obtener un documento vacio de un Tipo de Documento especifico
        /// </summary>
        /// <param name="sessionId">Identificador de la sesion</param>
        /// <param name="appId">Aplicacion del documento que se desea obtener</param>
        /// <param name="docTypeId">Tipo de Documento del documento que se desea obtener</param>
        /// <param name="queryId">Consulta con la cual se desea obtener el documento</param>
        /// <returns>Documento con las estructuas vacias</returns>
        //[WebMethod(Description="Metodo que retorno un documento nuevo para salvar",EnableSession=true)]
        public global::Interfaces.Collection getNewDocument(int sessionId, int appId, int docTypeId, int queryId)
        {
            lock (sessionId.ToString())
            {
                epSession actual = getSession(sessionId);

                if (actual != null)
                    return actual.getNewDoc(appId, docTypeId, queryId);
                else
                    throw new Exception("Sesion no existe");
            }
        }
        //[WebMethod(Description = "Metodo que retorna los usuarios asociados a un grupo", EnableSession = true)]
        public global::Session.User[] getUsersByGroup(int sessionId, String groupId)
        {
            lock (sessionId.ToString())
            {
                epSession actual = getSession(sessionId);

                if (actual != null)
                    return actual.getUsersByGroup(groupId);
                else
                    throw new Exception("Sesion no existe");
            }
        }

        //[WebMethod(Description = "Metodo que retorna los grupos asociados a un usuario", EnableSession = true)]
        public global::Session.Group[] getGroupsByUser(int sessionId, String userId)
        {
            lock (sessionId.ToString())
            {
                epSession actual = getSession(sessionId);

                if (actual != null)
                    return actual.getGroupsByUser(userId);
                else
                    throw new Exception("Sesion no existe");
            }
        }
        #endregion
#endif
    }
}