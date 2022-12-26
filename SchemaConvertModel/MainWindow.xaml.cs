using Microsoft.AspNetCore.Razor.Language;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace SchemaConvertModel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async Task Button_ClickAsync(List<Dictionary<string, object>> data)
        {
            try
            {


                var templatePath = "Template.cshtml";
                var k8sValue = new TableSchema()
                {
                    Data = data,
                };
                var k8sDynamicValues = new Dictionary<string, object>
                {
                    ["tablename"] = tbTableName.Text,
                    ["table_catalog"]=tbTABLE_CATALOG.Text,
                    ["source_sql"]=tbSOURCE_SQL.Text
                };
                var engine = new Template();
                var result = await engine.RenderAsync(templatePath, k8sValue, k8sDynamicValues);
                using (StreamWriter writer = new StreamWriter("result.html"))
                {
                    writer.WriteLine(result);
                }
                Console.WriteLine(result);
                tbLog.Text = "ok";
            }
            catch (Exception ex)
            {

                tbLog.Text = ex.Message.ToString();
            }

        }
        public class Template
        {
            public async Task<string> RenderAsync(string templatePath,
                TableSchema k8sValue,
                Dictionary<string, object> k8sDynamicValue)
            {
                return await Razor.Templating.Core.RazorTemplateEngine.RenderAsync(templatePath,
                    k8sValue,
                    k8sDynamicValue);
            }
        }

        public class DataModel
        {
            public string key { get; set; }
            public string value { get; set; }
                
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {

            StringBuilder ReturnMB001 = new StringBuilder();

            List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> data2 = new List<Dictionary<string, object>>();
            XmlDocument doc = new XmlDocument();


            doc.Load("config.xml");
            var root = doc.SelectNodes("config");
            var dataType = root[0].SelectNodes("DataType");
            var DataResult = root[0].SelectNodes("DataResult");
            var goal = dataType[0].Attributes[0].Value;
            var dataTypeItem = dataType[0].SelectNodes("Item");
            var DataResultItem = DataResult[0].SelectNodes("Item");
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            Dictionary<string, string> keyValuePairs2 = new Dictionary<string, string>();
            
            for (int i = 0; i < dataTypeItem.Count; i++)
            {
                keyValuePairs.Add(dataTypeItem[i].Attributes["key"].Value, dataTypeItem[i].Attributes["value"].Value);
            }

            for (int i = 0; i < DataResultItem.Count; i++)
            {
                keyValuePairs2.Add(DataResultItem[i].Attributes["readSite"].Value, DataResultItem[i].Attributes["razorLabel"].Value);
            }

            for (int i = 0; i < tbMultiLine.LineCount; i++)
            {
                string[] key = keyValuePairs2.Keys.ToArray();
                Dictionary<string, object> dicData = new Dictionary<string, object>();
                string[] Row = tbMultiLine.GetLineText(i).Replace("\r\n", "").Split(' ');

                for (int j = 0; j < keyValuePairs2.Count; j++)
                {
                    var flag = Convert.ToInt16( keyValuePairs2.Values.ToList()[j])-1;
                    if (goal == key[j])
                    {
                        var temp = keyValuePairs.Where(kv => Row[flag].Replace("\r\n", string.Empty).Contains(kv.Key)).Select(x => x.Value).FirstOrDefault();


                        if (temp == null)
                        {
                            dicData.Add(key[j].Replace("\r\n", ""), Row[flag].Replace("\r\n", ""));

                        }
                        else
                        {
                            dicData.Add(key[j].Replace("\r\n", ""), temp);

                        }
                    }
                    else
                    {

                        dicData.Add(key[j].Replace("\r\n", ""), Row[flag].Replace("\r\n", ""));
                    }
                }
                data.Add(dicData);


            }

       



            Button_ClickAsync(data);
        }

        private void tbTableName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }




}
