using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web.Configuration;
using Newtonsoft.Json.Linq;

namespace GR_Record_Sort_API.Controllers
{
    [RoutePrefix("api/GR_Record_Sort/records")]
    public class GR_Record_SortController : ApiController
    {
        //Added file path to the web.config file to be adjusted dynamically
        private string dataPath = WebConfigurationManager.AppSettings["DataFilePath"];
        Program test = new Program();
        // GET: api/GR_Record_Sort
        [HttpGet]
        [Route("{sortBy}")]
        public JObject GetBySort(string sortBy)
        {
            string one = test.Main(sortBy);
            JObject jsonObject = new JObject();
            try
            {
                jsonObject = JObject.Parse(one);
            }
            catch (Exception e)
            {
                jsonObject = null;
            }
            return jsonObject;
        }
        public string Get(int id)
        {
            return "" + id;
        }
        //POST: api/GR_Record_Sort
        public void Post(HttpRequestMessage incomingRequest)
        {
            var content = incomingRequest.Content;
            string jsonContent = content.ReadAsStringAsync().Result;
            AddContentToDataFile(jsonContent);
        }
        /// <summary>
        /// Method returns an List of strings after parsing the
        /// incoming json.
        /// </summary>
        /// <param name="jsonContent"></param>
        /// <returns></returns>
        private void AddContentToDataFile(string jsonContent)
        {
            string dataString = "";
            JsonTextReader reader = new JsonTextReader(new StringReader(jsonContent));
            while (reader.Read())
            {
                if (reader.Value != null && reader.Value.ToString() != "info")
                {
                    //Console.WriteLine("Token: {0}, Value: {1}", reader.TokenType, reader.Value);
                    dataString = reader.Value.ToString();
                }
            }
            using (StreamWriter dataWriter = File.AppendText(dataPath))
            {
                dataWriter.WriteLine("\r" + dataString);
            }
        }
        // PUT: api/GR_Record_Sort/5
        public void Put(int id, [FromBody]string value)
        {
        }
        // DELETE: api/GR_Record_Sort/5
        public void Delete(int id)
        {
        }
    }
    public class Program
    {
        public static DataTable recordTable;
        private string dataPath = WebConfigurationManager.AppSettings["DataFilePath"];
        public string Main(string sortBy)
        {
            recordTable = new DataTable();
            recordTable.Columns.Add("Last Name", typeof(string));
            recordTable.Columns.Add("First Name", typeof(string));
            recordTable.Columns.Add("Gender", typeof(string));
            recordTable.Columns.Add("Favorite color", typeof(string));
            recordTable.Columns.Add("Date Of Birth", typeof(string));
            recordTable.Columns.Add("DOB For Sorting", typeof(DateTime));
            if (dataPath == null || dataPath.Length == 0)
            {
                // Console.WriteLine("Please specify a filename as a parameter.");
                return "Error";
            }
            //Added below code for debugging purpose. The command line argument sent from
            //debug prepended a ?. This had to be removed first.
            string fileToSend = dataPath;
            fileToSend = Regex.Replace(dataPath, @"\?", string.Empty);
            ReadFileContents(fileToSend);
            string output = null;
            if (sortBy.ToUpper().Equals("GENDER"))
            {
                output = DisplayOutput_1(recordTable);
            }
            else if (sortBy.ToUpper().Equals("BIRTHDATE"))
            {
                output = DisplayOutput_2(recordTable);
            }
            else if (sortBy.ToUpper().Equals("NAME"))
            {
                output = DisplayOutput_3(recordTable);
            }
            else
            {
                output = "";
            }
            if (!String.IsNullOrEmpty(output))
            {
                return output;
            }
            return "Output Empty!";
        }
        /// <summary>
        /// Method is used to read in each line of a list of files.
        /// Then the send the content array to the SplitFileContents
        /// method. This method doesn't return a value.
        /// </summary>
        /// <param name="files"></param>
        public static void ReadFileContents(string file)
        {
            try
            {
                string[] fileContentsArray = File.ReadAllLines(file);
                SplitFileContents(fileContentsArray);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + Environment.NewLine + file);
            }
        }
        /// <summary>
        /// This method seperates the contents of each line based on the delimeter used.
        /// Then calls the PopulateDataTable method to add the corresponding data.
        /// This method does not return a value.
        /// </summary>
        /// <param name="contentToSplit"></param>
        public static void SplitFileContents(string[] contentToSplit)
        {
            foreach (string line in contentToSplit)
            {
                string[] splitOnDelimeterArray = Regex.Split(line, @"\||,|\s");
                PopulateDataTable(recordTable, splitOnDelimeterArray);
            }
        }
        /// <summary>
        /// This method is used to populate a datatable with the contents
        /// of a line in a given file. The methad takes in a datatable and
        /// a string arrray of the fields in the line of text. The method
        /// does not return anything.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="field"></param>
        public static void PopulateDataTable(DataTable table, string[] field)
        {
            List<string> rowData = new List<string>();
            foreach (string result in field)
            {
                rowData.Add(result);
            }
            try
            {
                string lastName = rowData.ElementAtOrDefault(0);
                string firstName = rowData.ElementAtOrDefault(1);
                string gender = rowData.ElementAtOrDefault(2);
                string favoritColor = rowData.ElementAtOrDefault(3);
                string dateOfBirthString = rowData.ElementAtOrDefault(4);
                DateTime dateOfBirth = DateTime.Parse(rowData.ElementAtOrDefault(4));
                table.Rows.Add(lastName.ToUpper(), firstName.ToUpper(), gender.ToUpper(), favoritColor.ToUpper(), dateOfBirthString, dateOfBirth);
            }
            catch (Exception e)
            {
                return;
            }
        }
        /// <summary>
        /// This method is used to display the data sorted
        /// by gender then last name ascending.
        /// First the data is sorted by gender then by last name.
        /// The method has no return value.
        /// </summary>
        /// <param name="table"></param>
        public static string DisplayOutput_1(DataTable table)
        {
            var newDataTable = table.AsEnumerable()
                    .OrderBy(r => r.Field<string>("Gender"))
                    .ThenBy(r => r.Field<string>("Last Name"))
                    .CopyToDataTable();
            if (table == null)
            {
                return "Table Empty";
            }
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            JsonWriter writer = new JsonTextWriter(sw);
            writer.WriteStartObject();
            int counter = 1;
            foreach (DataRow row in newDataTable.Rows)
            {
                StringBuilder displayString = new StringBuilder();
                displayString.Append(row["Last Name"].ToString());
                displayString.Append("  ");
                displayString.Append(row["First Name"].ToString());
                displayString.Append("  ");
                displayString.Append(row["Gender"].ToString());
                displayString.Append("  ");
                displayString.Append(row["Favorite color"].ToString());
                displayString.Append("  ");
                displayString.Append(row["Date Of Birth"].ToString());
                writer.WritePropertyName("Person info" + counter);
                writer.WriteValue(displayString.ToString());
                counter++;
            }
            writer.WriteEndObject();
            newDataTable.Dispose();
            return sb.ToString();
        }
        /// <summary>
        /// This method is used to display the data
        /// sorted by birth date ascending.
        /// First the data is sorted by gender then by last name.
        /// The method has no return value.
        /// </summary>
        /// <param name="table"></param>
        public static string DisplayOutput_2(DataTable table)
        {
            var newDataTable = table.AsEnumerable()
                     .OrderBy(r => r.Field<DateTime>("DOB For Sorting"))
                     .CopyToDataTable();
            if (table == null)
            {
                return "Table Empty";
            }
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            JsonWriter writer = new JsonTextWriter(sw);
            writer.WriteStartObject();
            int counter = 1;
            foreach (DataRow row in newDataTable.Rows)
            {
                StringBuilder displayString = new StringBuilder();
                displayString.Append(row["Last Name"].ToString());
                displayString.Append("  ");
                displayString.Append(row["First Name"].ToString());
                displayString.Append("  ");
                displayString.Append(row["Gender"].ToString());
                displayString.Append("  ");
                displayString.Append(row["Favorite color"].ToString());
                displayString.Append("  ");
                displayString.Append(row["Date Of Birth"].ToString());
                writer.WritePropertyName("Person info" + counter);
                writer.WriteValue(displayString.ToString());
                counter++;
            }
            writer.WriteEndObject();
            newDataTable.Dispose();
            return sb.ToString();
        }
        /// <summary>
        /// This method is used to display the data
        /// sorted by last name descending.
        /// First the data is sorted by gender then by last name.
        /// The method has no return value.
        /// </summary>
        /// <param name="table"></param>
        public static string DisplayOutput_3(DataTable table)
        {
            var newDataTable = table.AsEnumerable()
                     .OrderByDescending(r => r.Field<string>("Last Name"))
                     .CopyToDataTable();
            if (table == null)
            {
                return "Table Empty";
            }
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            JsonWriter writer = new JsonTextWriter(sw);
            writer.WriteStartObject();
            int counter = 1;
            foreach (DataRow row in newDataTable.Rows)
            {
                StringBuilder displayString = new StringBuilder();
                displayString.Append(row["Last Name"].ToString());
                displayString.Append("  ");
                displayString.Append(row["First Name"].ToString());
                displayString.Append("  ");
                displayString.Append(row["Gender"].ToString());
                displayString.Append("  ");
                displayString.Append(row["Favorite color"].ToString());
                displayString.Append("  ");
                displayString.Append(row["Date Of Birth"].ToString());
                writer.WritePropertyName("Person info" + counter);
                writer.WriteValue(displayString.ToString());
                counter++;
            }
            writer.WriteEndObject();
            newDataTable.Dispose();
            return sb.ToString();
        }
    }
}
