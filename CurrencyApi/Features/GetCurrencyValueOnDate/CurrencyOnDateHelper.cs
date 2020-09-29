using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DailyInfo;

namespace CurrencyApi.Features.GetCurrencyValueOnDate
{
    internal static class CurrencyOnDateHelper
    {
        public static async Task<Dictionary<string, decimal>> LoadCurrencies(string serviceURL, DateTime onDate)
        {
            var client = new DailyInfoSoapClient(new BasicHttpBinding(BasicHttpSecurityMode.None), new EndpointAddress(serviceURL));
            var dataset = ToDataSet((await client.GetCursOnDateAsync(new GetCursOnDateRequest(onDate))).GetCursOnDateResult);
            var result = new Dictionary<string, decimal>();
            foreach (DataRow dataRow in dataset.Tables[0].Rows)
            {
                result.Add(dataRow.ItemArray[4].ToString(),(decimal) dataRow.ItemArray[2]);
            }

            return result;
        }

        private static DataSet ToDataSet(ArrayOfXElement arrayOfXElement)
        {
            var strSchema = arrayOfXElement.Nodes[0].ToString();
            var strData = arrayOfXElement.Nodes[1].ToString();
            var strXml = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n\t<DataSet>";
            strXml += strSchema + strData;
            strXml += "</DataSet>";

            DataSet ds = new DataSet();
            ds.ReadXml(new MemoryStream(Encoding.UTF8.GetBytes(strXml)));

            return ds;
        }
    }
}
