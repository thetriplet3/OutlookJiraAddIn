using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Office = Microsoft.Office.Core;
using Outlook = Microsoft.Office.Interop.Outlook;
using OutlookJiraIssueCreator;
using OutlookJiraAddIn.Properties;
using System.Drawing;
using System.Windows;

// TODO:  Follow these steps to enable the Ribbon (XML) item:

// 1: Copy the following code block into the ThisAddin, ThisWorkbook, or ThisDocument class.

//  protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
//  {
//      return new OutlookJiraRibbon();
//  }

// 2. Create callback methods in the "Ribbon Callbacks" region of this class to handle user
//    actions, such as clicking a button. Note: if you have exported this Ribbon from the Ribbon designer,
//    move your code from the event handlers to the callback methods and modify the code to work with the
//    Ribbon extensibility (RibbonX) programming model.

// 3. Assign attributes to the control tags in the Ribbon XML file to identify the appropriate callback methods in your code.  

// For more information, see the Ribbon XML documentation in the Visual Studio Tools for Office Help.


namespace OutlookJiraAddIn
{
    [ComVisible(true)]
    public class OutlookJiraRibbon : Office.IRibbonExtensibility
    {
        private Office.IRibbonUI ribbon;

        public OutlookJiraRibbon()
        {
        }

        public void OnJiraButton(Office.IRibbonControl control)
        {
            Outlook.MailItem mailItem = null;
            Outlook.Application application = Globals.ThisAddIn.Application;
            Outlook.Explorer explorer = application.ActiveExplorer();
            Outlook.Inspector inspector = application.ActiveInspector();
            
            if (inspector != null)
            {
                if(inspector.CurrentItem is Outlook.MailItem)
                {
                    mailItem = inspector.CurrentItem as Outlook.MailItem;
                }
            }
            else if(explorer != null)
            {
                if (explorer.Selection.Count > 0)
                {
                    mailItem = explorer.Selection[1] as Outlook.MailItem;
                }
                
            }
            MessageBox.Show(application.Name);
            MainWindow main = new MainWindow(mailItem);
            main.ShowDialog();
        }

        public Bitmap LoadImage(Office.IRibbonControl control)
        {
            return Resources.icon;
        }

        #region IRibbonExtensibility Members

        public string GetCustomUI(string ribbonID)
        {
            return GetResourceText("OutlookJiraAddIn.OutlookJiraRibbon.xml");
        }

        #endregion

        #region Ribbon Callbacks
        //Create callback methods here. For more information about adding callback methods, visit https://go.microsoft.com/fwlink/?LinkID=271226

        public void Ribbon_Load(Office.IRibbonUI ribbonUI)
        {
            this.ribbon = ribbonUI;
        }

        #endregion

        #region Helpers

        private static string GetResourceText(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourceNames = asm.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i)
            {
                if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (StreamReader resourceReader = new StreamReader(asm.GetManifestResourceStream(resourceNames[i])))
                    {
                        if (resourceReader != null)
                        {
                            return resourceReader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }

        #endregion
    }
}
