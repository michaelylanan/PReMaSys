using System.Data;
using PReMaSys.Models;

namespace PReMaSys.Repository
{
    public interface IPremasys
    {
        string DocumentUpload(IFormFile fromFiles);
        DataTable InventoryDataTable(string path);
        void ImportInventory(DataTable inventory);
    }
}
