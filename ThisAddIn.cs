using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;

namespace OutlookJiraAddIn
{
    public partial class ThisAddIn
    {
        Outlook.Explorer currentExplorer = null;
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            currentExplorer = this.Application.ActiveExplorer();
            currentExplorer.SelectionChange += new Outlook.ExplorerEvents_10_SelectionChangeEventHandler(CurrentExplorer_Event);

        }

        private void CurrentExplorer_Event()
        {
            if (this.Application.ActiveExplorer().Selection.Count > 0)
            {
                Object selObject = this.Application.ActiveExplorer().Selection[1];
                if (selObject is Outlook.MailItem)
                {
                    Outlook.MailItem mailItem = (selObject as Outlook.MailItem);
                }
                else if (selObject is Outlook.ContactItem)
                {
                    Outlook.ContactItem contactItem =
                        (selObject as Outlook.ContactItem);
                }
                else if (selObject is Outlook.AppointmentItem)
                {
                    Outlook.AppointmentItem apptItem = (selObject as Outlook.AppointmentItem);
                }
                else if (selObject is Outlook.TaskItem)
                {
                    Outlook.TaskItem taskItem = (selObject as Outlook.TaskItem);
                }
                else if (selObject is Outlook.MeetingItem)
                {
                    Outlook.MeetingItem meetingItem = (selObject as Outlook.MeetingItem);
                }
            }
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            // Note: Outlook no longer raises this event. If you have code that 
            //    must run when Outlook shuts down, see https://go.microsoft.com/fwlink/?LinkId=506785
        }

        protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            return new OutlookJiraRibbon();
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
