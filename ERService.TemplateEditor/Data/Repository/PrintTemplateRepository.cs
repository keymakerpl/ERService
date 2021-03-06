﻿using ERService.Business;
using ERService.Infrastructure.Repositories;
using ERService.MSSQLDataAccess;

namespace ERService.TemplateEditor.Data.Repository
{
    public class PrintTemplateRepository : GenericRepository<PrintTemplate, ERServiceDbContext>, IPrintTemplateRepository
    {
        public PrintTemplateRepository(ERServiceDbContext context) : base(context)
        {
        }
    }
}
