using Microsoft.AspNetCore.Mvc;

namespace finance_management.DTOs.ImportTransaction
{
    public class ImportTransactionRequest
    {
        [FromForm(Name="file")]
        public IFormFile File { get; set; }

    }
}
