namespace ApiUsers.Models.Validators.Users
{
    public class ImportFromFileValidator : AbstractValidator<ImportFromFileRequest>
    {
        public ImportFromFileValidator()
        {
            RuleFor(r => r.Type)
                .IsInEnum()
                .WithMessage("Tipo de documento no permitido.");

            RuleFor(r => r.LayoutFile)
                .NotNull()
                .Must(a => a.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase)
                || a.FileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase)
                || a.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase)
                || a.FileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Debe proporcionar un archivo valido con las extensiones permitidas (.xlsx, .xls, csv y txt).");
        }
    }
}
