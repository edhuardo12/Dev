using System;
using System.Collections;
using System.Configuration;
using System.ComponentModel;
using System.Web;

using ePWS;

namespace EpowerWSManagement
{
    /// <summary>
    /// Variables y metodos globales para el Web Service.
    /// </summary>
    public class Global //: System.Web.HttpApplication
    {
        private System.ComponentModel.IContainer components = null;
        /// <summary>
        /// Contenedor de sesiones
        /// </summary>
        public static ArrayList sessions = null;
        /// <summary>
        /// Direccion UNC de la carpeta Reference para el Web Service
        /// </summary>
        public static String wsRefUNC = null;
        /// <summary>
        /// Direccion HTTP de la carpeta Reference para el Web Service
        /// </summary>
        public static String wsRefHTTP = null;
        /// <summary>
        /// Direccion de la carpeta Reference de ePower
        /// </summary>
        public static String epRef = null;
        /// <summary>
        /// Tiempo de inactividad del Web Service, no es necesario de ePower35st1 en adelante
        /// </summary>
        public static String wsTimeOut = null;
        /// <summary>
        /// URL del IOR de ePower
        /// </summary>
        public static String wsCntURL = null;
        /// <summary>
        /// Tamaño de los paquetes en que se dividirán los archivos
        /// </summary>
        public static int packSize = 0;

        /// <summary>
        /// Constructor por defecto qaue inicializa las variables
        /// </summary>
        public Global()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Metodo que inicializa las variables globales del Web Service
        /// Llamado cuando levanta el Web Service
        /// </summary>
        /// <param name="sender">Objeto que levanto la accion</param>
        /// <param name="e">Argumentos con que levanto el evento</param>
        protected void Application_Start(Object sender, EventArgs e)
        {
            /*try
			{
				int i = 0;
				//System.Runtime.Remoting.RemotingConfiguration.Configure(HttpContext.Current.Request.PhysicalApplicationPath +"Web.Config", false);
			}
			catch (Exception ex)
			{
				int i = 0;
			}*/
            /*wsCntURL = Properties.Settings.Default.ePowerIOR;
			epRef = Properties.Settings.Default.epowerReference;
			wsRefUNC = Properties.Settings.Default.epowerReferenceUNC;
			wsRefHTTP = Properties.Settings.Default.epowerReferenceHTTP;
			wsTimeOut = Properties.Settings.Default.ePowerTimeOut.ToString();
			packSize = Properties.Settings.Default.epowerPacketSize;
			sessions = new ArrayList();

			string[] files = System.IO.Directory.GetFiles(wsRefUNC);
			foreach (string file in files)
			{
				System.IO.File.Delete(file);
			}*/
        }
        /// <summary>
        /// Metodo que se llama cuando se inicia una sesion de Web Service, NO de epower
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Session_Start(Object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Metodo que se llama cuando se inicia un request
        /// NO utilizado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_BeginRequest(Object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Metodo que se llama cuando 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_EndRequest(Object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {

        }

        protected void Application_Error(Object sender, EventArgs e)
        {

        }

        protected void Session_End(Object sender, EventArgs e)
        {

        }

        protected void Application_End(Object sender, EventArgs e)
        {
            //gsiConnector.LogoutWebServer();
        }

        #region Web Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
        }
        #endregion
    }
}
