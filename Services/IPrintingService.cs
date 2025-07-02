// Файл: Services/IPrintingService.cs
using RepairServiceAppMVVM.Models;

namespace RepairServiceAppMVVM.Services
{
    public interface IPrintingService
    {
        void ShowPrintPreview(Repair repair);
    }
}