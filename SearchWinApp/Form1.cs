using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.SharePoint;
using Microsoft.Office.Server.Search;
using Microsoft.Office.Server.Search.Query;


namespace SearchWinApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            this.btnSearch.Enabled = false;
            this.Cursor = Cursors.WaitCursor;
            var results = Task.Factory.StartNew(() =>
            {
                SPSite site = new SPSite("http://intranet.contoso.com");
                KeywordQuery keywordQuery = new KeywordQuery(site);          
                keywordQuery.QueryText = txtSearchTerm.Text;

                SearchExecutor searchExecutor = new SearchExecutor();
                DataTable tableResults = null;
                Dictionary<string, DataTable> retResults = new Dictionary<string, DataTable>();

                ResultTableCollection queryResults = searchExecutor.ExecuteQuery(keywordQuery);

                foreach (ResultTable resultTable in queryResults.Filter("TableType", KnownTableTypes.RelevantResults))
                {
                    tableResults = new DataTable();
                    tableResults.Load(resultTable, LoadOption.OverwriteChanges);
                }

                return tableResults;
            });

            this.dgSearchResults.DataSource = await results;
            this.dgSearchResults.Refresh();
            this.btnSearch.Enabled = true;
            this.Cursor = Cursors.Default; 
        }
    }
}
