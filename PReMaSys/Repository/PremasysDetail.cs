using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using PReMaSys.Data;
using PReMaSys.Models;
namespace PReMaSys.Repository
{
    public class PremasysDetail : IPremasys
    {
        private IConfiguration _configuration;
        private IWebHostEnvironment _environment;
        private readonly UserManager<ApplicationUser> _userManager;

        public PremasysDetail(IConfiguration configuration, IWebHostEnvironment environment, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _environment = environment;
            _userManager = userManager;
        }


        public string DocumentUpload(IFormFile fromFiles)
        {
            string uploadpath = _environment.WebRootPath;
            string dest_path = Path.Combine(uploadpath, "uploaded_doc");

            if (!Directory.Exists(dest_path))
            {
                Directory.CreateDirectory(dest_path);
            }

            string sourcefile = Path.GetFileName(fromFiles.FileName);
            string path = Path.Combine(dest_path, sourcefile);

            using (FileStream fileStream = new FileStream(path, FileMode.Create))
            {
                fromFiles.CopyTo(fileStream);
            }
            return path;
        }

        public void ImportInventory(DataTable inventory)
        {
            // To Store Data to Database
            var sqlconn = _configuration.GetConnectionString("MyConnection");
            using (SqlConnection scon = new SqlConnection(sqlconn))
            {
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(scon))
                {
                    sqlBulkCopy.DestinationTableName = "SInventories";

                    sqlBulkCopy.ColumnMappings.Add("EmployeeNo", "EmployeeNo");
                    sqlBulkCopy.ColumnMappings.Add("EmployeeFirstname", "EmployeeFirstname");
                    sqlBulkCopy.ColumnMappings.Add("EmployeeLastname", "EmployeeLastname");
                    sqlBulkCopy.ColumnMappings.Add("EmployeeAddress", "EmployeeAddress");
                    sqlBulkCopy.ColumnMappings.Add("EmployeeBirthdate", "EmployeeBirthdate");
                    sqlBulkCopy.ColumnMappings.Add("Email", "Email");
                    sqlBulkCopy.ColumnMappings.Add("Readers", "Readers");
                    sqlBulkCopy.ColumnMappings.Add("Password", "Password");
                    sqlBulkCopy.ColumnMappings.Add("ConfirmPassword", "ConfirmPassword");

                    // Generate hashed passwords for each record
                    foreach (DataRow row in inventory.Rows)
                    {
                        var password = row["Password"].ToString();
                        var confirmPassword = row["ConfirmPassword"].ToString();

                        if (password == confirmPassword)
                        {
                            // Convert the provided password hash to byte array
                            byte[] passwordBytes = Convert.FromBase64String(password);

                            // Hash the password using the provided format
                            var hashedPassword = Encoding.UTF8.GetString(passwordBytes);
                            row["Password"] = hashedPassword;
                            row["ConfirmPassword"] = hashedPassword;
                        }
                    }

                    scon.Open();
                    sqlBulkCopy.WriteToServer(inventory);
                    scon.Close();
                }
            }
        }




        public DataTable InventoryDataTable(string path)
        {
            var constr = _configuration.GetConnectionString("excelconnection");
            DataTable datatable = new DataTable();
            constr = string.Format(constr, path);

            using (OleDbConnection excelconn = new OleDbConnection(constr))
            {
                using (OleDbCommand cmd = new OleDbCommand())
                {
                    using (OleDbDataAdapter adapterexcel = new OleDbDataAdapter())
                    {
                        excelconn.Open();
                        cmd.Connection = excelconn;
                        DataTable excelschema;
                        excelschema = excelconn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        var sheetname = excelschema.Rows[0]["Table_Name"].ToString();
                        excelconn.Close();


                        excelconn.Open();
                        cmd.CommandText = "SELECT * From [" + sheetname + "]";
                        adapterexcel.SelectCommand = cmd;
                        adapterexcel.Fill(datatable);
                        excelconn.Close();
                    }
                }
            }

            return datatable;
        }
    }
}
