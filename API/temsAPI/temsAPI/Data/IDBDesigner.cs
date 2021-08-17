using Microsoft.EntityFrameworkCore;

namespace temsAPI.Data
{
    public interface IDBDesigner
    {
        void ConfigureModels(ModelBuilder modelbuilder);
    }
}