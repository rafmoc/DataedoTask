namespace ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class DataReader
    {
        List<ImportedObject> importedObjects;

        public void ImportAndPrintData(string fileToImport, bool printData = true)
        {
            importedObjects = new List<ImportedObject>();

            StreamReader streamReader = new StreamReader(fileToImport);

            while (!streamReader.EndOfStream)
            {
                string line = streamReader.ReadLine();
                string[] values = line.Split(';');
                ImportedObject importedObject = new ImportedObject()
                {
                    Type = values.ElementAtOrDefault(0)?.Trim().ToUpper(),
                    Name = values.ElementAtOrDefault(1)?.Trim(),
                    Schema = values.ElementAtOrDefault(2)?.Trim(),
                    ParentName = values.ElementAtOrDefault(3)?.Trim(),
                    ParentType = values.ElementAtOrDefault(4)?.Trim(),
                    DataType = values.ElementAtOrDefault(5)?.Trim(),
                    IsNullable = values.ElementAtOrDefault(6)?.Trim(),
                };
                importedObjects.Add(importedObject);
            }

            streamReader.Close();

            // assign number of children
            foreach (var importedObject in importedObjects)
            {
                importedObject.NumberOfChildren = importedObjects.Count(IO =>
                    IO.ParentType == importedObject.Type 
                    && IO.ParentName == importedObject.Name
                );
            }

            if(printData)
            {
                foreach (var database in importedObjects.Where(data => data.Type == "DATABASE"))
                {
                    Console.WriteLine($"Database '{database.Name}' ({database.NumberOfChildren} tables)");

                    // print all database's tables
                    foreach (var table in importedObjects)
                    {
                        if ((table.ParentType?.ToUpper() == database.Type) && (table.ParentName == database.Name))
                        {
                            Console.WriteLine($"\tTable '{table.Schema}.{table.Name}' ({table.NumberOfChildren} columns)");

                            // print all table's columns
                            foreach (var column in importedObjects)
                            {
                                if ((column.ParentType?.ToUpper() == table.Type) && (column.ParentName == table.Name))
                                {
                                    Console.WriteLine($"\t\tColumn '{column.Name}' with {column.DataType} data type " +
                                        $"{(column.IsNullable == "1" ? "accepts nulls" : "with no nulls")}");
                                }
                            }
                        }
                    }
                }
            }
            Console.ReadLine();
        }
    }

    class ImportedObject : ImportedObjectBaseClass
    {
        //Not sure if someone want this new name. If not it can be deleted as parent have this variable
        public new string Name { get; set; } 
        public string Schema { get; set; }
        public string ParentName { get; set; }
        public string ParentType { get; set; }
        public string DataType { get; set; }
        public string IsNullable { get; set; }
        public double NumberOfChildren { get; set; }
    }

    class ImportedObjectBaseClass
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
