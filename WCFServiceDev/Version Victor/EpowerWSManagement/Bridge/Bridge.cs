using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Comun;
using ePWS.WS.Type;
using Interfaces;
using GSI;
using ePowerWebService;

namespace EpowerWSManagement
{
    class Bridge
    {
        private static Ewsm manager = null;
        private Ewsm epBridge;

        public Bridge()
        {
            this.epBridge = new Ewsm();
        }

        public static Ewsm getInstance()
        {
            if (manager == null)
            {
                manager = new Ewsm();
            }
            return manager;
        }

        public int doLogin(String user, String password, String module)
        {
            return this.epBridge.doLogin(user, password, module);
        }

        public bool doLogout(int sessionId)
        {
            return this.epBridge.doLogout(sessionId);
        }

        public void disconnectServer()
        {
            this.epBridge.disconnectServer();
        }

        public Collection getNewDocument(int sessionId, int appId, int docTypeId, int queryId)
        {
            return this.epBridge.getNewDocument(sessionId, appId, docTypeId, queryId);
        }

        public bool saveFullDocument(int sessionId, int appId, int docTypeId, int queryId, gsiStructTab[] tabs, gsiStructFld[] entryForm, gsiStructRef[] referenses, gsiStructAnot[] anotations, gsiHit[] hits)
        {
            return this.epBridge.saveFullDocument(sessionId, appId, docTypeId, queryId, tabs, entryForm, referenses, anotations, hits);
        }

        public bool updateFullDocument(int sessionId, int appId, int docTypeId, int queryId, int docId, gsiStructTab[] tabs, gsiStructFld[] entryForm, gsiStructRef[] referenses, gsiStructAnot[] anotations, gsiHit[] hits)
        {
            return this.epBridge.updateFullDocument(sessionId, appId, docTypeId, queryId, docId, tabs, entryForm, referenses, anotations, hits);
        }

        public bool delFullDocument(int sessionId, int appId, int docTypeId, int queryId, int docId)
        {
            return this.epBridge.delFullDocument(sessionId, appId, docTypeId, queryId, docId);
        }

        public Collection getFullDocument(int sessionId, int appId, int docTypeId, int queryId, int docId)
        {
            return epBridge.getFullDocument(sessionId, appId, docTypeId, queryId, docId);
        }

        public QueryInst doExecuteQuery(int sessionId, int appId, int docTypeId, int queryId, int maxRows, string[] fldValue, eTree condition, FullText FTCond)
        {
            return epBridge.doExecuteQuery(sessionId, appId, docTypeId, queryId, maxRows, fldValue, condition, FTCond);
        }

        public gsiStructRef saveRef(int sessionId, int appId, gsiFile docFile)
        {
            return epBridge.saveRef(sessionId, appId, docFile);
        }
    }
}