﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CsvToLinq
{
    public class CsvToLinq
    {
        /// <summary>
        /// Reads a CSV file and outputs dynamic objects.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<dynamic> ReadCsv(string filePath)
        {
            return File.ReadAllLines(filePath).ConvertFromCsv();
        }

        /// <summary>
        /// Reads a CSV file, and outputs mapped dynamic objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="mapFunction"></param>
        /// <returns></returns>
        public static List<T> ReadCsv<T>(string filePath, Func<dynamic, T> mapFunction)
        {
            return File.ReadAllLines(filePath).ConvertFromCsv(mapFunction);
        }
    }

    public static class CsvToLinqExtensions
    {
        /// <summary>
        /// Converts a CSV string to dynamic objects.
        /// </summary>
        /// <param name="csvString"></param>
        /// <returns></returns>
        public static List<dynamic> ConvertFromCsv(this string[] csvString)
        {
            // Get all items into rows
            var csvArray = csvString.Select(l => l.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)).ToList();

            // Get header row
            var header = csvArray.First();

            // Sanitize header items
            var sanitizerRegex = new Regex("[^a-zA-Z0-9]");
            header = header.Select(c => sanitizerRegex.Replace(c, string.Empty)).ToArray();
            
            // Get the other rows
            var items = csvArray.Skip(1);

            // Create list of dynamic objects
            var returnList = new List<dynamic>();

            // Populate properties of objects
            foreach (var item in items)
            {
                var listItem = new ExpandoObject();
                var li = listItem as IDictionary<String, object>;
                for (var i = 0; i < header.Count(); i++)
                {
                    li[header[i]] = item[i];
                }
                returnList.Add(listItem);
            }

            return returnList;
        } 

        /// <summary>
        /// Converts a CSV string to dynamic objects, which then get mapped with a MapFunction.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="csvString"></param>
        /// <param name="mapFunction"></param>
        /// <returns></returns>
        public static List<T> ConvertFromCsv<T>(this string[] csvString, Func<dynamic, T> mapFunction)
        {
            var converted = ConvertFromCsv(csvString);
            return converted.Select(i => (T)mapFunction(i)).ToList();
        }
    }
}
