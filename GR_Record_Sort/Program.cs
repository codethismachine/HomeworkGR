using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GR_Record_Sort
{
   
    class Program
    {
        private static DataTable recordTable;
        /// <summary>
        /// The program will take in three files as input and parse the input
        /// then store the information in a datatable for manipulation and output.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            recordTable = new DataTable();
            recordTable.Columns.Add("Last Name", typeof(string));
            recordTable.Columns.Add("First Name", typeof(string));
            recordTable.Columns.Add("Gender", typeof(string));
            recordTable.Columns.Add("Favorite color", typeof(string));
            recordTable.Columns.Add("Date Of Birth", typeof(string));
            recordTable.Columns.Add("DOB For Sorting", typeof(DateTime));
            List<string> fileNameList = new List<string>();

            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Please specify a filename as a parameter.");
                return;
            }

            foreach(string file in args)
            {
                //Added below code for debugging purpose. The command line argument sent from 
                //debug prepended a ?. This had to be removed first.
               string fileToSend = file;
               fileToSend = Regex.Replace(file, @"\?", string.Empty);
               fileNameList.Add(fileToSend);
            }

            ReadFileContents(fileNameList);
           
            DisplayOutput_1(recordTable);
            DisplayOutput_2(recordTable);
            DisplayOutput_3(recordTable);
            Console.Read();

        }
        /// <summary>
        /// Method is used to read in each line of a list of files.
        /// Then the send the content array to the SplitFileContents
        /// method. This method doesn't return a value.
        /// </summary>
        /// <param name="files"></param>
        private static void ReadFileContents(List<string> files)
        {
    
            foreach (string file in files)
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
        }
        /// <summary>
        /// This method seperates the contents of each line based on the delimeter used. 
        /// Then calls the PopulateDataTable method to add the corresponding data.
        /// This method does not return a value.
        /// </summary>
        /// <param name="contentToSplit"></param>
        private static void SplitFileContents(string[] contentToSplit)
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
         private static void PopulateDataTable(DataTable table, string[] field)
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
                DateTime dateOfBirth= DateTime.Parse(rowData.ElementAtOrDefault(4));
                table.Rows.Add(lastName, firstName, gender, favoritColor, dateOfBirthString, dateOfBirth);
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
        }
        /// <summary>
        /// Method is used to display to the console the contents of the 
        /// datatable. The method takes in a datatable as a parameter and has
        /// no return value.
        /// </summary>
        /// <param name="table"></param>
        private static void DisplayDataTableContents(DataTable table)
        {
            foreach (DataRow dataRow in recordTable.Rows)
            {
                Console.WriteLine();
                foreach (var field in dataRow.ItemArray)
                {
                    Console.Write(field + "    ");
                }
            }
        }
        /// <summary>
        /// This method is used to display the data sorted 
        /// by gender then last name ascending. 
        /// First the data is sorted by gender then by last name.
        /// The method has no return value.
        /// </summary>
        /// <param name="table"></param>
        private static void DisplayOutput_1(DataTable table)
        {
            DataView view = table.DefaultView;
            Console.WriteLine("=== Sorted by weight ===");
            for (int i = 0; i < view.Count; i++)
            {
                Console.WriteLine("{0}, {1}, {2}, {3}, {4}",
                    view[i][0],
                    view[i][1],
                    view[i][2],
                    view[i][3],
                    view[i][4]);
            }
            view.Sort = "Gender, Last Name DESC"; 
           // view.Sort = "Last Name DESC";

            Console.WriteLine(Environment.NewLine + "=== Sorted by Gender then Last Name===");
            for (int i = 0; i < view.Count; i++)
            {
                Console.WriteLine("{0}, {1}, {2}, {3}, {4}",
                    view[i][0],
                    view[i][1],
                    view[i][2],
                    view[i][3],
                    view[i][4]);
            }
        }
        /// <summary>
        /// This method is used to display the data 
        /// sorted by birth date ascending. 
        /// First the data is sorted by gender then by last name.
        /// The method has no return value.
        /// </summary>
        /// <param name="table"></param>
        private static void DisplayOutput_2(DataTable table)
        {
            DataView view = table.DefaultView;
            
            view.Sort = "DOB For Sorting ASC";

            Console.WriteLine(Environment.NewLine + "=== Sorted by Date Of Birth===");
            for (int i = 0; i < view.Count; i++)
            {
                Console.WriteLine("{0}, {1}, {2}, {3}, {4}",
                    view[i][0],
                    view[i][1],
                    view[i][2],
                    view[i][3],
                    view[i][4]);
            }
        }
        /// <summary>
        /// This method is used to display the data 
        /// sorted by last name descending. 
        /// First the data is sorted by gender then by last name.
        /// The method has no return value.
        /// </summary>
        /// <param name="table"></param>
        private static void DisplayOutput_3(DataTable table)
        {
            DataView view = table.DefaultView;

            view.Sort = "Last Name";

            Console.WriteLine(Environment.NewLine + "=== Sorted by Last Name===");
            for (int i = 0; i < view.Count; i++)
            {
                Console.WriteLine("{0}, {1}, {2}, {3}, {4}",
                    view[i][0],
                    view[i][1],
                    view[i][2],
                    view[i][3],
                    view[i][4]);
            }
        }
    }
}
