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
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Please specify a filename as a parameter.");
                return;
            }
            Console.WriteLine("args[0] = " + args[0]);
            string fileOne = args[0].ToString();
            //Added below code for debugging purpose. The command line argument sent from 
            //debug prepended a ?. This had to be removed first.
            fileOne = fileOne.Remove(0, 1);
            Console.WriteLine("fileOne = " + fileOne);
            string[] fileContents = File.ReadAllLines(fileOne); 
            foreach(string line in fileContents)
            {
                string[] splitOnDelimeter = Regex.Split(line, @"\|");
                foreach(string result in splitOnDelimeter)
                {
                    Console.Write(result);
                }
            }
            // Safely create and dispose of a DataTable
            //using (DataTable table = new DataTable())
            //{
            //    // Two columns.
            //    table.Columns.Add("Last Name", typeof(string));
            //    table.Columns.Add("First Name", typeof(string));
            //    table.Columns.Add("Gender", typeof(string));
            //    table.Columns.Add("Favorite color", typeof(string));
            //    table.Columns.Add("Date Of Birth", typeof(DateTime));
            //    // ... Add two rows.
            //    table.Rows.Add("cat", DateTime.Now);
            //    table.Rows.Add("dog", DateTime.Today);

            //    // ... Display first field.
            //    Console.WriteLine(table.Rows[0].Field<string>(0));
            //}
            //Console.WriteLine(fileContents);


            Console.Read();

        }
         
    }
}
