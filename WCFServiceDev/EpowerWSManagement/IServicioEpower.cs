using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace EpowerWSManagement
{
    [ServiceContract]
    public interface IServicioEpower
    {
        [OperationContract]
        ResultadoAccion DocumentSet(string User, string Password, int appId, int tipeId, int queryId, Datos[] data, Datos[] dataSet, Contenido[] contenido, int pos);
        [OperationContract]
        ResultadoAccion DocumentSetTest(string User, string Password, int appId, int tipeId, int queryId, Datos[] data, Datos[] dataSet, Contenido[] contenido, int pos);
        [OperationContract]
        ResultadoAccion LoginePower(string User, string Password);
        [OperationContract]
        ResultadoAccion LoginePowerTest(string User, string Password);
        [OperationContract]
        List<Resultado> QueryDocument(string User, string Password, int appId, int tipeId, int queryId, Datos[] data);
        [OperationContract]
        List<Resultado> QueryDocumentTest(string User, string Password, int appId, int tipeId, int queryId, Datos[] data);
        [OperationContract]
        int InsertDocument(string User,string Password, string No_Caso, string Institucion, string Fecha, string No_Correspondencia, string Remitente, string Destinatario, string tab, Contenido Imagen, Contenido Otro_Doc);
        [OperationContract]
        bool UpdateDocument(string User, string Password, int DocId, string No_Caso, string Institucion, string Fecha, string No_Correspondencia, string Remitente, string Destinatario, string tab, Contenido Imagen, Contenido Otro_Doc);
        [OperationContract]
        bool DeleteDocument(string User, string Password, int DocId);
    }

    [DataContract]
    public class ResultadoAccion
    {
        bool _resultado = false;
        string _mensaje = "";

        [DataMember]
        public bool Resultado
        {
            get { return _resultado; }
            set { _resultado = value; }
        }
        [DataMember]
        public string Mensaje
        {
            get { return _mensaje; }
            set { _mensaje = value; }
        }
    }

    [DataContract]
    public class Datos
    {
        int tipocontenido = 0;
        string valorstring = "";
        double valordec = 0;

        [DataMember]
        public int TipoContenido
        {
            get { return tipocontenido; }
            set { tipocontenido = value; }
        }
        [DataMember]
        public string ValorString
        {
            get { return valorstring; }
            set { valorstring = value; }
        }
        [DataMember]
        public double ValorDec
        {
            get { return valordec; }
            set { valordec = value; }
        }
    }


    [DataContract]
    public class Contenido
    {
        string binarioBase64 = "";
        string nombreBinario = "";
        string extensionBinario = "";

        [DataMember]
        public string BinarioBase64
        {
            get { return binarioBase64; }
            set { binarioBase64 = value; }
        }
        [DataMember]
        public string NombreBinario
        {
            get { return nombreBinario; }
            set { nombreBinario = value; }
        }
        [DataMember]
        public string ExtensionBinario
        {
            get { return extensionBinario; }
            set { extensionBinario = value; }
        }
    }

    [DataContract]
    public class Resultado
    {
        public int _docid;
        [DataMember]
        public List<ResultadoDet> ItemsDetalle { get; set; }
        [DataMember]
        public List<ResultadoCont> ItemsContenido { get; set; }

        [DataMember]
        public int docId
        {
            get { return _docid; }
            set { _docid = value; }
        }
    }

    [DataContract]
    public class ResultadoDet
    {
        public string _campos, _valores;
        [DataMember]
        public string campos
        {
            get { return _campos; }
            set { _campos = value; }
        }
        [DataMember]
        public string valores
        {
            get { return _valores; }
            set { _valores = value; }
        }
    }

    [DataContract]
    public class ResultadoCont
    {
        public string _ruta, _cejilla;
        [DataMember]
        public string ruta
        {
            get { return _ruta; }
            set { _ruta = value; }
        }
        [DataMember]
        public string cejilla
        {
            get { return _cejilla; }
            set { _cejilla = value; }
        }
    }
}
