using iSchool.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace iSchool.Svs.Domain
{
    public interface ISvsUnitOfWork : IUnitOfWork, IDisposable
    {
    }
}
